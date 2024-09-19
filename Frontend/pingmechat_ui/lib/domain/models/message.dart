import 'package:pingmechat_ui/domain/models/account.dart';
import 'package:pingmechat_ui/domain/models/chat.dart';
import 'package:pingmechat_ui/domain/models/attachment.dart';

class Message {
  final String chatId;
  final String senderId;
  final String? content;
  final DateTime createdDate;
  final List<Attachment>? attachments;
  final List<MessageReader>? messageReaders;
  final Chat? chat;
  final Account? sender;

  Message({
    required this.chatId,
    required this.senderId,
    this.content,
    required this.createdDate,
    this.attachments,
    this.messageReaders,
    this.chat,
    this.sender,
  });

  factory Message.fromJson(Map<String, dynamic> json) {
    return Message(
      chatId: json['chatId'],
      senderId: json['senderId'],
      content: json['content'],
      createdDate: DateTime.parse(json['createdDate']),
      attachments: (json['attachments'] as List?)
          ?.map((i) => Attachment.fromJson(i))
          .toList(),
      messageReaders: (json['messageReaders'] as List?)
          ?.map((i) => MessageReader.fromJson(i))
          .toList(),
      // sender: Account.fromJson(json['sender']) as Account? ??
      //     null // Dùng as Account? để ép kiểu về Account hoặc null
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'chatId': chatId,
      'senderId': senderId,
      'content': content,
      'createdDate': createdDate.toIso8601String(),
      'attachments': attachments?.map((i) => i.toJson()).toList(),
      'messageReaders': messageReaders?.map((i) => i.toJson()).toList(),
      'sender': sender!.toJson(),
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
