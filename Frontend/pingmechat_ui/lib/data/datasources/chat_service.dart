import 'dart:convert';
import 'dart:io';

import 'package:pingmechat_ui/data/datasources/file_upload_service.dart';
import 'package:mime/mime.dart';
import 'package:pingmechat_ui/providers/auth_provider.dart';
import 'package:http_parser/http_parser.dart';
import 'package:signalr_core/signalr_core.dart';

import '../../core/constants/constant.dart';
import '../../domain/models/chat.dart';
import '../../domain/models/message.dart';
import '../../domain/models/userchat.dart';
import '../models/chat_model.dart';
import 'package:http/http.dart' as http;

class ChatService {
  final AuthProvider authProvider;
  HubConnection? hubConnection;
  bool isConnected = false;
  bool _listenersRegistered = false; // Đã đăng ký các listeners hay chưa

  // Callbacks for different events
  Function(Chat)? onNewGroupChatCallback;
  Function(Chat)? onNewPrivateChatCallback;
  Function(Message)? onReceiveMessageCallback;
  Function(String, String)? onUserTypingCallback;
  Function(String, String)? onUserStopTypingCallback;

  ChatService({required this.authProvider}) {
    authProvider.addListener(_handleAuthChange);
  }

  void _handleAuthChange() {
    if (authProvider.isAuth && !isConnected) {
      _initialize();
    } else if (!authProvider.isAuth && isConnected) {
      _disconnect();
    }
  }

  Future<void> _initialize() async {
    if (hubConnection != null) {
      // Nếu hubConnection không phải là null và đang hoạt động, không cần khởi tạo lại
      if (hubConnection?.state == HubConnectionState.connected ||
          hubConnection?.state == HubConnectionState.connecting) {
        print("Đã có kết nối trước đó, không cần khởi tạo lại.");
        return;
      }
    }
    await _setupSignalRConnection();
    _setupSignalRListeners();
  }

  Future<void> _setupSignalRConnection() async {
    if (isConnected) return;
    if (!authProvider.isAuth) {
      print("Lỗi: Người dùng chưa đăng nhập");
      throw Exception("Người dùng chưa đăng nhập");
    }
    try {
      // Khởi tạo kết nối HubConnection
      hubConnection = HubConnectionBuilder()
          .withUrl(
            ChatHubConstants.chatHubUrl,
            HttpConnectionOptions(
              accessTokenFactory: () async => authProvider.accessToken!,
              transport: HttpTransportType.webSockets,
              customHeaders: await authProvider.getCustomHeaders(),
            ),
          )
          .withAutomaticReconnect()
          .build();

      final originalUrl = hubConnection!.baseUrl;
      final uriBuilder = Uri.parse(originalUrl).replace(queryParameters: {
        'access_token': authProvider.accessToken!,
        'refresh_token': authProvider.refreshToken!,
      });
      hubConnection!.baseUrl = uriBuilder.toString();

      await hubConnection!.start(); // Bắt đầu kết nối
      isConnected = true;
      print('Đã kết nối đến SignalR hub');
    } catch (e) {
      print('Có lỗi khi kết nối đến SignalR hub: $e');
      hubConnection = null;
      isConnected = false;
    }
  }

  void _setupSignalRListeners() {
    hubConnection?.onclose((error) {
      print('Kết nối đã đóng: $error');
      isConnected = false;
    });

    hubConnection?.onreconnecting((error) {
      print('Kết nối đang kết nối lại: $error');
      isConnected = false;
    });

    hubConnection?.onreconnected((connectionId) {
      print('Kết nối hub đã kết nối lại: $connectionId');
      isConnected = true;
    });

    hubConnection?.on('NewGroupChat', (arguments) {
      if (arguments != null && arguments.isNotEmpty) {
        final chat = Chat.fromJson(arguments[0]);
        onNewGroupChatCallback?.call(chat);
      }
    });

    hubConnection?.on('NewPrivateChat', (arguments) {
      if (arguments != null && arguments.isNotEmpty) {
        final chat = Chat.fromJson(arguments[0]);
        onNewPrivateChatCallback?.call(chat);
      }
    });

    hubConnection?.on('ReceiveMessage', (arguments) {
      if (arguments != null && arguments.isNotEmpty) {
        final message = Message.fromJson(arguments[0]);
        onReceiveMessageCallback?.call(message);
      }
    });

    hubConnection?.on('UserTyping', (arguments) {
      if (arguments != null && arguments.length >= 2) {
        final chatId = arguments[0] as String;
        final userId = arguments[1] as String;
        onUserTypingCallback?.call(chatId, userId);
      }
    });

    hubConnection?.on('UserStopTyping', (arguments) {
      if (arguments != null && arguments.length >= 2) {
        final chatId = arguments[0] as String;
        final userId = arguments[1] as String;
        onUserStopTypingCallback?.call(chatId, userId);
      }
    });

    _listenersRegistered = true;
  }

