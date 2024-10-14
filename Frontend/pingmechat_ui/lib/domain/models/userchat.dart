import 'package:pingmechat_ui/domain/models/account.dart';

class UserChat {
  String userId;
  String chatId;
  bool isAdmin;
  String id;
  Account? user;
  bool isOnline;

  UserChat({
    required this.userId,
    required this.chatId,
    required this.isAdmin,
    required this.id,
    this.user,
    this.isOnline = false,
  });

  factory UserChat.fromJson(Map<String, dynamic> json) {
    return UserChat(
      userId: json['userId'],
      chatId: json['chatId'],
      isAdmin: json['isAdmin'],
      id: json['id'],
      isOnline: json['isOnline'] ?? false,
      user: Account.fromJson(json['userDto']),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'userId': userId,
      'chatId': chatId,
      'isAdmin': isAdmin,
      'id': id,
    };
  }
}
