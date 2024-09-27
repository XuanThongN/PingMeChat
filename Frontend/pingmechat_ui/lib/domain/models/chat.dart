import 'dart:core';

import 'package:pingmechat_ui/domain/models/message.dart';
import 'package:pingmechat_ui/domain/models/userchat.dart';

class Chat {
  String? name;
  bool isGroup;
  String? avatarUrl;
  List<UserChat> userChats;
  List<Message>? messages;
  String id;
  DateTime? createdDate;
  String? createdBy;

  Chat({
    this.name,
    required this.isGroup,
    this.avatarUrl,
    required this.userChats,
    this.messages,
    required this.id,
    this.createdDate,
    this.createdBy,
  });

  factory Chat.fromJson(Map<String, dynamic> json) {
    return Chat(
        name: json['name'] ?? '',
        isGroup: json['isGroup'] ?? false,
        avatarUrl: json['avatarUrl'] ?? '',
        userChats: List<UserChat>.from(json['userChats']
            .map((userChatJson) => UserChat.fromJson(userChatJson))),
        messages: List<Message>.from(json['messages']
            .map((messageJson) => Message.fromJson(messageJson))),
        id: json['id'],
        // Nếu không có createdDate thì gán giá trị mặc định là null
        createdDate: json['createdDate'] != null
            ? DateTime.parse(json['createdDate'])
            : null,
        createdBy: json['createdBy'] ?? '');
  }
  Map<String, dynamic> toJson() {
    return {
      'name': name,
      'isGroup': isGroup,
      'avatarUrl': avatarUrl,
      'userChats': userChats.map((i) => i.toJson()).toList(),
      'messages': messages!.map((i) => i.toJson()).toList(),
    };
  }
}
