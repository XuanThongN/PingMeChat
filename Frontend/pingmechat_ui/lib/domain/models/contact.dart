import 'package:pingmechat_ui/core/constants/constant.dart';
import 'package:pingmechat_ui/domain/models/account.dart';

class Contact {
  final String id;
  String? nickname;
  String? fullName; // Thêm fullName vào Contact
  final DateTime? createdDate;
  final String? settings;
  String? avatarUrl; // Thêm avatarUrl vào Contact
  String? phoneNumber; // Thêm phoneNumber vào Contact
  String? email; // Thêm email vào Contact
  final bool isOnline; // Thêm isOnline vào Contact
  final Account? user; // Thêm account vào Contact
  final Account? contactUser; // Thêm contactAccount vào Contact
  String status; // Thêm status vào Contact

  Contact({
    required this.id,
    this.fullName,
    this.createdDate,
    this.nickname,
    this.settings,
    this.avatarUrl,
    this.isOnline = true,
    this.phoneNumber,
    this.email,
    this.user,
    this.contactUser,
    this.status = ContactStatus.STRANGER,
  });

  factory Contact.fromJson(Map<String, dynamic> json) {
    return Contact(
      id: json['id'],
      fullName: json['fullName'] ?? '',
      nickname: json['nickname'] ?? '',
      settings: json['settings'] ?? '',
      avatarUrl: json['avatarUrl'] ?? '',
      isOnline: json['isOnline'] ?? true,
      phoneNumber: json['phoneNumber'] ?? '',
      email: json['email'] ?? '',
      user: json['user'] != null ? Account.fromJson(json['user']) : null,
      contactUser: json['contactUser'] != null
          ? Account.fromJson(json['contactUser'])
          : null,
      status: json['status'] ?? ContactStatus.STRANGER,
      createdDate: json['createdDate'] != null
          ? DateTime.parse(json['createdDate'])
          : DateTime.now(),
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
