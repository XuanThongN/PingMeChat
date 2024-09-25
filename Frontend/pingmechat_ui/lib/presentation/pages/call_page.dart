// import 'package:flutter/material.dart';
// import 'package:pingmechat_ui/domain/models/chat.dart';
// import 'dart:ui';

// import '../../config/theme.dart';

// class CallPage extends StatelessWidget {

//   final Chat chat;
//   final bool isVideo;
//   final bool isIncoming;
//   const CallPage({super.key, required this.chat, required this.isVideo, required this.isIncoming});

//   @override
//   Widget build(BuildContext context) {
//     return Scaffold(
//       body: Stack(
//         children: [
//           // Background Image with Blur Effect
//           Container(
//             decoration: const BoxDecoration(
//               image: DecorationImage(
//                 image: NetworkImage(
//                   'https://plus.unsplash.com/premium_photo-1670148434900-5f0af77ba500?fm=jpg&q=60&w=3000&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MXx8c3BsYXNofGVufDB8fDB8fHww', // Replace with your background image URL
//                 ),
//                 fit: BoxFit.cover,
//               ),
//             ),
//             child: BackdropFilter(
//               filter: ImageFilter.blur(sigmaX: 5.0, sigmaY: 5.0),
//               child: Container(
//                 color: Colors.black.withOpacity(0.5),
//               ),
//             ),
//           ),
//           // Call Content
//           SafeArea(
//             child: Column(
//               mainAxisAlignment: MainAxisAlignment.spaceBetween,
//               children: [
//                 // Top Section with back button and calling text
//                 Padding(
//                   padding: const EdgeInsets.symmetric(
//                       horizontal: 20.0, vertical: 10.0),
//                   child: Row(
//                     children: [
//                       IconButton(
//                         icon: const Icon(Icons.arrow_back, color: AppColors.white),
//                         onPressed: () {
//                           Navigator.pop(context);
//                         },
//                       ),
//                       const Spacer(),
//                       const Text(
//                         'Calling ...',
//                         style: TextStyle(
//                           color: Colors.white,
//                           fontSize: 18,
//                         ),
//                       ),
//                       const Spacer(flex: 2),
//                     ],
//                   ),
//                 ),
//                 // Center Section with Profile Picture, Name, and Phone Number
//                 Column(
//                   children: [
//                     const CircleAvatar(
//                       radius: 50,
//                       backgroundImage: NetworkImage(
//                         'https://plus.unsplash.com/premium_photo-1670148434900-5f0af77ba500?fm=jpg&q=60&w=3000&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MXx8c3BsYXNofGVufDB8fDB8fHww', // Replace with your profile image URL
//                       ),
//                     ),
//                     const SizedBox(height: 20),
//                     const Text(
//                       'David Wayne',
//                       style: TextStyle(
//                         color: Colors.white,
//                         fontSize: 24,
//                         fontWeight: FontWeight.bold,
//                       ),
//                     ),
//                     const SizedBox(height: 10),
//                     Text(
//                       '(+44) 50 9285 3022',
//                       style: TextStyle(
//                         color: Colors.white.withOpacity(0.8),
//                         fontSize: 16,
//                       ),
//                     ),
//                   ],
//                 ),
//                 // Bottom Section with Call Timer and Action Buttons
//                 Column(
//                   children: [
//                     // Call Duration Timer
//                     Container(
//                       padding: const EdgeInsets.symmetric(
//                           vertical: 10.0, horizontal: 20.0),
//                       decoration: BoxDecoration(
//                         color: Colors.black.withOpacity(0.5),
//                         borderRadius: BorderRadius.circular(20),
//                       ),
//                       child: const Text(
//                         '03:45',
//                         style: TextStyle(
//                           color: Colors.white,
//                           fontSize: 16,
//                         ),
//                       ),
//                     ),
//                     const SizedBox(height: 20),
//                     // Action Buttons (Mute, Speaker, End Call)
//                     Row(
//                       mainAxisAlignment: MainAxisAlignment.center,
//                       children: [
//                         CircleButton(
//                           icon: Icons.mic_off,
//                           backgroundColor: Colors.white,
//                           iconColor: Colors.black,
//                           onPressed: () {},
//                         ),
//                         const SizedBox(width: 30),
//                         CircleButton(
//                           icon: Icons.volume_up,
//                           backgroundColor: Colors.white,
//                           iconColor: Colors.black,
//                           onPressed: () {},
//                         ),
//                         const SizedBox(width: 30),
//                         CircleButton(
//                           icon: Icons.call_end,
//                           backgroundColor: Colors.red,
//                           iconColor: Colors.white,
//                           onPressed: () {},
//                         ),
//                       ],
//                     ),
//                     const SizedBox(height: 40),
//                   ],
//                 ),
//               ],
//             ),
//           ),
//         ],
//       ),
//     );
//   }
// }

// class CircleButton extends StatelessWidget {
//   final IconData icon;
//   final Color backgroundColor;
//   final Color iconColor;
//   final VoidCallback onPressed;

//   const CircleButton({
//     super.key,
//     required this.icon,
//     required this.backgroundColor,
//     required this.iconColor,
//     required this.onPressed,
//   });

//   @override
//   Widget build(BuildContext context) {
//     return InkWell(
//       onTap: onPressed,
//       child: Container(
//         padding: const EdgeInsets.all(15.0),
//         decoration: BoxDecoration(
//           shape: BoxShape.circle,
//           color: backgroundColor,
//         ),
//         child: Icon(
//           icon,
//           color: iconColor,
//           size: 30,
//         ),
//       ),
//     );
//   }
// }


import 'package:flutter/material.dart';
import 'package:flutter_webrtc/flutter_webrtc.dart';
import 'package:provider/provider.dart';
import '../../providers/call_provider.dart';
import '../../domain/models/chat.dart';

class CallPage extends StatelessWidget {
  final bool isIncoming;

  const CallPage({Key? key,required this.isIncoming}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    final callProvider = Provider.of<CallProvider>(context);

    return Scaffold(
      body: Stack(
        children: [
          RTCVideoView(callProvider.remoteRenderer, objectFit: RTCVideoViewObjectFit.RTCVideoViewObjectFitCover),
          Positioned(
            right: 20,
            top: 20,
            child: Container(
              width: 100,
              height: 150,
              child: RTCVideoView(callProvider.localRenderer, mirror: true),
            ),
          ),
          Positioned(
            bottom: 30,
            left: 0,
            right: 0,
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceEvenly,
              children: [
                IconButton(
                  icon: Icon(Icons.call_end),
                  color: Colors.red,
                  onPressed: callProvider.endCall,
                ),
                if (callProvider.isVideo)
                  IconButton(
                    icon: Icon(Icons.switch_camera),
                    color: Colors.white,
                    onPressed: () {
                      // Implement switch camera logic
                    },
                  ),
                IconButton(
                  icon: Icon(Icons.mic_off),
                  color: Colors.white,
                  onPressed: () {
                    // Implement mute/unmute logic
                  },
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}