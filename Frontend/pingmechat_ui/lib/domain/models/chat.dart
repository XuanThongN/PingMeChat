import 'package:pingmechat_ui/domain/models/message.dart';
import 'package:pingmechat_ui/domain/models/userchat.dart';

class Chat {
  final String name;
  final bool isGroup;
  final String? avatarUrl;
  final List<UserChat> userChats;
  final List<Message> messages;

  Chat({
    required this.name,
    required this.isGroup,
    this.avatarUrl,
    required this.userChats,
    required this.messages,
  });

  factory Chat.fromJson(Map<String, dynamic> json) {
    return Chat(
      name: json['name'],
      isGroup: json['isGroup'],
      avatarUrl: json['avatarUrl'],
      userChats: (json['userChats'] as List)
          .map((i) => UserChat.fromJson(i))
          .toList(),
      messages: (json['messages'] as List)
          .map((i) => Message.fromJson(i))
          .toList(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'name': name,
      'isGroup': isGroup,
      'avatarUrl': avatarUrl,
      'userChats': userChats.map((i) => i.toJson()).toList(),
      'messages': messages.map((i) => i.toJson()).toList(),
    };
  }
}