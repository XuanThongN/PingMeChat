import 'package:pingmechat_ui/domain/models/account.dart';
import 'package:pingmechat_ui/domain/models/chat.dart';
import 'package:pingmechat_ui/domain/models/attachment.dart';

class Message {
  final String chatId;
  final String senderId;
  final String? content;
  final DateTime sentAt;
  final List<Attachment>? attachments;
  final List<MessageReader>? messageReaders;
  final Chat chat;
  final Account sender;

  Message({
    required this.chatId,
    required this.senderId,
    this.content,
    required this.sentAt,
    this.attachments,
    this.messageReaders,
    required this.chat,
    required this.sender,
  });

  factory Message.fromJson(Map<String, dynamic> json) {
    return Message(
      chatId: json['chatId'],
      senderId: json['senderId'],
      content: json['content'],
      sentAt: DateTime.parse(json['sentAt']),
      attachments: (json['attachments'] as List?)
          ?.map((i) => Attachment.fromJson(i))
          .toList(),
      messageReaders: (json['messageReaders'] as List?)
          ?.map((i) => MessageReader.fromJson(i))
          .toList(),
      chat: Chat.fromJson(json['chat']),
      sender: Account.fromJson(json['sender']),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'chatId': chatId,
      'senderId': senderId,
      'content': content,
      'sentAt': sentAt.toIso8601String(),
      'attachments': attachments?.map((i) => i.toJson()).toList(),
      'messageReaders': messageReaders?.map((i) => i.toJson()).toList(),
      'chat': chat.toJson(),
      'sender': sender.toJson(),
    };
  }
}

class MessageReader {
  final String messageId;
  final String readerId;
  final DateTime readAt;

  MessageReader({
    required this.messageId,
    required this.readerId,
    required this.readAt,
  });

  factory MessageReader.fromJson(Map<String, dynamic> json) {
    return MessageReader(
      messageId: json['messageId'],
      readerId: json['readerId'],
      readAt: DateTime.parse(json['readAt']),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'messageId': messageId,
      'readerId': readerId,
      'readAt': readAt.toIso8601String(),
    };
  }
}