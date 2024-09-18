import 'package:http/http.dart' as http;
import '../../../domain/models/chat.dart';
import '../../models/chat_model.dart';
import 'chat_hub_service.dart';

class ChatRemoteDataSource {
  final http.Client client;
  final String baseUrl;
  final ChatHubService chatHubService;

  ChatRemoteDataSource({
    required this.client, 
    required this.baseUrl,
    required this.chatHubService
  });

  // ... các phương thức khác ...

  Future<void> sendMessage(String chatId, String message) async {
    await chatHubService.sendMessage(chatId, message);
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



  // ... thêm các phương thức khác tương ứng với các chức năng SignalR ...
}
