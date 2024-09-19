import 'dart:convert';

import 'package:flutter/foundation.dart';

import '../data/datasources/chat_service.dart';
import '../data/models/chat_model.dart';
import '../domain/models/account.dart';
import '../domain/models/chat.dart';
import '../domain/models/message.dart';

class ChatProvider extends ChangeNotifier {
  final ChatService _chatService;
  List<Chat> _chats = [];
  List<Message> _messages = [];

  //Các biến dùng cho phân trang
  int _currentPage = 1;
  final int _itemsPerPage = 20;
  bool _isLoading = false;
  bool _hasMoreChats = true; // Có còn chats để load hay không

  ChatProvider(this._chatService) {
    _initialize();
  }

  List<Chat> get chats => _chats;
  List<Message> get messages => _messages;
  bool get isLoading => _isLoading;
  bool get hasMoreChats => _hasMoreChats;

  Future<void> _initialize() async {
    await _chatService.initialize();
    _setupSignalRListeners();
    await loadChats(); //Load tất cả đoạn chat từ server
  }

  void _setupSignalRListeners() {
    _chatService.onNewGroupChat((chat) {
      _chats.add(chat);
      notifyListeners();
    });

    _chatService.onNewPrivateChat((chat) {
      _chats.add(chat);
      notifyListeners();
    });

    _chatService.chatHubService.onReceiveMessage((message) {

      messages.add(Message(
        chatId: message.chatId,
        senderId: message.senderId,
        content: message.content,
        createdDate: DateTime.now(),
        chat: Chat(
          id: '1',
          name: 'Chat 1',
          isGroup: false,
          userChats: [],
          messages: [],
        ),
        sender: Account(
          id: '1',
          fullName: 'User 1',
          phoneNumber: 'user1',
          email: '',
        ),
      ));
      // Assume message is a JSON string that can be converted to a Message object
      print("Tin nhắn nhận đc từ server: ");
      // _messages.add(Message.fromJson(jsonDecode(message)));
      notifyListeners();
    });

    _chatService.onNewGroupChat((chat) {
      _chats.add(chat);
      notifyListeners();
    });

    _chatService.onNewPrivateChat((chat) {
      _chats.add(chat);
      notifyListeners();
    });
  }

  // Future<void> loadChats() async {
  //   _chats = await _chatService.getChats();
  //   notifyListeners();
  // }

   Future<void> loadChats({bool refresh = false}) async {
    if (refresh) {
      _currentPage = 1;
      _chats.clear();
      _hasMoreChats = true;
    }

    if (_isLoading || !_hasMoreChats) return;

    _isLoading = true;
    notifyListeners();

    try {
      // Gọi API để lấy chats với _currentPage và _itemsPerPage
      final newChats = await _chatService.getChats(page: _currentPage, pageSize: _itemsPerPage);
      
      if (newChats.isEmpty) {
        _hasMoreChats = false;
      } else {
        _chats.addAll(newChats);
        _currentPage++;
      }
    } catch (error) {
      print('Error loading chats: $error');
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

  Future<void> sendMessage(String chatId, String message) async {
    // Thêm tin nhắn vào danh sách local
    // _messages.add(Message(
    //   chatId: chatId,
    //   senderId: '1',
    //   content: message,
    //   sentAt: DateTime.now(),
    //   chat: Chat(
    //     name: 'Chat 1',
    //     isGroup: false,
    //     userChats: [],
    //     messages: [],
    //   ),
    //   sender: Account(
    //     fullName: 'User 1',
    //     phoneNumber: 'user1',
    //     email: 'acb@gmail.com',
    //   ),
    // ));

    // notifyListeners(); // Thông báo cho UI để cập nhật giao diện

    try {
      await _chatService.sendMessage(chatId, message);

      // Cập nhật lại danh sách tin nhắn nếu cần
      notifyListeners();
    } catch (error) {
      // Xử lý lỗi khi gửi tin nhắn
      print('Error sending message: $error');
      // Có thể xóa tin nhắn khỏi danh sách local nếu gửi thất bại
      _messages.remove(message);
      notifyListeners();
      // Hiển thị thông báo lỗi cho người dùng
    }
  }

  Future<void> startNewChat(ChatCreateDto chatCreateDto) async {
    await _chatService.startNewChat(chatCreateDto);
    // Chat mới sẽ được thêm vào thông qua listeners
  }

  // Thêm các phương thức khác tương ứng với các chức năng của ChatHub
}
