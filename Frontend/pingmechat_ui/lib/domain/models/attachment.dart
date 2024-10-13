class Attachment {
  final String? fileName;
  final String fileUrl;
  final String fileType;
  final int fileSize;
  final String? messageId;
  bool isUploading;
  String? thumbnailUrl;

  Attachment({
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
      fileName: json['fileName'] ?? '',
      fileUrl: json['filePath'],
      fileType: json['fileType'],
      fileSize: json['fileSize'],
      messageId: json['messageId'],
      isUploading: json['isUploading'] ?? false,
      thumbnailUrl: json['thumbnailUrl'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'fileName': fileName,
      'fileUrl': fileUrl,
      'fileType': fileType,
      'fileSize': fileSize,
      'messageId': messageId,
      'isUploading': isUploading,
      'thumbnailUrl': thumbnailUrl,
    };
  }
}
