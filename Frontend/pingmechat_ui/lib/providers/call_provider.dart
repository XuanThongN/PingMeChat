import 'package:flutter/material.dart';
import 'package:flutter_webrtc/flutter_webrtc.dart';
import 'package:permission_handler/permission_handler.dart';
import '../data/datasources/chat_hub_service.dart';
import '../domain/models/chat.dart';

class CallProvider extends ChangeNotifier {
  final ChatHubService _chatHubService;
  RTCPeerConnection? _peerConnection;
  MediaStream? _localStream;
  RTCVideoRenderer localRenderer = RTCVideoRenderer();
  RTCVideoRenderer remoteRenderer = RTCVideoRenderer();
  bool isCallActive = false;
  bool isVideo = false;

  CallProvider(this._chatHubService) {
    _initRenderers();
    _setupSignalRHandlers();
  }

  Future<void> _initRenderers() async {
    await localRenderer.initialize();
    await remoteRenderer.initialize();
  }

  void _setupSignalRHandlers() {
    _chatHubService.onIncomingCall(_handleIncomingCall);
    _chatHubService.onCallAnswered(_handleCallAnswered);
    _chatHubService.onIceCandidate(_handleIceCandidate);
    _chatHubService.onOffer(_handleOffer);
    _chatHubService.onAnswer(_handleAnswer);
    _chatHubService.onCallEnded(_handleCallEnded);
  }
  Future<void> _handleCameraAndMic(Permission permission) async {
    final status = await permission.request();
    print(status);
  }
  Future<void> initiateCall(Chat chat, bool isVideoCall) async {
    // Request permissions before starting the call
    await _handleCameraAndMic(Permission.camera);
    await _handleCameraAndMic(Permission.microphone);
    
    isVideo = isVideoCall;
    isCallActive = true;
    await _setupLocalStream();
    await _createPeerConnection();
    await _createOffer(chat.id);
    notifyListeners();
  }

  Future<void> _setupLocalStream() async {
    _localStream = await navigator.mediaDevices.getUserMedia({
      'audio': true,
      'video': isVideo,
    });
    localRenderer.srcObject = _localStream;
  }

  Future<void> _createPeerConnection() async {
    _peerConnection = await createPeerConnection({
      'iceServers': [{'urls': 'stun:stun.l.google.com:19302'}]
    });

    _localStream!.getTracks().forEach((track) {
      _peerConnection!.addTrack(track, _localStream!);
    });

    _peerConnection!.onIceCandidate = (candidate) {
      _chatHubService.sendIceCandidate(candidate.toMap());
    };

    _peerConnection!.onTrack = (event) {
      if (event.track.kind == 'video') {
        remoteRenderer.srcObject = event.streams[0];
      }
    };
  }

  Future<void> _createOffer(String chatId) async {
    RTCSessionDescription offer = await _peerConnection!.createOffer();
    await _peerConnection!.setLocalDescription(offer);
    await _chatHubService.sendOffer(chatId, offer.sdp!);
  }

  void _handleIncomingCall(String callerId, String chatId, bool isVideoCall) {
    // Handle incoming call UI
    isVideo = isVideoCall;
    isCallActive = true;
    notifyListeners();
  }

  void _handleCallAnswered(String answeringUserId, String chatId, bool accepted) {
    if (accepted) {
      // Start the call
    } else {
      // Handle rejected call
      endCall();
    }
  }

  void _handleIceCandidate(Map<String, dynamic> candidateMap) {
    RTCIceCandidate candidate = RTCIceCandidate(
      candidateMap['candidate'],
      candidateMap['sdpMid'],
      candidateMap['sdpMLineIndex'],
    );
    _peerConnection?.addCandidate(candidate);
  }

  void _handleOffer(String sdp) {
    _peerConnection?.setRemoteDescription(RTCSessionDescription(sdp, 'offer'));
    _createAnswer();
  }

  void _handleAnswer(String sdp) {
    _peerConnection?.setRemoteDescription(RTCSessionDescription(sdp, 'answer'));
  }

  void _handleCallEnded(String userId) {
    endCall();
  }

  Future<void> _createAnswer() async {
    RTCSessionDescription answer = await _peerConnection!.createAnswer();
    await _peerConnection!.setLocalDescription(answer);
    await _chatHubService.sendAnswer(answer.sdp!);
  }

  void endCall() {
    _chatHubService;
    _disposeCall();
    isCallActive = false;
    notifyListeners();
  }

  void _disposeCall() {
    _localStream?.dispose();
    _peerConnection?.close();
  }

  @override
  void dispose() {
    localRenderer.dispose();
    remoteRenderer.dispose();
    _disposeCall();
    super.dispose();
  }
}