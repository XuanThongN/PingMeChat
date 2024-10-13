import 'dart:io';

import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:pingmechat_ui/domain/models/attachment.dart';
import 'package:uuid/uuid.dart';

import '../data/datasources/chat_service.dart';
import '../data/datasources/file_upload_service.dart';
import '../data/models/chat_model.dart';
import '../domain/models/account.dart';
import '../domain/models/chat.dart';
import '../domain/models/message.dart';
import '../domain/models/userchat.dart';

class ChatProvider extends ChangeNotifier {
  ChatService _chatService;
  ChunkedUploader _fileUploadService;
  final List<Chat> _chats = [];

  final int _itemsPerPage = 20;
  //Các biến dùng cho phân trang
  int _currentPage = 1;
  bool _isLoading = false;
  bool _hasMoreChats = true; // Có còn chats để load hay không

  // Các biến dùng cho phân trang tin nhắn
  final Map<String, List<Message>> _messagesByChatId = {};
  final Map<String, int> _currentPageByChatId = {};
  final Map<String, bool> _hasMoreMessagesByChatId = {};
  final Map<String, bool> _isLoadingMessagesByChatId = {};

// Callback để mở ChatPage
  void Function(Chat)? onOpenChatPage;

  // Biến dùng để lưu trữ userId của người dùng đang gõ tin nhắn
  Map<String, String> _typingUserIds = {};

