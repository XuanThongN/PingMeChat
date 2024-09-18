import 'package:http/http.dart' as http;
import 'dart:convert';

import 'auth_service.dart';

class ApiClient {
  final String baseUrl;
  final AuthService authService;

  ApiClient({required this.baseUrl, required this.authService});

  Future<http.Response> get(String path) async {
    final accessToken = await authService.getAccessToken();
    final refreshToken = await authService.getRefreshToken();

    final response = await http.get(
      Uri.parse('$baseUrl$path'),
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer $accessToken',
        'X-Refresh-Token': refreshToken ?? '',
      },
    );

    if (response.statusCode == 401) {
      // Token expired, try to refresh
      await refreshTokens();
      // Retry the request
      return get(path);
    }

    return response;
  }

  Future<http.Response> post(String path, dynamic body) async {
    final accessToken = await authService.getAccessToken();
    final refreshToken = await authService.getRefreshToken();

    final response = await http.post(
      Uri.parse('$baseUrl$path'),
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer $accessToken',
        'X-Refresh-Token': refreshToken ?? '',
      },
      body: json.encode(body),
    );

    if (response.statusCode == 401) {
      // Token expired, try to refresh
      await refreshTokens();
      // Retry the request
      return post(path, body);
    }

    return response;
  }

  // Implement put, delete methods similarly

  Future<void> refreshTokens() async {
    final refreshToken = await authService.getRefreshToken();
    final response = await http.post(
      Uri.parse('$baseUrl/refresh-token'),
      headers: {'X-Refresh-Token': refreshToken ?? ''},
    );

    if (response.statusCode == 200) {
      final Map<String, dynamic> data = json.decode(response.body);
      await authService.saveTokens(data['accessToken'], data['refreshToken']);
    } else {
      // Refresh failed, user needs to login again
      await authService.clearTokens();
      throw Exception('Session expired. Please login again.');
    }
  }
}
