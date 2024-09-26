import 'package:flutter/material.dart';
import 'package:flutter_webrtc/flutter_webrtc.dart';
import 'package:permission_handler/permission_handler.dart';
import '../data/datasources/chat_hub_service.dart';
import '../domain/models/chat.dart';

class CallProvider extends ChangeNotifier {
  final ChatHubService _chatHubService;
  RTCPeerConnection? _peerConnection;
  RTCVideoRenderer localRenderer = RTCVideoRenderer();
  RTCVideoRenderer remoteRenderer = RTCVideoRenderer();
  bool isInCall = false;

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
    _chatHubService.onOffer(_handleOffer);
    _chatHubService.onAnswer(_handleAnswer);
    _chatHubService.onIceCandidate(_handleIceCandidate);
    _chatHubService.onCallEnded(_handleCallEnded);
  }

  Future<void> startCall(String chatId) async {
    isInCall = true;
    await _createPeerConnection();
    RTCSessionDescription offer = await _peerConnection!.createOffer();
    await _peerConnection!.setLocalDescription(offer);
    await _chatHubService.sendOffer(chatId, offer.sdp!);
    notifyListeners();
  }

  Future<void> _createPeerConnection() async {
    _peerConnection = await createPeerConnection({
      'iceServers': [
        {'urls': 'stun:stun.l.google.com:19302'}
      ]
    });

    _peerConnection!.onIceCandidate = (candidate) {
      _chatHubService.sendIceCandidate('chatId', candidate.toMap());
    };

    _peerConnection!.onTrack = (event) {
      if (event.track.kind == 'video') {
        remoteRenderer.srcObject = event.streams[0];
        notifyListeners();
      }
    };

    // Thêm local stream vào peer connection
    MediaStream localStream = await navigator.mediaDevices
        .getUserMedia({'video': true, 'audio': true});
    localStream.getTracks().forEach((track) {
      _peerConnection!.addTrack(track, localStream);
    });
    localRenderer.srcObject = localStream;
  }

  void _handleIncomingCall(String callerId, String chatId, bool isVideo) {
    // Hiển thị UI cho cuộc gọi đến
    // Ví dụ: showDialog để hiển thị thông báo cuộc gọi đến
    SnackBar snackBar = SnackBar(
      content: Text('Incoming call from $callerId'),
      action: SnackBarAction(label: 'Answer', onPressed: () => ()),
    );
  }

  Future<void> _handleOffer(String sdp) async {
    await _createPeerConnection();
    await _peerConnection!
        .setRemoteDescription(RTCSessionDescription(sdp, 'offer'));
    RTCSessionDescription answer = await _peerConnection!.createAnswer();
    await _peerConnection!.setLocalDescription(answer);
    await _chatHubService.sendAnswer(answer.sdp!);
  }

  Future<void> _handleAnswer(String sdp) async {
    await _peerConnection
        ?.setRemoteDescription(RTCSessionDescription(sdp, 'answer'));
  }

  void _handleIceCandidate(Map<String, dynamic> candidate) {
    _peerConnection?.addCandidate(RTCIceCandidate(
      candidate['candidate'],
      candidate['sdpMid'],
      candidate['sdpMLineIndex'],
    ));
  }

  void _handleCallEnded(String userId) {
    endCall();
  }

  void endCall() {
    _peerConnection?.close();
    _peerConnection = null;
    isInCall = false;
    notifyListeners();
  }

  @override
  void dispose() {
    localRenderer.dispose();
    remoteRenderer.dispose();
    _peerConnection?.close();
    super.dispose();
  }
}
