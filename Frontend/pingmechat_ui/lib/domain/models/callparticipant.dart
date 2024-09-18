import 'package:pingmechat_ui/domain/models/account.dart';
import 'package:pingmechat_ui/domain/models/call.dart';

class CallParticipant {
  String? role;
  DateTime joinedAt;
  DateTime? leftAt;
  Account? user;
  Call? call;

  CallParticipant({
    this.role,
    required this.joinedAt,
    this.leftAt,
    this.user,
    this.call,
  });

  factory CallParticipant.fromJson(Map<String, dynamic> json) {
    return CallParticipant(
      role: json['role'] as String?,
      joinedAt: DateTime.parse(json['joinedAt'] as String),
      leftAt: json['leftAt'] != null
          ? DateTime.parse(json['leftAt'] as String)
          : null,
      user: json['user'] != null
          ? Account.fromJson(json['user'] as Map<String, dynamic>)
          : null,
      call: json['call'] != null
          ? Call.fromJson(json['call'] as Map<String, dynamic>)
          : null,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'role': role,
      'joinedAt': joinedAt.toIso8601String(),
      'leftAt': leftAt?.toIso8601String(),
      'user': user?.toJson(),
      'call': call?.toJson(),
    };
  }
}
