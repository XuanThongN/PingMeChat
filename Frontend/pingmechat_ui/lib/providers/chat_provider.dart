import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:pingmechat_ui/presentation/pages/chat_page.dart';

import '../data/datasources/chat_service.dart';
import '../data/models/chat_model.dart';
import '../domain/models/chat.dart';
import '../domain/models/message.dart';

class ChatProvider extends ChangeNotifier {
  final ChatService _chatService;
  List<Chat> _chats = [];

  final int _itemsPerPage = 20;
  //Các biến dùng cho phân trang
  int _currentPage = 1;
  bool _isLoading = false;
  bool _hasMoreChats = true; // Có còn chats để load hay không

  // Các biến dùng cho phân trang tin nhắn
  Map<String, List<Message>> _messagesByChatId = {};
  Map<String, int> _currentPageByChatId = {};
  Map<String, bool> _hasMoreMessagesByChatId = {};
  Map<String, bool> _isLoadingMessagesByChatId = {};

// Callback để mở ChatPage
  void Function(Chat)? onOpenChatPage;

  // Constructor
  ChatProvider(this._chatService) {
    _initialize();
  }

  // Các getter để lấy dữ liệu từ provider
  List<Chat> get chats => _chats;

  // Các getter để lấy dữ liệu tin nhắn từ provider
  List<Message> getMessagesForChat(String chatId) =>
      _messagesByChatId[chatId] ?? [];
  bool isLoadingMessages(String chatId) =>
      _isLoadingMessagesByChatId[chatId] ?? false;
  bool hasMoreMessages(String chatId) =>
      _hasMoreMessagesByChatId[chatId] ?? true;

  // Các getter để lấy trạng thái của phân trang
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
      // Thêm cuộc trò chuyện mới vào danh sách chats
      _chats.add(chat);
      notifyListeners();
      // Mở ChatPage với cuộc trò chuyện mới
      onOpenChatPage?.call(chat);
    });

    _chatService.chatHubService.onReceiveMessage((message) async {
      // Check if the chat exists in the current list
      final chatIndex = _chats.indexWhere((c) => c.id == message.chatId);

      if (chatIndex == -1) {
        // Chat not found, fetch it from the database
        try {
          final newChat = await _chatService.getChatById(message.chatId);
          if (newChat != null) {
            // Thêm đoạn chat vào đầu danh sách chats
            _chats.insert(0, newChat);
            _messagesByChatId[message.chatId] = [message];
          }
        } catch (error) {
          print('Error fetching chat: $error');
        }
      } else {
        // Chat found, add the message to the existing chat
        _messagesByChatId[message.chatId]?.add(Message(
          chatId: message.chatId,
          senderId: message.senderId,
          content: message.content,
          createdDate: message.createdDate,
        ));

        // Update the last message of the chat
        if (_chats[chatIndex].messages!.isNotEmpty) {
          _chats[chatIndex].messages!.clear();
        }
        _chats[chatIndex].messages!.insert(0, message);

        // Sort chats by the last message date
        _chats.sort((a, b) {
          final lastMessageA = a.messages!.isNotEmpty
              ? a.messages!.first.createdDate
              : a.createdDate;
          final lastMessageB = b.messages!.isNotEmpty
              ? b.messages!.first.createdDate
              : b.createdDate;
          return lastMessageB!.compareTo(lastMessageA!);
        });
      }

      print("Tin nhắn nhận đc từ server: ");
      notifyListeners();
    });
  }

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
      final newChats = await _chatService.getChats(
          page: _currentPage, pageSize: _itemsPerPage);

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
      // _messages.remove(message);
      notifyListeners();
      // Hiển thị thông báo lỗi cho người dùng
    }
  }

  Future<void> startNewChat(ChatCreateDto chatCreateDto) async {
    await _chatService.startNewChat(chatCreateDto);
    // Chat mới sẽ được thêm vào thông qua listeners
  }

  // Hàm get tất cả tin nhắn của một cuộc trò chuyện
  Future<void> loadMessages(String chatId, {bool refresh = false}) async {
    // Nếu refresh = true thì reset lại danh sách tin nhắn
    if (refresh) {
      _messagesByChatId[chatId] = [];
      _currentPageByChatId[chatId] = 1;
      _hasMoreMessagesByChatId[chatId] = true;
    }

    // Nếu đang load tin nhắn hoặc không còn tin nhắn để load thì return
    if (_isLoadingMessagesByChatId[chatId] == true ||
        _hasMoreMessagesByChatId[chatId] == false) return;

    _isLoadingMessagesByChatId[chatId] = true;
    notifyListeners();

    try {
      final currentPage = _currentPageByChatId[chatId] ?? 1;
      final newMessages = await _chatService.getMessages(
          chatId: chatId, page: currentPage, pageSize: _itemsPerPage);

      if (newMessages.isEmpty) {
        _hasMoreMessagesByChatId[chatId] = false;
      } else {
        _messagesByChatId[chatId] = [
          ...newMessages.reversed,
          ...(_messagesByChatId[chatId] ?? [])
        ];
        _currentPageByChatId[chatId] = currentPage + 1;
      }
    } catch (error) {
      print('Error loading messages: $error');
    } finally {
      _isLoadingMessagesByChatId[chatId] = false;
      notifyListeners();
    }
  }

  // Thêm các phương thức khác tương ứng với các chức năng của ChatHub
  bool _shouldOpenChatPage(Chat chat) {
    return chat.isGroup == false && chat.messages!.isEmpty;
  }
}
