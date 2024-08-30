import 'package:flutter/material.dart';
import 'package:camera/camera.dart';

class VideoCallPage extends StatefulWidget {
  @override
  _VideoCallPageState createState() => _VideoCallPageState();
}

class _VideoCallPageState extends State<VideoCallPage> {
  CameraController? _cameraController;
  late Future<void> _initializeControllerFuture;

  @override
  void initState() {
    super.initState();
    _initializeCamera();
  }

  Future<void> _initializeCamera() async {
    // Lấy danh sách camera
    final cameras = await availableCameras();
    final firstCamera = cameras.first;

    _cameraController = CameraController(
      firstCamera,
      ResolutionPreset.high,
    );

    _initializeControllerFuture = _cameraController!.initialize();

    // Sử dụng setState để cập nhật UI
    setState(() {});
  }

  @override
  void dispose() {
    _cameraController?.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Stack(
        children: [
          FutureBuilder<void>(
            future: _initializeControllerFuture, // Đợi camera được khởi tạo
            builder: (context, snapshot) {
              if (snapshot.connectionState == ConnectionState.done) {
                // Hiển thị video của người khác
                return Center(
                  child: CameraPreview(_cameraController!),
                );
              } else {
                return Center(child: CircularProgressIndicator());
              }
            },
          ),
          Positioned(
            top: 50,
            left: 20,
            child: Text(
              'Calling ...',
              style: TextStyle(color: Colors.white, fontSize: 18),
            ),
          ),
          // Hiển thị video từ camera của máy hiện tại
          Positioned(
            right: 20,
            bottom: 20,
            child: Container(
              width: 120,
              height: 160,
              decoration: BoxDecoration(
                borderRadius: BorderRadius.circular(8.0),
                border: Border.all(color: Colors.white, width: 2),
              ),
              child: FutureBuilder<void>(
                future: _initializeControllerFuture,
                builder: (context, snapshot) {
                  if (snapshot.connectionState == ConnectionState.done) {
                    return ClipRRect(
                      borderRadius: BorderRadius.circular(8.0),
                      child: CameraPreview(_cameraController!),
                    );
                  } else {
                    return Center(child: CircularProgressIndicator());
                  }
                },
              ),
            ),
          ),
          // Thời gian cuộc gọi
          Positioned(
            bottom: 90,
            child: Center(
              child: Text(
                '03:45',
                style: TextStyle(color: Colors.white, fontSize: 16),
              ),
            ),
          ),
          // Nút điều khiển
          Positioned(
            bottom: 20,
            left: 0,
            right: 0,
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceEvenly,
              children: [
                IconButton(
                  icon: Icon(Icons.mic, color: Colors.white),
                  onPressed: () {
                    // Xử lý tắt/bật mic
                  },
                ),
                IconButton(
                  icon: Icon(Icons.volume_up, color: Colors.white),
                  onPressed: () {
                    // Xử lý tắt/bật loa
                  },
                ),
                IconButton(
                  icon: Icon(Icons.switch_camera, color: Colors.white),
                  onPressed: () {
                    // Chuyển đổi giữa camera trước và sau
                  },
                ),
                IconButton(
                  icon: Icon(Icons.call_end, color: Colors.red),
                  onPressed: () {
                    // Kết thúc cuộc gọi
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
