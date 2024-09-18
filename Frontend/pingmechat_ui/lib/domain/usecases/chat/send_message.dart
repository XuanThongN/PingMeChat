import 'package:pingmechat_ui/domain/models/message.dart';
import 'package:pingmechat_ui/domain/repositories/chat_repository.dart';

class SendMessage {
  final ChatRepository repository;

  SendMessage(this.repository);

  Future<void> call(String chatId, String message) async {
    return await repository.sendMessage(chatId, message);
  }
}
