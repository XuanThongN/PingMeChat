class ChatCreateDto {
  final String id;
  final String name;
  final DateTime createdAt;

  ChatCreateDto({
    required this.id,
    required this.name,
    required this.createdAt,
  });

  factory ChatCreateDto.fromJson(Map<String, dynamic> json) {
    return ChatCreateDto(
      id: json['id'],
      name: json['name'],
      createdAt: DateTime.parse(json['createdAt']),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'name': name,
      'createdAt': createdAt.toIso8601String(),
    };
  }
}