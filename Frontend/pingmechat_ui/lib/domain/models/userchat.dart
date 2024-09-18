class UserChat {
  final String userId;
  final String chatId;
  final bool isAdmin;

  UserChat({
    required this.userId,
    required this.chatId,
    required this.isAdmin,
  });

  factory UserChat.fromJson(Map<String, dynamic> json) {
    return UserChat(
      userId: json['userId'],
      chatId: json['chatId'],
      isAdmin: json['isAdmin'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'userId': userId,
      'chatId': chatId,
      'isAdmin': isAdmin,
    };
  }
}