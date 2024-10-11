import 'package:pingmechat_ui/domain/models/account.dart';
import 'package:pingmechat_ui/domain/models/chat.dart';
import 'package:pingmechat_ui/domain/models/attachment.dart';

class Message {
  String? id;
  final String chatId;
  final String senderId;
  final String? content;
  final DateTime createdDate;
  List<Attachment>? attachments;
  final List<MessageReader>? messageReaders;
  final Chat? chat;
  Account? sender;
  MessageStatus? status;

  Message({
    this.id,
    required this.chatId,
    required this.senderId,
    this.content,
    required this.createdDate,
    this.attachments,
    this.messageReaders,
    this.chat,
    this.sender,
    this.status,
  });

  factory Message.fromJson(Map<String, dynamic> json) {
    return Message(
      id: json['id'],
      chatId: json['chatId'],
      senderId: json['senderId'],
      content: json['content'],
      createdDate: DateTime.parse(json['createdDate']).toLocal(),
      attachments: (json['attachments'] as List?)
          ?.map((i) => Attachment.fromJson(i))
          .toList(),
      messageReaders: (json['messageReaders'] as List?)
          ?.map((i) => MessageReader.fromJson(i))
          .toList(),
      // Map json['sender'] to Account object
      sender: json['sender'] != null ? Account.fromJson(json['sender']) : null,
      status: json['status'] != null ? MessageStatus.values.byName(json['status']) : MessageStatus.sent,
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

  // copyWith method
  Message copyWith({
    String? id,
    String? chatId,
    String? senderId,
    String? content,
    DateTime? createdDate,
    List<Attachment>? attachments,
    List<MessageReader>? messageReaders,
    Chat? chat,
    Account? sender,
    MessageStatus? status,
  }) {
    return Message(
      id: id ?? this.id,
      chatId: chatId ?? this.chatId,
      senderId: senderId ?? this.senderId,
      content: content ?? this.content,
      createdDate: createdDate ?? this.createdDate,
      attachments: attachments ?? this.attachments,
      messageReaders: messageReaders ?? this.messageReaders,
      chat: chat ?? this.chat,
      sender: sender ?? this.sender,
      status: status ?? this.status,
    );
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

// Tạo class Message cho send message
class MessageSendDto {
  final String tempId;
  final String chatId;
  final String? content;
  final List<Attachment>? attachments;

  MessageSendDto({
    required this.tempId,
    this.content,
    required this.chatId,
    this.attachments,
  });

  Map<String, dynamic> toJson() {
    return {
      'tempId': tempId,
      'chatId': chatId,
      'content': content,
      'attachments': attachments?.map((i) => i.toJson()).toList(),
    };
  }
}

enum MessageStatus {
  sending,
  sent,
  delivered,
  read,
  failed,
}

extension MessageStatusExtension on MessageStatus {
  String get description {
    switch (this) {
      case MessageStatus.sending:
        return 'Sending...';
      case MessageStatus.sent:
        return 'Sent';
      case MessageStatus.delivered:
        return 'Delivered';
      case MessageStatus.read:
        return 'Read';
      case MessageStatus.failed:
        return 'Failed to send';
    }
  }
}