  Future<void> _disconnect() async {
    if (hubConnection != null) {
      await hubConnection?.stop();
      // Xoá các listeners đã đăng ký
      hubConnection?.off('NewGroupChat');
      hubConnection?.off('NewPrivateChat');
      hubConnection?.off('ReceiveMessage');
      hubConnection?.off('UserTyping');
      hubConnection?.off('UserStopTyping');
      hubConnection = null;
      _listenersRegistered = false;
    }
    isConnected = false;
    print('Đã ngắt kết nối đến SignalR hub');
  }

  String getCurrentUserId() {
    return authProvider.currentUser?.id ?? '';
  }

// Hàm lấy danh sách các cuộc trò chuyện
  Future<List<Chat>> getChats({int page = 1, int pageSize = 20}) async {
    try {
      final response = await http.get(
        Uri.parse(
            '${ApiConstants.getChatListEndpoint}?pageNumber=$page&pageSize=$pageSize'),
        headers: await authProvider.getCustomHeaders(),
      );

      if (response.statusCode == 200) {
        final jsonResponse = json.decode(response.body);

        // Lấy phần 'result' và truy cập vào 'data' trong đó
        final result = jsonResponse['result'];
        final List<dynamic> data = result['data'];

        // Chuyển đổi từng phần tử trong 'data' thành đối tượng 'Chat'
        List<Chat> chatList = data.map((json) => Chat.fromJson(json)).toList();

        return chatList;
      } else {
        throw Exception(
            'HTTP error ${response.statusCode}: ${response.reasonPhrase}');
      }
    } catch (e) {
      throw Exception('Failed to load chats: $e');
    }
  }

// Hàm lấy cuộc trò chuyện dựa theo id
  Future<Chat> getChatById(String chat) async {
    try {
      final response = await http.get(
        Uri.parse(ApiConstants.getChatByIdEndpoint(chat)),
        headers: await authProvider.getCustomHeaders(),
      );

      if (response.statusCode == 200) {
        final jsonResponse = json.decode(response.body);

        // Lấy phần 'result' và truy cập vào 'data' trong đó
        final data = jsonResponse['result'];

        // Chuyển đổi từng phần tử trong 'data' thành đối tượng 'Chat'
        Chat chat = Chat.fromJson(data);

        return chat;
      } else {
        throw Exception(
            'HTTP error ${response.statusCode}: ${response.reasonPhrase}');
      }
    } catch (e) {
      throw Exception('Failed to load chat: $e');
    }
  }

// Hàm lấy danh sách tin nhắn của một cuộc trò chuyện
  Future<List<Message>> getMessages(
      {required String chatId, int page = 1, int pageSize = 20}) async {
    try {
      final response = await http.get(
        Uri.parse(ApiConstants.getMessagesEndpoint(chatId, page, pageSize)),
        headers: await authProvider.getCustomHeaders(),
      );

      if (response.statusCode == 200) {
        final jsonResponse = json.decode(response.body);

        // Lấy phần 'result' và truy cập vào 'data' trong đó
        final result = jsonResponse['result'];
        final List<dynamic> data = result['data'];

        // Chuyển đổi từng phần tử trong 'data' thành đối tượng 'Message'
        List<Message> messageList =
            data.map((json) => Message.fromJson(json)).toList();

        return messageList;
      } else {
        throw Exception(
            'HTTP error ${response.statusCode}: ${response.reasonPhrase}');
      }
    } catch (e) {
      throw Exception('Failed to load messages: $e');
    }
  }

  Future<void> sendMessage(MessageSendDto input) async {
    await hubConnection!.invoke('SendMessage', args: [input]);
  }

  void onMessageStatusUpdate(
      void Function(String messageId, MessageStatus status) handler) {
    hubConnection!.on('MessageStatusUpdate', (arguments) {
      if (arguments != null && arguments.length >= 2) {
        final messageId = arguments[0] as String;
        final statusString = arguments[1] as String;
        final status = MessageStatus.values.firstWhere(
          (e) => e.toString().split('.').last == statusString,
          orElse: () => MessageStatus.sent,
        );
        handler(messageId, status);
      }
    });
  }

  Future<void> startNewChat(ChatCreateDto chatCreateDto) async {
    await hubConnection!.invoke('StartNewChatAsync', args: [chatCreateDto]);
  }

  void onNewGroupChat(void Function(Chat chat) handler) {
    hubConnection!.on('NewGroupChat', (arguments) {
      if (arguments != null && arguments.isNotEmpty) {
        handler(Chat.fromJson(arguments[0]));
      }
    });
  }

