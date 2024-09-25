import 'dart:convert';
import 'dart:io';

import 'package:pingmechat_ui/data/datasources/file_upload_service.dart';
import 'package:pingmechat_ui/providers/auth_provider.dart';

import '../../core/constants/constant.dart';
import '../../domain/models/chat.dart';
import '../../domain/models/message.dart';
import '../models/chat_model.dart';
import 'chat_hub_service.dart';
import 'package:http/http.dart' as http;

class ChatService {
  final ChatHubService chatHubService;
  final AuthProvider authProvider;

  ChatService({required this.chatHubService, required this.authProvider});

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

// Các phương thức ChatHubService sẽ được gọi từ ChatService
  Future<void> initialize() async {
    await chatHubService.connect();
  }

  Future<void> sendMessage(MessageSendDto input) async {
    await chatHubService.sendMessage(input);
  }

  Future<void> startNewChat(ChatCreateDto chatCreateDto) async {
    await chatHubService.startNewChat(chatCreateDto);
  }

  void onNewGroupChat(void Function(Chat chat) handler) {
    chatHubService.onNewGroupChat(handler);
  }

  void onNewPrivateChat(void Function(Chat chat) handler) {
    chatHubService.onNewPrivateChat(handler);
  }

  // Thêm các phương thức mở rộng
 Future<List<UploadResult>> uploadFiles(List<File> files) async {
    final uri = Uri.parse(ApiConstants.uploadFileEndpoint);

    // Tạo request Multipart
    final request = http.MultipartRequest('POST', uri);

    // Thêm các file vào request
    for (var file in files) {
      request.files.add(await http.MultipartFile.fromPath('files', file.path));
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
      for (var fileJson in jsonResponse['files']) {
        results.add(UploadResult(url: fileJson['url']));
      }
      return results;
    } else {
      throw Exception('Failed to upload files');
    }
  }
}
