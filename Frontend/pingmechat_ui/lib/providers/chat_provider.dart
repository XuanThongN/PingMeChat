import 'package:flutter/foundation.dart';

import '../data/datasources/remote/chat_service.dart';
import '../data/models/chat_model.dart';
import '../domain/models/chat.dart';
import '../domain/models/message.dart';

class ChatProvider extends ChangeNotifier {
  final ChatService _chatService;
  List<Chat> _chats = [];
  List<Message> _messages = [];

  ChatProvider(this._chatService) {
    _initListeners();
  }

  List<Chat> get chats => _chats;
  List<Message> get messages => _messages;

  void _initListeners() {
    _chatService.onNewGroupChat((chat) {
      _chats.add(chat);
      notifyListeners();
    });

    _chatService.onNewPrivateChat((chat) {
      _chats.add(chat);
      notifyListeners();
    });

    // Thêm các listeners khác
  }

  // Future<void> loadChats() async {
  //   _chats = await _chatService.getChatList();
  //   notifyListeners();
  // }

  Future<void> sendMessage(String chatId, String message) async {
    await _chatService.sendMessage(chatId, message);
    // Có thể cập nhật local state nếu cần
    notifyListeners();
  }

  Future<void> startNewChat(ChatCreateDto chatCreateDto) async {
    await _chatService.startNewChat(chatCreateDto);
    // Chat mới sẽ được thêm vào thông qua listeners
  }

  // Thêm các phương thức khác tương ứng với các chức năng của ChatHub
}
