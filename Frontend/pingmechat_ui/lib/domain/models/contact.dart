class Contact {
  final String id;
  final String contactId;
  final String nickname;
  final DateTime addedAt;
  final String? settings;

  Contact({
    required this.id,
    required this.contactId,
    required this.nickname,
    required this.addedAt,
    this.settings,
  });

  factory Contact.fromJson(Map<String, dynamic> json) {
    return Contact(
      id: json['id'],
      contactId: json['contactId'],
      nickname: json['nickname'],
      addedAt: DateTime.parse(json['addedAt']),
      settings: json['settings'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'contactId': contactId,
      'nickname': nickname,
      'addedAt': addedAt.toIso8601String(),
      'settings': settings,
    };
  }
}