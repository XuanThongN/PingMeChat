import 'package:flutter/material.dart';

class UploadProgressIndicator extends StatefulWidget {
  final bool isUploading;

  UploadProgressIndicator({required this.isUploading});

  @override
  _UploadProgressIndicatorState createState() => _UploadProgressIndicatorState();
}

class _UploadProgressIndicatorState extends State<UploadProgressIndicator>
    with SingleTickerProviderStateMixin {
  late AnimationController _controller;
  late Animation<double> _animation;

  @override
  void initState() {
    super.initState();
    _controller = AnimationController(
      duration: const Duration(seconds: 10), // Thay đổi thời gian nếu cần
      vsync: this,
    );

    _animation = Tween<double>(begin: 0, end: 1).animate(_controller);

    if (widget.isUploading) {
      _controller.forward();
    }
  }

  @override
  void didUpdateWidget(UploadProgressIndicator oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (widget.isUploading && !oldWidget.isUploading) {
      _controller.forward(from: 0); // Reset animation khi bắt đầu upload
    } else if (!widget.isUploading) {
      _controller.stop();
      _controller.reset(); // Reset animation khi không còn upload
    }
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: 30, // Kích thước cố định để không làm tăng kích cỡ cha
      height: 30,
      child: widget.isUploading
          ? Stack(
              alignment: Alignment.center,
              children: [
                // Vòng tròn nền
                Container(
                  width: 30,
                  height: 30,
                  decoration: BoxDecoration(
                    shape: BoxShape.circle,
                    color: Colors.grey[300],
                  ),
                ),
                // Vòng tròn progress
                AnimatedBuilder(
                  animation: _animation,
                  builder: (context, child) {
                    return CustomPaint(
                      painter: ProgressPainter(_animation.value),
                      size: const Size(30, 30), // Đảm bảo kích thước nhỏ gọn
                    );
                  },
                ),
              ],
            )
          : SizedBox.shrink(), // Ẩn widget khi không upload
    );
  }
}

class ProgressPainter extends CustomPainter {
  final double progress;

  ProgressPainter(this.progress);

  @override
  void paint(Canvas canvas, Size size) {
    final paint = Paint()
      ..color = Colors.blue
      ..style = PaintingStyle.stroke
      ..strokeWidth = 4; // Đường viền nhỏ hơn để phù hợp với kích thước

    // Vẽ vòng tròn progress
    canvas.drawArc(
      Rect.fromCircle(center: Offset(size.width / 2, size.height / 2), radius: size.width / 2 - 2),
      -90 * 3.14 / 180, // Bắt đầu từ 12 giờ
      (progress * 2 * 3.14), // Độ dài vòng tròn dựa trên progress
      false,
      paint,
    );
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) {
    return true;
  }
}