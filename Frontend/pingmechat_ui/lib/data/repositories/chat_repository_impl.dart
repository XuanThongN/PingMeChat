import 'package:pingmechat_ui/domain/models/chat.dart';
import 'package:pingmechat_ui/domain/models/message.dart';
import 'package:pingmechat_ui/domain/usecases/chat/get_messages.dart';

import '../../domain/repositories/chat_repository.dart';
import '../datasources/remote/chat_remote_datasource.dart';

class ChatRepositoryImpl implements ChatRepository {
  final ChatRemoteDataSource remoteDataSource;

  ChatRepositoryImpl(this.remoteDataSource);

  // @override
  // Future<List<Message>> getMessages() async {
  //   try {
  //     return await remoteDataSource.getMessages();
  //   } catch (e) {
  //     throw Exception('Failed to get messages');
  //   }
  // }

  @override
  Future<void> sendMessage(String content, String message) async {
    try {
      await remoteDataSource.sendMessage(content, message);
    } catch (e) {
      throw Exception('Failed to send message');
    }
  }

  // @override
  // Future<List<Chat>> getChatList() async {
  //   try {
  //     return await remoteDataSource.getChatList();
  //   } catch (e) {
  //     throw Exception('Failed to get chat list');
  //   }
  // }
}
