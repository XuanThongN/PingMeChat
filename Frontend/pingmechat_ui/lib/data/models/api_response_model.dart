class ApiResponse<T> {
  final String message;
  final T result;
  final int statusCode;

  ApiResponse({required this.message, required this.result, required this.statusCode});

  factory ApiResponse.fromJson(Map<String, dynamic> json, T Function(dynamic) fromJson) {
    return ApiResponse(
      message: json['message'] ?? '',
      result: fromJson(json['result'] ?? []), // Xử lý kết quả trống
      statusCode: json['statusCode'] ?? 0,
    );
  }
}
