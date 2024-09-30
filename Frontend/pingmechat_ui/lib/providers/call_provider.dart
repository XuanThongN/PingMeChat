// import 'package:flutter/material.dart';
// import 'package:flutter_webrtc/flutter_webrtc.dart';
// import '../data/datasources/chat_hub_service.dart';
// import 'webrtc_service.dart';

// enum CallState { idle, outgoing, incoming, connected }

// class CallProvider with ChangeNotifier {
//   final ChatHubService _chatHubService;
//   final WebRTCService _webRTCService;

//   CallState _callState = CallState.idle;
//   String? _currentChatId;
//   String? _remoteUserId;
//   bool _isVideo = false;
//   bool _isMuted = false;

//   CallProvider(this._chatHubService, this._webRTCService) {
//     _setupSignalRListeners();
//     _initializeWebRTC();
//   }

//   Future<void> _initializeWebRTC() async {
//     await _webRTCService.initializePeerConnection();
//   }

//   CallState get callState => _callState;
//   String? get currentChatId => _currentChatId;
//   String? get remoteUserId => _remoteUserId;
//   bool get isVideo => _isVideo;
//   bool get isMuted => _isMuted;
//   WebRTCService get webRTCService => _webRTCService;
//   MediaStream? get localStream => _webRTCService.localStream;
//   MediaStream? get remoteStream => _webRTCService.remoteStream;

//   void _setupSignalRListeners() {
//     _chatHubService.onIncomingCall(handleIncomingCall);
//     _chatHubService.onCallAnswered(_handleCallAnswered);
//     _chatHubService.onIceCandidate(_handleIceCandidate);
//     _chatHubService.onOffer(_handleOffer);
//     _chatHubService.onAnswer(_handleAnswer);
//     _chatHubService.onCallEnded(_handleCallEnded);
//   }

//   Future<void> initiateCall(String chatId, bool isVideo) async {
//     _currentChatId = chatId;
//     _isVideo = isVideo;
//     _callState = CallState.outgoing;
//     notifyListeners();

//     await _webRTCService.initializePeerConnection();
//     await _webRTCService.getUserMedia(isVideo);

//     final offer = await _webRTCService.createOffer();
//     await _chatHubService.initiateCall(chatId, isVideo);
//     await _chatHubService.sendOffer(chatId, offer!);
//   }

//   Future<void> acceptCall() async {
//     _callState = CallState.connected;
//     notifyListeners();

//     await _webRTCService.initializePeerConnection();
//     await _webRTCService.getUserMedia(_isVideo);

//     // Wait for the offer before creating an answer
//   }

//   // Hàm để reject call
//   Future<void> rejectCall() async {
//     // await _chatHubService.rejectCall(_currentChatId!);
//     _endCall();
//   }

//   Future<void> handleIncomingCall(String callerId, String chatId, bool isVideo) async {
//     _remoteUserId = callerId;
//     _currentChatId = chatId;
//     _isVideo = isVideo;
//     _callState = CallState.incoming;
//     notifyListeners();

//     // Don't create an offer here, wait for the user to accept the call
//   }

//   void _handleCallAnswered(String answeringUserId, String chatId, bool accept) {
//     if (accept) {
//       _callState = CallState.connected;
//     } else {
//       _endCall();
//     }
//     notifyListeners();
//   }

//   Future<void> _handleIceCandidate(
//       String userId, String candidateString) async {
//     final candidate = RTCIceCandidate(
//       candidateString,
//       '',
//       0,
//     );
//     await _webRTCService.addIceCandidate(candidate);
//   }

//   Future<void> _handleOffer(String userId, String sdp) async {
//     if (_callState != CallState.connected) {
//       // If we're not in a connected state, we shouldn't be handling offers
//       return;
//     }

//     await _webRTCService.setRemoteDescription(sdp);
//     final answer = await _webRTCService.createAnswer();
//     await _webRTCService.setLocalDescription(answer!);
//     await _chatHubService.sendAnswer(_currentChatId!, answer);
//   }

//   Future<void> _handleAnswer(String userId, String sdp) async {
//     await _webRTCService.setRemoteDescription(sdp);
//   }

//   void _handleCallEnded(String userId) {
//     _endCall();
//   }

//   void _endCall() {
//     _callState = CallState.idle;
//     _currentChatId = null;
//     _remoteUserId = null;
//     _isVideo = false;
//     _webRTCService.dispose();
//     notifyListeners();
//   }

//   Future<void> endCall() async {
//     await _chatHubService.endCall(_currentChatId!);
//     _endCall();
//   }

//   void toggleMute() {
//     _isMuted = !_isMuted;
//     _webRTCService.toggleMicrophone(_isMuted);
//     notifyListeners();
//   }

//   void toggleCamera() {
//     _isVideo = !_isVideo;
//     _webRTCService.toggleCamera(_isVideo);
//     notifyListeners();
//   }
// }
