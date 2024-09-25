import 'dart:io';

import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import 'package:permission_handler/permission_handler.dart';

class ImagePickerWidget extends StatefulWidget {
  const ImagePickerWidget({super.key});

  @override
  _ImagePickerWidgetState createState() => _ImagePickerWidgetState();
}

class _ImagePickerWidgetState extends State<ImagePickerWidget> {
  XFile? _image;

  Future<void> _pickImage() async {
    // Xin quyền truy cập vào bộ nhớ
    var status = await Permission.storage.request();
    
    if (status.isGranted) {
      // Nếu quyền được cấp, mở image picker
      final ImagePicker picker = ImagePicker();
      final XFile? image = await picker.pickImage(source: ImageSource.gallery);

      setState(() {
        _image = image;
      });
    } else if (status.isDenied) {
      // Nếu quyền bị từ chối, hiển thị thông báo
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Quyền truy cập bị từ chối. Vui lòng cấp quyền trong cài đặt.')),
      );
    } else if (status.isPermanentlyDenied) {
      // Nếu quyền bị từ chối vĩnh viễn, mở cài đặt ứng dụng
      openAppSettings();
    }
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        ElevatedButton(
          onPressed: _pickImage,
          child: const Text('Chọn ảnh'),
        ),
        if (_image != null)
          Image.file(
            File(_image!.path),
            height: 200,
          ),
      ],
    );
  }
}
