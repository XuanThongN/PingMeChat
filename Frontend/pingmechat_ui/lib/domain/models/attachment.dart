class Attachment {
  final String fileName;
  final String filePath;
  final String fileType;
  final int fileSize;
  final String? messageId;

  Attachment({
    required this.fileName,
    required this.filePath,
    required this.fileType,
    required this.fileSize,
    this.messageId,
  });

  factory Attachment.fromJson(Map<String, dynamic> json) {
    return Attachment(
      fileName: json['fileName'],
      filePath: json['filePath'],
      fileType: json['fileType'],
      fileSize: json['fileSize'],
      messageId: json['messageId'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'fileName': fileName,
      'filePath': filePath,
      'fileType': fileType,
      'fileSize': fileSize,
      'messageId': messageId,
    };
  }
}