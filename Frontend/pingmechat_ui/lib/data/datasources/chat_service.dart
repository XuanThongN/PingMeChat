import 'dart:convert';

import '../../domain/models/chat.dart';
import '../../domain/models/message.dart';
import '../../domain/repositories/chat_repository.dart';
import '../models/api_response_model.dart';
import '../models/chat_model.dart';
import 'chat_hub_service.dart';
import 'package:http/http.dart' as http;

class ChatService {
  final ChatRepository chatRepository;
  final ChatHubService chatHubService;
  final String _baseUrl = 'https://4jnvrgvp-7043.asse.devtunnels.ms/api';
  String accessToken =
      'eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJzdXBlcmFkbWluQGdtYWlsLmNvbSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJzdXBlcmFkbWluIiwiVXNlcklkIjoiY2MxZTY2M2EtMTdiNy00NzBiLWFmZDUtYmM0NzUyYzgwZGQxIiwiRW1haWwiOiJzdXBlcmFkbWluQGdtYWlsLmNvbSIsImp0aSI6IjBmYjBiOTMwLWQ0YjktNGE5ZC1hMTRiLWQyYTVlMDJkMjlmOCIsImV4cCI6MTcyNjUzMjY5NCwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MDMwIiwiYXVkIjoiVXNlciJ9.4mEXsimCtuAB8PSyOjRKHGCGXXh1A29CR8A5OXFuZzYKcpE5mYDmCIR-2xFrv6urFoK0i5uC22xuRM3e38K3nQ';
  String refreshToken =
      'pwkvVjtwWqH10PhHZD3YjMwEZBUVkejOd2x2qIhgA1+OdBC3Th9t+SbinZLss5iQg5RxVTD4iZZFLto5NJ0U+g==';

  ChatService({required this.chatRepository, required this.chatHubService});

// Hàm lấy danh sách các cuộc trò chuyện
  Future<List<Chat>> getChats({int page = 1, int pageSize = 20}) async {
  try {
    final response = await http.get(
      Uri.parse('$_baseUrl/chats/get-chat-list?pageNumber=$page&pageSize=$pageSize'),
      headers: {
        'Authorization': 'Bearer $accessToken',
        'RefreshToken': refreshToken,
      },
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
      Uri.parse('$_baseUrl/messages/$chatId?pageNumber=$page&pageSize=$pageSize'),
      headers: {
        'Authorization': 'Bearer $accessToken',
        'RefreshToken': refreshToken,
      },
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
