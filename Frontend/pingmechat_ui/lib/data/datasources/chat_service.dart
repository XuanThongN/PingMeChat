import '../../domain/models/chat.dart';
import '../../domain/repositories/chat_repository.dart';
import '../models/chat_model.dart';
import 'chat_hub_service.dart';

class ChatService {
  final ChatRepository chatRepository;
  final ChatHubService chatHubService;

  ChatService({required this.chatRepository, required this.chatHubService});

  // Future<List<Chat>> getChatList() async {
  //   return await chatRepository.getChatList();
  // }
  Future<void> initialize() async {
    await chatHubService.connect();
  }

  Future<void> sendMessage(String chatId, String message) async {
    await chatRepository.sendMessage(chatId, message);
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

  // Thêm các phương thức khác tương ứng với các chức năng của ChatHub
}
