import 'package:flutter/material.dart';
import 'package:flutter_webrtc/flutter_webrtc.dart';

class CallPage extends StatelessWidget {
  final RTCVideoRenderer localRenderer;
  final RTCVideoRenderer remoteRenderer;
  final VoidCallback onEndCall;

  CallPage({
    required this.localRenderer,
    required this.remoteRenderer,
    required this.onEndCall,
  });

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Stack(
        children: [
          RTCVideoView(remoteRenderer, objectFit: RTCVideoViewObjectFit.RTCVideoViewObjectFitCover),
          Positioned(
            right: 20,
            bottom: 20,
            child: Container(
              width: 100,
              height: 150,
              child: RTCVideoView(localRenderer),
            ),
          ),
          Positioned(
            bottom: 20,
            left: 0,
            right: 0,
            child: Center(
              child: ElevatedButton(
                onPressed: onEndCall,
                child: Text('End Call'),
                style: ElevatedButton.styleFrom(
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }
}