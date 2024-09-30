// import 'dart:async';
// import 'package:flutter_webrtc/flutter_webrtc.dart';

// class WebRTCService {
//   RTCPeerConnection? _peerConnection;
//   final Map<String, RTCIceCandidate> _pendingCandidates = {};
//   MediaStream? _localStream;
//   MediaStream? _remoteStream;
//   RTCVideoRenderer? _localRenderer;
//   RTCVideoRenderer? _remoteRenderer;

//   Function(RTCIceCandidate)? onIceCandidate;
//   Function(MediaStream)? onAddStream;

//   WebRTCService() {
//     initializeRenderers();
//   }

//   Future<void> initializeRenderers() async {
//     _localRenderer = RTCVideoRenderer();
//     _remoteRenderer = RTCVideoRenderer();
//     await _localRenderer!.initialize();
//     await _remoteRenderer!.initialize();
//   }

//   Future<void> initializePeerConnection() async {
//     _peerConnection = await createPeerConnection({
//       'iceServers': [
//         {'urls': 'stun:stun.l.google.com:19302'},
//       ]
//     }, {});

//     _peerConnection!.onIceCandidate = (RTCIceCandidate candidate) {
//       onIceCandidate?.call(candidate);
//     };

//     _peerConnection!.onAddStream = (MediaStream stream) {
//       _remoteStream = stream;
//       _remoteRenderer!.srcObject = _remoteStream;
//       onAddStream?.call(stream);
//     };

//     _peerConnection!.onRemoveStream = (MediaStream stream) {
//       _remoteRenderer!.srcObject = null;
//     };

//     _peerConnection!.onIceConnectionState = (RTCIceConnectionState state) {
//       print('ICE Connection State: $state');
//     };
//   }

//   Future<String?> createOffer() async {
//     RTCSessionDescription offer = await _peerConnection!.createOffer();
//     await _peerConnection!.setLocalDescription(offer);
//     return offer.sdp;
//   }

//   Future<String?> createAnswer() async {
//     RTCSessionDescription answer = await _peerConnection!.createAnswer();
//     await _peerConnection!.setLocalDescription(answer);
//     return answer.sdp;
//   }

//   Future<void> setRemoteDescription(String sdp) async {
//     await _peerConnection!.setRemoteDescription(
//       RTCSessionDescription(sdp, sdp.startsWith('v=') ? 'offer' : 'answer'),
//     );
//   }

//   Future<void> addIceCandidate(RTCIceCandidate candidate) async {
//     if (_peerConnection != null) {
//       await _peerConnection!.addCandidate(candidate);
//     } else {
//       _pendingCandidates[candidate.sdpMid!] = candidate;
//     }
//   }

//   Future<void> getUserMedia(bool isVideo) async {
//     final Map<String, dynamic> constraints = {
//       'audio': true,
//       'video': isVideo
//           ? {
//               'facingMode': 'user',
//             }
//           : false,
//     };

//     _localStream = await navigator.mediaDevices.getUserMedia(constraints);
//     _localRenderer!.srcObject = _localStream;

//     // Add tracks to peer connection
//     _localStream!.getTracks().forEach((track) {
//       _peerConnection!.addTrack(track, _localStream!);
//     });
//   }

//   Future<void> switchCamera() async {
//     if (_localStream != null) {
//       await Helper.switchCamera(_localStream!.getVideoTracks()[0]);
//     }
//   }

//   Future<void> toggleMicrophone(bool enabled) async {
//     if (_localStream != null) {
//       _localStream!.getAudioTracks()[0].enabled = enabled;
//     }
//   }

//   Future<void> toggleCamera(bool enabled) async {
//     if (_localStream != null) {
//       _localStream!.getVideoTracks()[0].enabled = enabled;
//     }
//   }

//   void dispose() {
//     _localStream?.dispose();
//     _remoteStream?.dispose();
//     _peerConnection?.close();
//     _localRenderer?.dispose();
//     _remoteRenderer?.dispose();
//   }

//   MediaStream? get localStream => _localStream;
//   MediaStream? get remoteStream => _remoteStream;
//   RTCVideoRenderer? get localRenderer => _localRenderer;
//   RTCVideoRenderer? get remoteRenderer => _remoteRenderer;

//   Future<void> setLocalDescription(String sdp) async {
//     await _peerConnection!.setLocalDescription(
//       RTCSessionDescription(sdp, sdp.startsWith('v=') ? 'offer' : 'answer'),
//     );
//   }
// }