  void onNewPrivateChat(void Function(Chat chat) handler) {
    hubConnection!.on('NewPrivateChat', (arguments) {
      if (arguments != null && arguments.isNotEmpty) {
        handler(Chat.fromJson(arguments[0]));
      }
    });
  }

  void onReceiveMessage(void Function(Message message) handler) {
    hubConnection!.on('ReceiveMessage', (arguments) {
      if (arguments != null && arguments.isNotEmpty) {
        final messageData = arguments[0] as Map<String, dynamic>;
        final message = Message.fromJson(messageData);
        handler(message);
      }
    });
  }

  void onUserTyping(void Function(String chatId, String userId) handler) {
    hubConnection!.on('UserTyping', (arguments) {
      if (arguments != null && arguments.length >= 2) {
        handler(arguments[0] as String, arguments[1] as String);
      }
    });
  }

  void onUserStopTyping(void Function(String chatId, String userId) handler) {
    hubConnection!.on('UserStopTyping', (arguments) {
      if (arguments != null && arguments.length >= 2) {
        handler(arguments[0] as String, arguments[1] as String);
      }
    });
  }

  // Thêm các phương thức mở rộng
  Future<List<UploadResult>> uploadFiles(List<File> files) async {
    final uri = Uri.parse(ApiConstants.uploadFileEndpoint);

    // Tạo request Multipart
    final request = http.MultipartRequest('POST', uri);

    // Thêm các file vào request
    for (var file in files) {
      // Lấy ContentType của file
      final mimeType = lookupMimeType(file.path);

      // Thêm file vào request với ContentType
      request.files.add(await http.MultipartFile.fromPath(
        'files',
        file.path,
        contentType: mimeType != null ? MediaType.parse(mimeType) : null,
      ));
    }

    // Thêm headers (authentication, etc.)
    request.headers.addAll(await authProvider.getCustomHeaders());

    // Gửi request
    final response = await request.send();

    if (response.statusCode == 200) {
      final responseBody = await response.stream.bytesToString();
      final jsonResponse = json.decode(responseBody);

      // Xử lý response cho từng file (giả sử server trả về danh sách các kết quả upload)
      List<UploadResult> results = [];
      for (var fileJson in jsonResponse['result']) {
        results.add(UploadResult(
            publicId: fileJson['publicId'],
            url: fileJson['url'],
            fileName: fileJson['fileName'],
            fileType: fileJson['fileType'],
            fileSize: fileJson['fileSize']));
      }
      return results;
    } else {
      throw Exception('Failed to upload files');
    }
  }

  Future<void> sendUserTyping(String chatId) async {
    await hubConnection!.invoke('UserTyping', args: [chatId]);
  }

  Future<void> sendUserStopTyping(String chatId) async {
    await hubConnection!.invoke('UserStopTyping', args: [chatId]);
  }

  Future<List<UserChat>> addMembersToChat(
      String chatId, List<String> selectedMembers) async {
    try {
      final response = await http.post(
        Uri.parse(ApiConstants.getAddMembersToChatEndpoint(chatId)),
        headers: {
          'Content-Type': 'application/json ; charset=UTF-8',
          ...await authProvider.getCustomHeaders(),
        },
        body: json.encode(selectedMembers),
      );

      if (response.statusCode == 200) {
        final jsonResponse = json.decode(response.body);
        final List<dynamic> data = jsonResponse['result'];

        // Kiểm tra xem data có null hay không và chuyển đổi từng phần tử trong 'data' thành đối tượng 'UserChat'
        List<UserChat> newMembers =
            data.map((json) => UserChat.fromJson(json)).toList();

        return newMembers;
      } else {
        throw Exception(
            'HTTP error ${response.statusCode}: ${response.reasonPhrase}');
      }
    } catch (e) {
      throw Exception('Failed to load messages: $e');
    }
  }

  Future<bool> removeMemberFromChat(String chatId, String userId) async {
    try {
      final uri =
          Uri.parse(ApiConstants.getRemoveMemberFromChatEndpoint(chatId))
              .replace(queryParameters: {
        'userId': userId,
      });
      final response = await http.delete(uri,
          headers: await authProvider.getCustomHeaders());

      if (response.statusCode == 200) {
        final jsonResponse = json.decode(response.body);
        final data = jsonResponse['result'] as bool;

        // Kiểm tra xem data có null hay không và chuyển thành true/false
        return data;
      } else {
        throw Exception(
            'HTTP error ${response.statusCode}: ${response.reasonPhrase}');
      }
    } catch (e) {
      throw Exception('Failed to remove member from chat: $e');
    }
  }
}
