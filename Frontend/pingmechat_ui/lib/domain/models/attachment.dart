class Attachment {
  String? uploadId;
  final String? fileName;
  final String fileUrl;
  final String fileType;
  final int fileSize;
  final String? messageId;
  bool isUploading;
  String? thumbnailUrl;

  Attachment({
    this.uploadId,
    this.fileName,
    required this.fileUrl,
    required this.fileType,
    required this.fileSize,
    this.messageId,
    this.isUploading = false,
    this.thumbnailUrl,
  });

  factory Attachment.fromJson(Map<String, dynamic> json) {
    return Attachment(
      uploadId: json['uploadId'] ?? '',
      fileName: json['fileName'] ?? '',
      fileUrl: json['filePath'],
      fileType: json['fileType'],
      fileSize: json['fileSize'],
      messageId: json['messageId'],
      isUploading: json['isUploading'] ?? false,
      thumbnailUrl: Attachment._getThumbnailUrl(json['filePath']) ?? '',
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'uploadId': uploadId,
      'fileName': fileName,
      'fileUrl': fileUrl,
      'fileType': fileType,
      'fileSize': fileSize,
      'messageId': messageId,
      'isUploading': isUploading,
      'thumbnailUrl': thumbnailUrl,
    };
  }

// Hàm lấy thumbnail url của file video bằng cách đổi extension .mp4 thành .jpg
  static String? _getThumbnailUrl(String fileUrl) {
    final extension = fileUrl.split('.').last;
    return fileUrl.replaceAll('.$extension', '.jpg') ?? '';
  }
}
