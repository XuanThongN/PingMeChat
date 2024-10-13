import 'dart:io';
import 'dart:math';
import 'package:http/http.dart' as http;
import 'package:mime/mime.dart';
import 'dart:convert';
import 'package:http_parser/http_parser.dart';

import '../../core/constants/constant.dart';
import '../../providers/auth_provider.dart';

class ChunkedUploader {
  final AuthProvider authProvider;
  static const int CHUNK_SIZE = 1024 * 1024; // 1MB chunks

  ChunkedUploader({required this.authProvider});

  Future<List<UploadResult>> uploadFiles(List<File> files) async {
    final List<UploadResult> results = [];

    for (var file in files) {
      try {
        final result = await _uploadFileInChunks(file);
        results.add(result);
      } catch (e) {
        print('Error uploading ${file.path}: $e');
        // You might want to add error handling here
      }
    }

    return results;
  }

  Future<UploadResult> _uploadFileInChunks(File file) async {
    final fileSize = await file.length();
    final fileName = file.path.split('/').last;
    final mimeType = lookupMimeType(file.path);
    final totalChunks = (fileSize / CHUNK_SIZE).ceil();

    String uploadId = _generateUploadId();

    for (int i = 0; i < totalChunks; i++) {
      final start = i * CHUNK_SIZE;
      final end = min((i + 1) * CHUNK_SIZE, fileSize);
      final chunkSize = end - start;

      final chunk = await file.openRead(start, end).toList();
      final bytes = chunk.expand((element) => element).toList();

      await _uploadChunk(uploadId, i, totalChunks, bytes, fileName, mimeType);
    }

    return await _completeUpload(uploadId, fileName, mimeType, fileSize);
  }

  Future<void> _uploadChunk(String uploadId, int chunkIndex, int totalChunks,
      List<int> bytes, String fileName, String? mimeType) async {
    final uri = Uri.parse(ApiConstants.uploadChunkEndpoint);
    final request = http.MultipartRequest('POST', uri);

    request.fields['uploadId'] = uploadId;
    request.fields['chunkIndex'] = chunkIndex.toString();
    request.fields['totalChunks'] = totalChunks.toString();

    request.files.add(http.MultipartFile.fromBytes(
      'chunk',
      bytes,
      filename: fileName,
      contentType: mimeType != null ? MediaType.parse(mimeType) : null,
    ));

    request.headers.addAll(await authProvider.getCustomHeaders());

    final response = await request.send();

    if (response.statusCode != 200) {
      throw Exception('Failed to upload chunk $chunkIndex');
    }
  }

  Future<UploadResult> _completeUpload(
      String uploadId, String fileName, String? mimeType, int fileSize) async {
    final uri = Uri.parse(ApiConstants.completeUploadEndpoint);
    final response = await http.post(
      uri,
      headers: {
        ...await authProvider.getCustomHeaders(),
        'Content-Type': 'application/json',
      },
      body: json.encode({
        'uploadId': uploadId,
        'fileName': fileName,
        'mimeType': mimeType,
        'fileSize': fileSize,
      }),
    );

    if (response.statusCode == 200) {
      final jsonResponse = json.decode(response.body);
      return UploadResult.fromJson(jsonResponse['result']);
    } else {
      throw Exception('Failed to complete upload');
    }
  }

  String _generateUploadId() {
    return DateTime.now().millisecondsSinceEpoch.toString();
  }
}

class UploadResult {
  String? publicId;
  String? fileName;
  String url;
  String fileType;
  int fileSize;
  UploadResult(
      {this.publicId,
      this.fileName,
      required this.url,
      required this.fileType,
      required this.fileSize});

  factory UploadResult.fromJson(Map<String, dynamic> json) {
    return UploadResult(
      publicId: json['publicId'],
      url: json['url'],
      fileName: json['fileName'],
      fileType: json['fileType'],
      fileSize: json['fileSize'],
    );
  }
}
