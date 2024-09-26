class Attachment {
  final String? fileName;
  final String fileUrl;
  final String fileType;
  final int fileSize;
  final String? messageId;

  Attachment({
    this.fileName,
    required this.fileUrl,
    required this.fileType,
    required this.fileSize,
    this.messageId,
  });

  factory Attachment.fromJson(Map<String, dynamic> json) {
    return Attachment(
      fileName: json['fileName'] ?? '',
      fileUrl: json['filePath'],
      fileType: json['fileType'],
      fileSize: json['fileSize'],
      messageId: json['messageId'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'fileName': fileName,
      'fileUrl': fileUrl,
      'fileType': fileType,
      'fileSize': fileSize,
      'messageId': messageId,
    };
  }
}
