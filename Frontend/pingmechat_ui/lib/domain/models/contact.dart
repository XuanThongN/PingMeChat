import 'package:pingmechat_ui/domain/models/account.dart';

class Contact {
  final String id;
  final String? nickname;
  final String? fullName; // Thêm fullName vào Contact
  final DateTime? addedAt;
  final String? settings;
  final String? avatarUrl; // Thêm avatarUrl vào Contact
  final bool isOnline; // Thêm isOnline vào Contact
  final Account? user; // Thêm account vào Contact
  final Account? contactUser; // Thêm contactAccount vào Contact

  Contact({
    required this.id,
    this.fullName,
    this.addedAt,
    this.nickname,
    this.settings,
    this.avatarUrl,
    this.isOnline = true,
    this.user,
    this.contactUser,

  });

  factory Contact.fromJson(Map<String, dynamic> json) {
    return Contact(
      id: json['id'],
      fullName: json['fullName'] ?? '',
      nickname: json['nickname'] ?? '',
      settings: json['settings'] ?? '',
      avatarUrl: json['avatarUrl'] ?? '',
      isOnline: json['isOnline'] ?? true,
      user: json['user'] != null ? Account.fromJson(json['user']) : null,
      contactUser: json['contactUser'] != null ? Account.fromJson(json['contactUser']) : null,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'fullName': fullName,
      'nickname': nickname,
      'settings': settings,
    };
  }
}

// Tạo class thêm liên hệ mới
class AddContactRequest {
  final String contactId;
  final String? nickname;

  AddContactRequest({
    required this.contactId,
    this.nickname,
  });

  Map<String, dynamic> toJson() {
    return {
      'contactUserId': contactId,
      'nickname': nickname,
    };
  }
}
