import 'package:pingmechat_ui/domain/models/message.dart';
import 'package:pingmechat_ui/domain/repositories/chat_repository.dart';

class GetMessages {
  final ChatRepository repository;

  GetMessages(this.repository);

  // Future<List<Message>> call() async {
  //   return await repository.getMessages();
  // }
}
