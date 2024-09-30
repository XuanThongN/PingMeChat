import '../../domain/models/message.dart';
import '../../domain/models/chat.dart';
import '../models/chat_model.dart';

abstract class ChatHubServiceInterface {
  Future<void> sendMessage(MessageSendDto message);
  Future<void> startNewChat(ChatCreateDto chatCreateDto);
  void onReceiveMessage(void Function(Message message) handler);
  void onNewGroupChat(void Function(Chat chat) handler);
  void onNewPrivateChat(void Function(Chat chat) handler);
  Future<void> initiateCall(String chatId, bool isVideo);
  Future<void> answerCall(String chatId, bool accept);
  Future<void> sendIceCandidate(String chatId, Map<String, dynamic> candidate);
  Future<void> sendOffer(String chatId, String sdp);
  Future<void> sendAnswer(String chatId, String sdp);
  Future<void> endCall(String chatId);
  void onIncomingCall(void Function(String callerId, String chatId, bool isVideo) handler);
  void onCallAnswered(void Function(String answeringUserId, String chatId, bool accept) handler);
  void onIceCandidate(void Function(String userId, String candidate) handler);
  void onOffer(void Function(String userId, String sdp) handler);
  void onAnswer(void Function(String userId, String sdp) handler);
  void onCallEnded(void Function(String userId) handler);
}