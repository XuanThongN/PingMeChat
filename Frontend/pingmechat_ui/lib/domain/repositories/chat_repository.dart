import 'package:pingmechat_ui/domain/models/message.dart';

import '../models/chat.dart';

abstract class ChatRepository {
  Future<void> sendMessage(String chatId, String message);
  // Future<List<Message>> getMessages();
  // Thêm hàm get chat list
  // Future<List<Chat>> getChatList();
}
