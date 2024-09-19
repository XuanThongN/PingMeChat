import 'package:pingmechat_ui/data/models/chat_model.dart';
import 'package:pingmechat_ui/domain/models/message.dart';
import 'package:signalr_core/signalr_core.dart';

import '../../domain/models/chat.dart';
import '../models/message_model.dart';

class ChatHubService {
  late HubConnection _hubConnection;
  String accessToken =
      'eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJzdXBlcmFkbWluQGdtYWlsLmNvbSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJzdXBlcmFkbWluIiwiVXNlcklkIjoiY2MxZTY2M2EtMTdiNy00NzBiLWFmZDUtYmM0NzUyYzgwZGQxIiwiRW1haWwiOiJzdXBlcmFkbWluQGdtYWlsLmNvbSIsImp0aSI6IjBmYjBiOTMwLWQ0YjktNGE5ZC1hMTRiLWQyYTVlMDJkMjlmOCIsImV4cCI6MTcyNjUzMjY5NCwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MDMwIiwiYXVkIjoiVXNlciJ9.4mEXsimCtuAB8PSyOjRKHGCGXXh1A29CR8A5OXFuZzYKcpE5mYDmCIR-2xFrv6urFoK0i5uC22xuRM3e38K3nQ';
  String refreshToken =
      'pwkvVjtwWqH10PhHZD3YjMwEZBUVkejOd2x2qIhgA1+OdBC3Th9t+SbinZLss5iQg5RxVTD4iZZFLto5NJ0U+g==';

  Future<void> connect() async {
    _hubConnection = HubConnectionBuilder()
        .withUrl(
          'https://4jnvrgvp-7043.asse.devtunnels.ms/chatHub', // Use HTTPS for negotiation
          HttpConnectionOptions(
            accessTokenFactory: () async {
              return 'Bearer ${accessToken},${refreshToken}';
            },
            transport: HttpTransportType.webSockets,
            customHeaders: {
              'Authorization': 'Bearer $accessToken',
              'RefreshToken': refreshToken,
            },
            logging: (level, message) => print('SignalR: $level - $message'),
          ),
        )
        .build();

    _hubConnection.onclose((error) async {
      print('Connection closed: $error');
      await reconnect();
    });

    try {
      await _hubConnection.start();
      print('Connected to SignalR hub');
    } catch (e) {
      if (e.toString().contains('401')) {
        print('Authentication failed: Invalid access token');
        // Optionally, handle token refresh logic here
      } else {
        print('Error connecting to SignalR hub: $e');
        await reconnect();
      }
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
        await Future.delayed(Duration(seconds: 2));
      }
    }
    print('Failed to reconnect after $maxAttempts attempts');
  }

  // void onReceiveMessage(void Function(MessageDto message) handler) {
  //   _hubConnection.on('ReceiveMessage', (arguments) {
  //     print('Received data: $arguments');
  //     if (arguments == null || arguments.isEmpty) return;
  //     final messageData = arguments![0] as Map<String, dynamic>;
  //     final messageDto = MessageDto.fromJson(messageData);
  //     handler(messageDto);
  //   });
  // }
  
  void onReceiveMessage(void Function(Message message) handler) {
    _hubConnection.on('ReceiveMessage', (arguments) {
      print('Received data: $arguments');
      if (arguments == null || arguments.isEmpty) return;
      final messageData = arguments![0] as Map<String, dynamic>;
      final messsage = Message.fromJson(messageData);
      handler(messsage);
    });
  }

  Future<void> sendMessage(String chatId, String message) async {
    await _hubConnection.invoke('SendMessage', args: [chatId, message]);
  }

  Future<void> startNewChat(ChatCreateDto chatCreateDto) async {
    await _hubConnection
        .invoke('StartNewChatAsync', args: [chatCreateDto.toJson()]);
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
    await _hubConnection.invoke('JoinChat', args: [chatId]);
  }

  Future<void> sendMessageToGroup(String chatId, String message) async {
    await _hubConnection.invoke('SendMessageToGroup', args: [chatId, message]);
  }

  Future<void> userIsTyping(String chatId) async {
    await _hubConnection.invoke('UserIsTyping', args: [chatId]);
  }

  Future<void> userStoppedTyping(String chatId) async {
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

  // Call audio/video methods
  Future<void> initiateCall(String targetUserId, bool isVideo) async {
    await _hubConnection.invoke('InitiateCall', args: [targetUserId, isVideo]);
  }

  Future<void> answerCall(String callerUserId, bool accept) async {
    await _hubConnection.invoke('AnswerCall', args: [callerUserId, accept]);
  }

  Future<void> sendIceCandidate(String targetUserId, String candidate) async {
    await _hubConnection
        .invoke('IceCandidate', args: [targetUserId, candidate]);
  }

  Future<void> sendOffer(String targetUserId, String sdp) async {
    await _hubConnection.invoke('Offer', args: [targetUserId, sdp]);
  }

  Future<void> sendAnswer(String targetUserId, String sdp) async {
    await _hubConnection.invoke('Answer', args: [targetUserId, sdp]);
  }

  Future<void> endCall(String targetUserId) async {
    await _hubConnection.invoke('EndCall', args: [targetUserId]);
  }

  void onIncomingCall(
      void Function(String callerUserId, bool isVideo) handler) {
    _hubConnection.on('IncomingCall', (arguments) {
      if (arguments != null && arguments.length >= 2) {
        handler(arguments[0] as String, arguments[1] as bool);
      }
    });
  }

  void onCallAnswered(void Function(String targetUserId, bool accept) handler) {
    _hubConnection.on('CallAnswered', (arguments) {
      if (arguments != null && arguments.length >= 2) {
        handler(arguments[0] as String, arguments[1] as bool);
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
