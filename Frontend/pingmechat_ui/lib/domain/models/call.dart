import 'package:pingmechat_ui/domain/models/callparticipant.dart';
import 'package:pingmechat_ui/domain/models/chat.dart';

enum CallStatus {
  inProgress,
  completed,
  missed,
}

enum CallType {
  audio,
  video,
}

class Call {
  String chatId;
  CallStatus callStatus;
  CallType callType;
  DateTime startTime;
  DateTime? endTime;
  Chat chat;
  List<CallParticipant> callParticipants;

  Call({
    required this.chatId,
    required this.callStatus,
    required this.callType,
    required this.startTime,
    this.endTime,
    required this.chat,
    required this.callParticipants,
  });

  factory Call.fromJson(Map<String, dynamic> json) {
    return Call(
      chatId: json['chatId'],
      callStatus: CallStatus.values[json['callStatus']],
      callType: CallType.values[json['callType']],
      startTime: DateTime.parse(json['startTime']),
      endTime: json['endTime'] != null ? DateTime.parse(json['endTime']) : null,
      chat: Chat.fromJson(json['chat']),
      callParticipants: (json['callParticipants'] as List)
          .map((participant) => CallParticipant.fromJson(participant))
          .toList(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'chatId': chatId,
      'callStatus': callStatus.index,
      'callType': callType.index,
      'startTime': startTime.toIso8601String(),
      'endTime': endTime?.toIso8601String(),
      'chat': chat.toJson(),
      'callParticipants': callParticipants.map((participant) => participant.toJson()).toList(),
    };
  }
}
