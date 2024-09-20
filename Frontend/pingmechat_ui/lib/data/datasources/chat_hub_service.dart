import 'package:flutter/material.dart';
import 'package:pingmechat_ui/data/models/chat_model.dart';
import 'package:pingmechat_ui/domain/models/message.dart';
import 'package:provider/provider.dart';
import 'package:signalr_core/signalr_core.dart';

import '../../domain/models/chat.dart';
import '../../providers/auth_provider.dart';
import '../models/message_model.dart';
import 'constant.dart';

class ChatHubService {
  late HubConnection _hubConnection;
  final AuthProvider authProvider;

  ChatHubService(this.authProvider);

  Future<void> connect() async {
     _hubConnection = HubConnectionBuilder()
        .withUrl(
          ChatHubConstants.chatHubUrl, // URL of the SignalR hub
          HttpConnectionOptions(
            accessTokenFactory: () async {
              return await authProvider.getAuthorizationString();
            },
            transport: HttpTransportType.webSockets,
            customHeaders: await authProvider.getCustomHeaders(),
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