  // Constructor
  ChatProvider(this._chatService, this._fileUploadService) {
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

  get typingUserId => null;

// Phương thức cập nhật ChatService nếu cần thay đổi
  void updateChatService(ChatService newChatService) {
    _chatService = newChatService;
    // _setupSignalRListeners(); // Cập nhật lại các listeners nếu có
  }

  Future<void> _initialize() async {
    // _setupSignalRListeners();
    _setupCallbacks();
    await loadChats(); //Load tất cả đoạn chat từ server
  }

  void _setupCallbacks() {
    _chatService.onNewGroupChatCallback = _handleNewGroupChat;
    _chatService.onNewPrivateChatCallback = _handleNewPrivateChat;
    _chatService.onReceiveMessageCallback = _handleNewMessage;
    _chatService.onUserTypingCallback = _handleUserTyping;
    _chatService.onUserStopTypingCallback = _handleUserStopTyping;
    _chatService.onSentMessageCallback = _handleSentMessage;
    _chatService.onMarkMessageAsReadCallback = _handleMarkMessageAsRead;
  }

  void _handleNewGroupChat(Chat chat) {
    _chats.insert(0, chat);
    notifyListeners();

    // Chỉ mở trang chat nếu người dùng hiện tại là người tạo nhóm
    if (chat.createdBy == _chatService.getCurrentUserId()) {
      onOpenChatPage?.call(chat);
    }
  }

  void _handleNewPrivateChat(Chat chat) {
    // Thêm cuộc trò chuyện mới vào danh sách chats
    _chats.insert(0, chat);
    notifyListeners();
    // Mở ChatPage với cuộc trò chuyện mới
    onOpenChatPage?.call(chat);
  }

  void _handleNewMessage(Message message) async {
    // Check if the chat exists in the current list
    final chatIndex = _chats.indexWhere((c) => c.id == message.chatId);

    if (chatIndex == -1) {
      // Chat not found, fetch it from the database
      try {
        final newChat = await _chatService.getChatById(message.chatId);
        _chats.insert(0, newChat);

        // Find the sender in the chat's userChats
        final sender = newChat.userChats
                .firstWhere(
                  (userChat) => userChat.userId == message.senderId,
                  orElse: () => UserChat(
                      userId: message.senderId,
                      chatId: message.chatId,
                      isAdmin: false,
                      id: ''),
                )
                .user ??
            Account(id: message.senderId, email: '', fullName: '');

        message.sender = sender;
        _messagesByChatId[message.chatId]?.add(message);
      } catch (error) {
        print('Error fetching chat: $error');
      }
    } else {
      // Chat found, find the sender in the existing chat's userChats
      final sender = _chats[chatIndex]
              .userChats
              .firstWhere(
                (userChat) => userChat.userId == message.senderId,
                orElse: () => UserChat(
                    userId: message.senderId,
                    chatId: message.chatId,
                    isAdmin: false,
                    id: ''),
              )
              .user ??
          Account(id: message.senderId, email: '', fullName: '');

      final new_message = Message(
        chatId: message.chatId,
        senderId: message.senderId,
        content: message.content,
        createdDate: message.createdDate,
        attachments: message.attachments,
        sender: sender,
      );
      // Add the message to the existing chat
      _messagesByChatId[message.chatId]?.add(new_message);

      // Update the last message of the chat
      if (_chats[chatIndex].messages!.isNotEmpty) {
        _chats[chatIndex].messages!.clear();
      }
      _chats[chatIndex].messages!.insert(0, new_message);

      // Sắp xếp lại tất cả các đoạn chat theo thời gian tin nhắn cuối cùng nhận được
      sortChats();
    }

    print("Tin nhắn nhận được từ server: ${message.content}");
    notifyListeners();
  }

// Hàm xử lý tin nhắn đã được gửi thành công
  void _handleSentMessage(String tempId, Message sentMessage) {
    final chatId = sentMessage.chatId;
    final index = _messagesByChatId[chatId]?.indexWhere((m) => m.id == tempId);
    print("Tin nhắn đã được gửi thành công: $tempId");
    if (index != null && index != -1) {
      _messagesByChatId[chatId]![index] = sentMessage;
      notifyListeners();
    }
  }

// Hàm xử lý tin nhắn đã được đánh dấu là đã đọc
  void _handleMarkMessageAsRead(String chatId, MessageReader messageReader) {
    final chat = _chats.firstWhere((c) => c.id == chatId);
    if (chat.messages != null) {
      final message =
          chat.messages!.firstWhere((m) => m.id == messageReader.messageId);
      if (message.messageReaders != null) {
        message.messageReaders!.add(messageReader);
      }
      notifyListeners();
    }
  }

  void _handleUserTyping(String chatId, String userId) {
    _typingUserIds[chatId] = userId;
    notifyListeners();
  }

  void _handleUserStopTyping(String chatId, String userId) {
    _typingUserIds.remove(chatId);
    notifyListeners();
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

  Future<void> sendMessage({
    required String chatId,
    required String message,
    List<File> files = const [],
  }) async {
    final tempId = Uuid().v4();
    final currentUserId = _chatService.getCurrentUserId();

    List<Attachment> tempAttachments = [];
    if (files.isNotEmpty) {
      tempAttachments = files
          .map((file) => Attachment(
                fileName: file.path.split('/').last,
                fileUrl:
                    'file://${file.path}', // Tạo đường dẫn file cho file local
                fileType: _getFileType(file),
                fileSize: file.lengthSync(),
                isUploading: true, // Đánh dấu là đang upload
              ))
          .toList();
    }

    // Tạo tin nhắn tạm thời
    final tempMessage = Message(
      id: tempId,
      chatId: chatId,
      senderId: currentUserId,
      content: message,
      createdDate: DateTime.now(),
      status: MessageStatus.sending,
      attachments: tempAttachments,
    );

    // Thêm tin nhắn tạm thời vào danh sách tin nhắn của chat
    _addTempMessageToChat(chatId, tempMessage);

    try {
      // Upload file lên server
      List<UploadResult> uploadedAttachments = [];
      if (files.isNotEmpty) {
        uploadedAttachments = await _fileUploadService.uploadFiles(files);
      }

      if (uploadedAttachments.isEmpty && message.isEmpty) {
        throw Exception('Message and attachments cannot be empty');
      }

      // Gửi tin nhắn lên server
      MessageSendDto messageDto = MessageSendDto(
        tempId: tempId,
        chatId: chatId,
        content: message,
        attachments: uploadedAttachments
            .map((e) => Attachment(
                  fileName: e.publicId,
                  fileUrl: e.url,
                  fileType: e.fileType,
                  fileSize: e.fileSize,
                ))
            .toList(),
      );

      await _chatService.sendMessage(messageDto);
    } catch (error) {
      print('Error sending message: $error');
      _updateMessageStatus(chatId, tempId, MessageStatus.failed);
    }
  }

  // Hàm mark message as read
  Future<void> markMessageAsRead(String chatId, String messageId) async {
    // Kiểm tra xem người dùng hiện tại có phải là người đọc tin nhắn không
    // final currentUserId = _chatService.getCurrentUserId();
    // final messages = _messagesByChatId[chatId];
    // final message = messages?.firstWhere((m) => m.id == messageId);
    // if (message != null &&
    //     message.messageReaders!.any((r) => r.readerId == currentUserId)) {
    //   return;
    // }
    // print('Marking message as read: $messageId');
    await _chatService.markMessageAsRead(chatId, messageId);
  }

  void _addTempMessageToChat(String chatId, Message tempMessage) {
    _messagesByChatId[chatId]?.add(tempMessage);
    final chat = _chats.firstWhere((c) => c.id == chatId);
    chat.messages?.clear();
    chat.messages?.insert(0, tempMessage);
    sortChats();
    notifyListeners();
  }

  void _updateMessageStatus(
      String chatId, String messageId, MessageStatus status) {
    final index =
        _messagesByChatId[chatId]?.indexWhere((m) => m.id == messageId);
    if (index != null && index != -1) {
      _messagesByChatId[chatId]![index] =
          _messagesByChatId[chatId]![index].copyWith(
        status: status,
      );
      notifyListeners();
    }
  }

  String _getFileType(File file) {
    final extension = file.path.split('.').last.toLowerCase();
    if (['jpg', 'jpeg', 'png', 'gif'].contains(extension)) {
      return 'Image';
    } else if (['mp4', 'mov', 'avi'].contains(extension)) {
      return 'Video';
    } else if (['mp3', 'wav', 'ogg'].contains(extension)) {
      return 'Audio';
    } else {
      return 'File';
    }
  }

  // Hàm get file type từ string
  String _getFileTypeFromStr(String fileType) {
    if (fileType.contains('image')) {
      return 'Image';
    } else if (fileType.contains('video')) {
      return 'Video';
    } else if (fileType.contains('audio')) {
      return 'Audio';
    } else {
      return 'Document';
    }
  }

  void _updateAttachments(
      String chatId, String messageId, List<UploadResult> uploadedAttachments) {
    final index =
        _messagesByChatId[chatId]?.indexWhere((m) => m.id == messageId);
    if (index != null && index != -1) {
      final updatedAttachments = uploadedAttachments
          .map((e) => Attachment(
                fileName: e.publicId,
                fileUrl: e.url,
                fileType: _getFileTypeFromStr(e.fileType),
                fileSize: e.fileSize,
                isUploading: false,
              ))
          .toList();
      _messagesByChatId[chatId]![index] =
          _messagesByChatId[chatId]![index].copyWith(
        attachments: updatedAttachments,
      );
      notifyListeners();
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

  // Hàm set trạng thái gõ tin nhắn
  Future<void> sendTypingStatus(String chatId, bool isTyping) async {
    try {
      if (isTyping) {
        await _chatService.sendUserTyping(chatId);
      } else {
        await _chatService.sendUserStopTyping(chatId);
      }
    } catch (error) {
      print('Error sending typing status: $error');
    }
  }

  bool isUserTyping(String chatId) => _typingUserIds.containsKey(chatId);
  String? getTypingUser(String chatId) => _typingUserIds[chatId];
  void setTypingStatus(String chatId, String userId, bool isTyping) {
    if (isTyping) {
      _typingUserIds[chatId] = userId;
    } else {
      _typingUserIds.remove(chatId);
    }
    notifyListeners();
  }

// Hàm thêm thành viên vào cuộc trò chuyện
  Future<bool> addMembersToCurrentChat(
      String chatId, List<String> selectedMembers) async {
    // Thêm thành viên vào danh sách userChats của chat hiện tại
    var result = await _chatService.addMembersToChat(chatId, selectedMembers);
    if (result.isNotEmpty) {
      // Cập nhật lại danh sách thành viên của chat hiện tại
      final chatIndex = _chats.indexWhere((c) => c.id == chatId && c.isGroup);
      if (chatIndex != -1) {
        final members = _chats[chatIndex].userChats;
        members.addAll(result);
        notifyListeners();
      }
    }
    return result.isNotEmpty;
  }

  // Hàm xóa thành viên khỏi cuộc trò chuyện
  Future<bool> removeMemberFromCurrentChat(String chatId, String userId) async {
    final result = await _chatService.removeMemberFromChat(chatId, userId);
    if (result) {
      final chatIndex = _chats.indexWhere((c) => c.id == chatId && c.isGroup);
      if (chatIndex != -1) {
        final members = _chats[chatIndex].userChats;
        members.removeWhere((member) => member.userId == userId);
        notifyListeners();
      }
    }
    return result;
  }

  // Clear all chats and reset state
  void clearData() {
    _chats.clear();
    _messagesByChatId.clear();
    _currentPageByChatId.clear();
    _hasMoreMessagesByChatId.clear();
    _isLoadingMessagesByChatId.clear();
    _typingUserIds.clear();
    _currentPage = 1;
    _isLoading = false;
    _hasMoreChats = true;
    notifyListeners();
  }

  // Hàm sort danh sách chat
  void sortChats() {
    // Sắp xếp lại tất cả các đoạn chat theo thời gian tin nhắn cuối cùng nhận được
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
}
