import 'dart:convert';
import 'chat_hub_service_interface.dart';
import '../../domain/models/message.dart';
import '../../domain/models/chat.dart';
import '../models/chat_model.dart';
import 'signalr_connection.dart';

class ChatHubService implements ChatHubServiceInterface {
  final SignalRConnection _signalRConnection;

  ChatHubService(this._signalRConnection);

  @override
  Future<void> sendMessage(MessageSendDto message) async {
    await _signalRConnection.invoke('SendMessage', args: [message]);
  }

  @override
  Future<void> startNewChat(ChatCreateDto chatCreateDto) async {
    await _signalRConnection
        .invoke('StartNewChatAsync', args: [chatCreateDto.toJson()]);
  }

  @override
  void onReceiveMessage(void Function(Message message) handler) {
    _signalRConnection.on('ReceiveMessage', (arguments) {
      if (arguments != null && arguments.isNotEmpty) {
        final messageData = arguments[0] as Map<String, dynamic>;
        final message = Message.fromJson(messageData);
        handler(message);
      }
    });
  }

  @override
  void onNewGroupChat(void Function(Chat chat) handler) {
    _signalRConnection.on('NewGroupChat', (arguments) {
      if (arguments != null && arguments.isNotEmpty) {
        handler(Chat.fromJson(arguments[0]));
      }
    });
  }

  @override
  void onNewPrivateChat(void Function(Chat chat) handler) {
    _signalRConnection.on('NewPrivateChat', (arguments) {
      if (arguments != null && arguments.isNotEmpty) {
        handler(Chat.fromJson(arguments[0]));
      }
    });
  }

  @override
  Future<void> initiateCall(String chatId, bool isVideo) async {
    await _signalRConnection.invoke('InitiateCall', args: [chatId, isVideo]);
  }

  @override
  Future<void> answerCall(String chatId, bool accept) async {
    await _signalRConnection.invoke('AnswerCall', args: [chatId, accept]);
  }

  @override
  Future<void> sendIceCandidate(
      String chatId, Map<String, dynamic> candidate) async {
    String candidateString = jsonEncode(candidate);
    await _signalRConnection
        .invoke('IceCandidate', args: [chatId, candidateString]);
  }

  @override
  Future<void> sendOffer(String chatId, String sdp) async {
    await _signalRConnection.invoke('Offer', args: [chatId, sdp]);
  }

  @override
  Future<void> sendAnswer(String chatId, String sdp) async {
    await _signalRConnection.invoke('Answer', args: [chatId, sdp]);
  }

  @override
  Future<void> endCall(String chatId) async {
    await _signalRConnection.invoke('EndCall', args: [chatId]);
  }

  @override
  void onIncomingCall(
      void Function(String callerId, String chatId, bool isVideo) handler) {
    _signalRConnection.on('IncomingCall', (arguments) {
      if (arguments != null && arguments.length >= 3) {
        handler(arguments[0] as String, arguments[1] as String,
            arguments[2] as bool);
      }
    });
  }

  @override
  void onCallAnswered(
      void Function(String answeringUserId, String chatId, bool accept)
          handler) {
    _signalRConnection.on('CallAnswered', (arguments) {
      if (arguments != null && arguments.length >= 3) {
        handler(arguments[0] as String, arguments[1] as String,
            arguments[2] as bool);
      }
    });
  }

  @override
  void onIceCandidate(void Function(String userId, String candidate) handler) {
    _signalRConnection.on('IceCandidate', (arguments) {
      if (arguments != null && arguments.length >= 2) {
        handler(arguments[0] as String, arguments[1] as String);
      }
    });
  }

  @override
  void onOffer(void Function(String userId, String sdp) handler) {
    _signalRConnection.on('Offer', (arguments) {
      if (arguments != null && arguments.length >= 2) {
        handler(arguments[0] as String, arguments[1] as String);
      }
    });
  }

  @override
  void onAnswer(void Function(String userId, String sdp) handler) {
    _signalRConnection.on('Answer', (arguments) {
      if (arguments != null && arguments.length >= 2) {
        handler(arguments[0] as String, arguments[1] as String);
      }
    });
  }

  @override
  void onCallEnded(void Function(String userId) handler) {
    _signalRConnection.on('CallEnded', (arguments) {
      if (arguments != null && arguments.isNotEmpty) {
        handler(arguments[0] as String);
      }
    });
  }
}
