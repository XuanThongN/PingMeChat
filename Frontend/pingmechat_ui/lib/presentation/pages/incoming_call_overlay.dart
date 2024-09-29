import 'package:flutter/material.dart';

class IncomingCallOverlay extends StatelessWidget {
  final String callerId;
  final String chatId;
  final bool isVideoCall;
  final VoidCallback onAccept;
  final VoidCallback onReject;

  const IncomingCallOverlay({
    Key? key,
    required this.callerId,
    required this.chatId,
    required this.isVideoCall,
    required this.onAccept,
    required this.onReject,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: Text('Incoming ${isVideoCall ? 'Video' : 'Audio'} Call'),
      content: Text('Call from $callerId'),
      actions: <Widget>[
        TextButton(
          child: Text('Reject'),
          onPressed: () {
            onReject();
            Navigator.of(context).pop();
          },
        ),
        TextButton(
          child: Text('Accept'),
          onPressed: () {
            onAccept();
            Navigator.of(context).pop();
          },
        ),
      ],
    );
  }
}