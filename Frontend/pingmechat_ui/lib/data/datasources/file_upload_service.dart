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
}
