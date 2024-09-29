import 'dart:async';
import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:flutter_webrtc/flutter_webrtc.dart';
import '../data/datasources/chat_hub_service.dart';
import '../presentation/pages/incoming_call_overlay.dart';
import '../main.dart' show navigatorKey;

class CallProvider extends ChangeNotifier {
  final ChatHubService _chatHubService;
  RTCPeerConnection? _peerConnection;
  RTCVideoRenderer localRenderer = RTCVideoRenderer();
  RTCVideoRenderer remoteRenderer = RTCVideoRenderer();
  MediaStream? _localStream;
  bool isInCall = false;
  bool isMuted = false;
  bool isVideo = false;
  String? currentChatId;
  String? currentCallerId;
  List<RTCIceCandidate> _pendingCandidates = [];
  bool _isOffer = false;

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
    _chatHubService.onOffer(_handleOffer);
    _chatHubService.onAnswer(_handleAnswer);
    _chatHubService.onIceCandidate(_handleIceCandidate);
    _chatHubService.onCallEnded(_handleCallEnded);
  }

  Future<void> startCall(String chatId, bool isVideoCall) async {
    currentChatId = chatId;
    isInCall = true;
    isVideo = isVideoCall;
    _isOffer = true;
    await _createPeerConnection();
    await _createOffer();
    await _chatHubService.initiateCall(chatId, isVideoCall);
    notifyListeners();
  }

  Future<void> _createPeerConnection() async {
    _peerConnection = await createPeerConnection({
      'iceServers': [
        {'urls': 'stun:stun.l.google.com:19302'},
      ]
    });

    _peerConnection!.onIceCandidate = (candidate) {
      _chatHubService.sendIceCandidate(currentChatId!, candidate.toMap());
    };

    _peerConnection!.onTrack = (event) {
      if (event.track.kind == 'video') {
        remoteRenderer.srcObject = event.streams[0];
      }
      notifyListeners();
    };

    _localStream = await navigator.mediaDevices
        .getUserMedia({'audio': true, 'video': isVideo});

    _localStream!.getTracks().forEach((track) {
      _peerConnection!.addTrack(track, _localStream!);
    });

    localRenderer.srcObject = _localStream;
    notifyListeners();
  }

  Future<void> _createOffer() async {
    RTCSessionDescription offer = await _peerConnection!.createOffer();
    await _peerConnection!.setLocalDescription(offer);
    await _chatHubService.sendOffer(currentChatId!, offer.sdp!);
  }

  void _handleIncomingCall(String callerId, String chatId, bool isVideoCall) {
    print("Incoming call received: $callerId, $chatId, $isVideoCall");
    currentCallerId = callerId;
    currentChatId = chatId;
    isVideo = isVideoCall;
    showIncomingCallOverlay(callerId, chatId, isVideoCall);
  }

  void showIncomingCallOverlay(
      String callerId, String chatId, bool isVideoCall) {
    final context = navigatorKey.currentContext;
    if (context == null) {
      print("Error: No valid context found");
      return;
    }

    showDialog(
      context: context,
      barrierDismissible: false,
      builder: (BuildContext context) {
        return IncomingCallOverlay(
          callerId: callerId,
          chatId: chatId,
          isVideoCall: isVideoCall,
          onAccept: () => acceptIncomingCall(chatId, isVideoCall),
          onReject: () => endCall(),
        );
      },
    );
  }

  void _handleCallAnswered(
      String answeringUserId, String chatId, bool accept) async {
    if (accept) {
      final offer = await _peerConnection!.createOffer();
      await _peerConnection!.setLocalDescription(offer);
      await _chatHubService.sendOffer(chatId, offer.sdp!);
    } else {
      endCall();
    }
  }

  Future<void> acceptIncomingCall(String chatId, bool isVideoCall) async {
    currentChatId = chatId;
    isInCall = true;
    isVideo = isVideoCall;
    _isOffer = false;
    await _createPeerConnection();
    notifyListeners();
  }

  Future<void> _handleOffer(String userId, String sdp) async {
    if (_peerConnection == null) {
      await _createPeerConnection();
    }

    try {
      await _peerConnection!.setRemoteDescription(
        RTCSessionDescription(sdp, 'offer'),
      );

      RTCSessionDescription answer = await _peerConnection!.createAnswer();
      await _peerConnection!.setLocalDescription(answer);
      await _chatHubService.sendAnswer(currentChatId!, answer.sdp!);

      _addPendingCandidates();
    } catch (e) {
      print("Error handling offer: $e");
      endCall();
    }
  }

  Future<void> _handleAnswer(String userId, String sdp) async {
    if (_peerConnection != null) {
      try {
        await _peerConnection!.setRemoteDescription(
          RTCSessionDescription(sdp, 'answer'),
        );
        _addPendingCandidates();
      } catch (e) {
        print("Error handling answer: $e");
        endCall();
      }
    }
  }

  void _handleIceCandidate(String userId, String candidateString) {
    RTCIceCandidate candidate = RTCIceCandidate(
      json.decode(candidateString)['candidate'],
      json.decode(candidateString)['sdpMid'],
      json.decode(candidateString)['sdpMLineIndex'],
    );

    if (_peerConnection != null) {
      if (_peerConnection!.getRemoteDescription() != null) {
        _peerConnection!.addCandidate(candidate);
      } else {
        _pendingCandidates.add(candidate);
      }
    }
  }

  void _addPendingCandidates() {
    _pendingCandidates.forEach((candidate) {
      _peerConnection!.addCandidate(candidate);
    });
    _pendingCandidates.clear();
  }

  void _handleCallEnded(String userId) {
    print("Call ended by $userId");
    endCall();
  }

  void endCall() {
    print("Ending call");
    _peerConnection?.close();
    _peerConnection = null;
    _localStream?.dispose();
    _resetCallState();
  }

  void _resetCallState() {
    print("Resetting call state");
    isInCall = false;
    isMuted = false;
    isVideo = false;
    currentChatId = null;
    currentCallerId = null;
    _isOffer = false;
    notifyListeners();
  }

  void toggleMute() {
    if (_localStream != null) {
      final audioTrack = _localStream!.getAudioTracks()[0];
      audioTrack.enabled = !audioTrack.enabled;
      isMuted = !audioTrack.enabled;
      notifyListeners();
    }
  }

  void switchCamera() {
    if (_localStream != null) {
      Helper.switchCamera(_localStream!.getVideoTracks()[0]);
    }
  }

  @override
  void dispose() {
    localRenderer.dispose();
    remoteRenderer.dispose();
    _peerConnection?.close();
    _localStream?.dispose();
    super.dispose();
  }
}
