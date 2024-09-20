import 'dart:convert';

import 'package:pingmechat_ui/providers/auth_provider.dart';

import '../../domain/models/chat.dart';
import '../../domain/models/message.dart';
import '../models/chat_model.dart';
import 'chat_hub_service.dart';
import 'package:http/http.dart' as http;
import 'constant.dart';

class ChatService {
  final ChatHubService chatHubService;
  final AuthProvider authProvider;
  
  ChatService({required this.chatHubService,required this.authProvider});

// Hàm lấy danh sách các cuộc trò chuyện
  Future<List<Chat>> getChats({int page = 1, int pageSize = 20}) async {
  try {
    final response = await http.get(
      Uri.parse('${ApiConstants.baseApiUrl}/chats/get-chat-list?pageNumber=$page&pageSize=$pageSize'),
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
      throw Exception('HTTP error ${response.statusCode}: ${response.reasonPhrase}');
    }
  } catch (e) {
    throw Exception('Failed to load chats: $e');
  }
}

// Hàm lấy danh sách tin nhắn của một cuộc trò chuyện
Future<List<Message>> getMessages({required String chatId, int page = 1, int pageSize = 20}) async {
  try {
    final response = await http.get(
      Uri.parse('${ApiConstants.baseApiUrl}/messages/$chatId?pageNumber=$page&pageSize=$pageSize'),
      headers: await authProvider.getCustomHeaders(),

    );

    if (response.statusCode == 200) {
      final jsonResponse = json.decode(response.body);
      
      // Lấy phần 'result' và truy cập vào 'data' trong đó
      final result = jsonResponse['result'];
      final List<dynamic> data = result['data'];

      // Chuyển đổi từng phần tử trong 'data' thành đối tượng 'Message'
      List<Message> messageList = data.map((json) => Message.fromJson(json)).toList();

      return messageList;
    } else {
      throw Exception('HTTP error ${response.statusCode}: ${response.reasonPhrase}');
    }
  } catch (e) {
    throw Exception('Failed to load messages: $e');
  }
}

// Các phương thức ChatHubService sẽ được gọi từ ChatService
  Future<void> initialize() async {
    await chatHubService.connect();
  }

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

  // Thêm các phương thức khác tương ứng với các chức năng của ChatHub
}
