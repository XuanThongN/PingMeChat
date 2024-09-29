import 'dart:async';
import 'dart:convert';

import 'package:pingmechat_ui/data/models/chat_model.dart';
import 'package:pingmechat_ui/domain/models/message.dart';
import 'package:signalr_core/signalr_core.dart';

import '../../core/constants/constant.dart';
import '../../domain/models/chat.dart';
import '../../providers/auth_provider.dart';

class ChatHubService {
  late HubConnection _hubConnection;
  final AuthProvider authProvider;
  final Completer<void> _connectionCompleter = Completer<void>();
  bool _isConnected = false;

  ChatHubService(this.authProvider) {
    authProvider.addListener(_handleAuthChange);
    _initializeConnection();
  }

  Future<void> _initializeConnection() async {
    if (!authProvider.isAuth) {
      print('User not authenticated. Skipping connection initialization.');
      return;
    }
    try {
      _hubConnection = await _buildHubConnection();
      // Thêm token vào URL cho WebSocket connection
      final originalUrl = _hubConnection.baseUrl;
      final uriBuilder = Uri.parse(originalUrl).replace(queryParameters: {
        'access_token': authProvider.accessToken!,
        'refresh_token': authProvider.refreshToken!,
      });
      _hubConnection.baseUrl = uriBuilder.toString();
      _setupConnectionHandlers();

      await _hubConnection.start();
      print('Connected to SignalR hub');
      _connectionCompleter.complete();
    } catch (e) {
      print('Error initializing connection: $e');
      await _handleConnectionError(e);
    }
  }

  Future<void> connect() async {
    await _connectionCompleter.future;
  }

  Future<HubConnection> _buildHubConnection() async {
    return HubConnectionBuilder()
        .withUrl(
          ChatHubConstants.chatHubUrl,
          HttpConnectionOptions(
            accessTokenFactory: () async => authProvider.accessToken!,
            transport: HttpTransportType.webSockets,
            customHeaders: await authProvider.getCustomHeaders(),
            // logging: (level, message) => print('SignalR: $level - $message'),
          ),
        )
        .build();
  }

  void _setupConnectionHandlers() {
    _hubConnection.onclose((error) async {
      print('Connection closed: $error');
      await reconnect();
    });
  }

  Future<void> _handleConnectionError(dynamic e) async {
    print('Error connecting to SignalR hub: $e');
    _isConnected = false;
    if (e.toString().contains('401')) {
      print('Authentication failed: Invalid access token');
    } else {
      print('Error connecting to SignalR hub: $e');
      await reconnect();
    }
  }

  Future<void> reconnect() async {
    const int maxAttempts = 5;
    int attempt = 0;
    while (attempt < maxAttempts) {
      attempt++;
      print('Reconnecting attempt $attempt...');
      try {
        await _hubConnection.start();
        print('Reconnected to SignalR hub');
        return;
      } catch (e) {
        print('Reconnection attempt $attempt failed: $e');
        await Future.delayed(const Duration(seconds: 2));
      }
    }
    print('Failed to reconnect after $maxAttempts attempts');
  }

  void _handleAuthChange() {
    if (!authProvider.isAuth && _isConnected) {
      _disconnectHub();
    } else if (authProvider.isAuth && !_isConnected) {
      _initializeConnection();
    }
  }

  Future<void> _disconnectHub() async {
    await _hubConnection.stop();
    _isConnected = false;
    print('Disconnected from SignalR hub');
  }

  // Phương thức mới để đóng kết nối và dọn dẹp tài nguyên
  Future<void> close() async {
    authProvider.removeListener(_handleAuthChange);
    await _disconnectHub();
    print('ChatHubService closed and resources cleaned up');
  }

  Future<void> sendMessage(MessageSendDto message) async {
    await _connectionCompleter.future;
    await _hubConnection.invoke('SendMessage', args: [message]);
  }

  Future<void> startNewChat(ChatCreateDto chatCreateDto) async {
    await _connectionCompleter.future;
    await _hubConnection
        .invoke('StartNewChatAsync', args: [chatCreateDto.toJson()]);
  }

  void onReceiveMessage(void Function(Message message) handler) {
    _hubConnection.on('ReceiveMessage', (arguments) {
      print('Received data: $arguments');
      if (arguments == null || arguments.isEmpty) return;
      final messageData = arguments[0] as Map<String, dynamic>;
      final messsage = Message.fromJson(messageData);
      handler(messsage);
    });
  }

  void onNewGroupChat(void Function(Chat chat) handler) {
    _hubConnection.on('NewGroupChat', (arguments) {
      if (arguments != null && arguments.isNotEmpty) {
        handler(Chat.fromJson(arguments[0]));
      }
    });
  }

