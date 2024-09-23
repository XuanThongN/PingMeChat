class ChatCreateDto {
  final String? name;
  final bool isGroup;
  final String? avatar;
  final List<String> userIds;

  ChatCreateDto(
      {this.name, required this.isGroup, this.avatar, required this.userIds});

  factory ChatCreateDto.fromJson(Map<String, dynamic> json) {
    return ChatCreateDto(
      name: json['name'],
      isGroup: json['isGroup'] ?? false,
      avatar: json['avatar'],
      userIds: List<String>.from(json['userIds']),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'name': name,
      'isGroup': isGroup,
      'avatar': avatar,
      'userIds': userIds,
    };
  }
}
