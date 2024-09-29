import 'package:flutter/material.dart';
import 'package:flutter_webrtc/flutter_webrtc.dart';
import 'package:provider/provider.dart';

import '../../providers/call_provider.dart';

class CallPage extends StatelessWidget {
  final String chatId;
  final bool isVideo;
  final RTCVideoRenderer localRenderer;
  final RTCVideoRenderer remoteRenderer;
  final VoidCallback onEndCall;

  CallPage({
    required this.chatId,
    required this.isVideo,
    required this.localRenderer,
    required this.remoteRenderer,
    required this.onEndCall,
  });

  @override
  Widget build(BuildContext context) {
    final callProvider = Provider.of<CallProvider>(context);

    return Scaffold(
      body: Stack(
        children: [
          if (isVideo) ...[
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
          ] else ...[
            Center(child: Text('Audio Call')),
          ],
          Positioned(
            bottom: 20,
            left: 0,
            right: 0,
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceEvenly,
              children: [
                IconButton(
                  icon: Icon(Icons.mic_off),
                  onPressed: callProvider.toggleMute,
                ),
                IconButton(
                  icon: Icon(Icons.call_end),
                  onPressed: onEndCall,
                  color: Colors.red,
                ),
                if (isVideo)
                  IconButton(
                    icon: Icon(Icons.switch_camera),
                    onPressed: callProvider.switchCamera,
                  ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}