  void onNewPrivateChat(void Function(Chat chat) handler) {
    _hubConnection.on('NewPrivateChat', (arguments) {
      if (arguments != null && arguments.isNotEmpty) {
        handler(Chat.fromJson(arguments[0]));
      }
    });
  }

  Future<void> joinChat(String chatId) async {
    await _connectionCompleter.future;
    await _hubConnection.invoke('JoinChat', args: [chatId]);
  }

  Future<void> sendMessageToGroup(String chatId, String message) async {
    await _connectionCompleter.future;
    await _hubConnection.invoke('SendMessageToGroup', args: [chatId, message]);
  }

  Future<void> userIsTyping(String chatId) async {
    await _connectionCompleter.future;
    await _hubConnection.invoke('UserIsTyping', args: [chatId]);
  }

  Future<void> userStoppedTyping(String chatId) async {
    await _connectionCompleter.future;
    await _hubConnection.invoke('UserStoppedTyping', args: [chatId]);
  }

  void onUserTyping(void Function(String userId) handler) {
    _hubConnection.on('UserTyping', (arguments) {
      if (arguments != null && arguments.isNotEmpty) {
        handler(arguments[0] as String);
      }
    });
  }

  void onUserStoppedTyping(void Function(String userId) handler) {
    _hubConnection.on('UserStoppedTyping', (arguments) {
      if (arguments != null && arguments.isNotEmpty) {
        handler(arguments[0] as String);
      }
    });
  }

  // Phương thức để khởi tạo cuộc gọi
  Future<void> initiateCall(String chatId, bool isVideo) async {
    await _connectionCompleter.future;
    await _hubConnection.invoke('InitiateCall', args: [chatId, isVideo]);
  }

  // Phương thức để trả lời cuộc gọi
  Future<void> answerCall(String chatId, bool accept) async {
    await _connectionCompleter.future;
    await _hubConnection.invoke('AnswerCall', args: [chatId, accept]);
  }

  // Phương thức để gửi ICE candidate
  Future<void> sendIceCandidate(
      String chatId, Map<String, dynamic> candidate) async {
    await _connectionCompleter.future;
    String candidateString = jsonEncode(candidate);
    await _hubConnection
        .invoke('IceCandidate', args: [chatId, candidateString]);
  }

  // Phương thức để gửi offer
  Future<void> sendOffer(String chatId, String sdp) async {
    await _connectionCompleter.future;
    await _hubConnection.invoke('Offer', args: [chatId, sdp]);
  }

  // Phương thức để gửi answer
  Future<void> sendAnswer(String chatId, String sdp) async {
    await _connectionCompleter.future;
    await _hubConnection.invoke('Answer', args: [chatId, sdp]);
  }

  // Phương thức để kết thúc cuộc gọi
  Future<void> endCall(String chatId) async {
    await _connectionCompleter.future;
    await _hubConnection.invoke('EndCall', args: [chatId]);
  }

  // Các phương thức để lắng nghe các sự kiện từ server

  void onIncomingCall(
      void Function(String callerId, String chatId, bool isVideo) handler) {
    _hubConnection.on('IncomingCall', (arguments) {
      if (arguments != null && arguments.length >= 3) {
        handler(arguments[0] as String, arguments[1] as String,
            arguments[2] as bool);
      }
    });
  }

  void onCallAnswered(
      void Function(String answeringUserId, String chatId, bool accept)
          handler) {
    _hubConnection.on('CallAnswered', (arguments) {
      if (arguments != null && arguments.length >= 3) {
        handler(arguments[0] as String, arguments[1] as String,
            arguments[2] as bool);
      }
    });
  }

  void onIceCandidate(void Function(String userId, String candidate) handler) {
    _hubConnection.on('IceCandidate', (arguments) {
      if (arguments != null && arguments.length >= 2) {
        handler(arguments[0] as String, arguments[1] as String);
      }
    });
  }

  void onOffer(void Function(String userId, String sdp) handler) {
    _hubConnection.on('Offer', (arguments) {
      if (arguments != null && arguments.length >= 2) {
        handler(arguments[0] as String, arguments[1] as String);
      }
    });
  }

  void onAnswer(void Function(String userId, String sdp) handler) {
    _hubConnection.on('Answer', (arguments) {
      if (arguments != null && arguments.length >= 2) {
        handler(arguments[0] as String, arguments[1] as String);
      }
    });
  }

  void onCallEnded(void Function(String userId) handler) {
    _hubConnection.on('CallEnded', (arguments) {
      if (arguments != null && arguments.isNotEmpty) {
        handler(arguments[0] as String);
      }
    });
  }
}
