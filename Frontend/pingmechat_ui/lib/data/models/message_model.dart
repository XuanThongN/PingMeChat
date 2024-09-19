class MessageDto {
  final String chatId;
  final String content;
  final String senderId;

  MessageDto({
    required this.chatId,
    required this.content,
    required this.senderId,
  });

  factory MessageDto.fromJson(Map<String, dynamic> json) {
    return MessageDto(
      chatId: json['chatId'] as String,
      content: json['content'] as String,
      senderId: json['senderId'] as String,
    );
  }
}
