import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:shared_preferences/shared_preferences.dart';

import '../core/constants/constant.dart';
import '../domain/models/account.dart';

class AuthProvider with ChangeNotifier {
  String? _accessToken;
  String? _refreshToken;
  DateTime? _expiryDate;
  String? _userId;
  Account? _currentUser;

  // Hàm để gọi khi đăng nhập thành công
  Function? onLoginSuccess;

  bool get isAuth {
    return _accessToken != null;
  }

  String? get accessToken {
    if (_expiryDate != null &&
        _expiryDate!.isAfter(DateTime.now()) &&
        _accessToken != null) {
      return _accessToken;
    }
    return null;
  }

  String? get refreshToken {
    return _refreshToken;
  }

  String? get userId {
    return _userId;
  }

  Account? get currentUser {
    return _currentUser;
  }

  Future<bool> signup(String email, String userName, String password,
      String confirmPassword, String fullName, String phone) async {
    try {
      final response = await http.post(
        Uri.parse(ApiConstants.registerEndpoint),
        body: json.encode({
          'email': email,
          'userName': userName,
          'password': password,
          'confirmPassword': confirmPassword,
          'fullName': fullName,
          'phone': phone,
        }),
        headers: {'Content-Type': 'application/json'},
      );

      final responseData = json.decode(response.body);
      if (responseData['statusCode'] != 201) {
        throw Exception(responseData['message']);
      }

      // Đăng ký thành công
      return true;

      // Đăng ký thành công thì chuyển hướng về trang đăng nhập
    } catch (error) {
      print('Signup error: $error');
      return false; //Đăng ký thất bại
    }
  }

  Future<bool> login(String email, String password) async {
    try {
      final response = await http.post(
        Uri.parse(ApiConstants.loginEndpoint),
        body: json.encode({
          'userName': email,
          'password': password,
        }),
        headers: {'Content-Type': 'application/json'},
      );

      final responseData = json.decode(response.body);
      if (responseData['statusCode'] != 200) {
        throw Exception(responseData['message']);
      }

      _accessToken = responseData['result']['accessToken'];
      _refreshToken = responseData['result']['refreshToken'];
      _expiryDate = DateTime.parse(responseData['result']['tokenExpiresIn']);
      _userId = responseData['result']['userId'];
      _currentUser = Account(
        id: responseData['result']['userId'],
        email: responseData['result']['email'],
        fullName: responseData['result']['fullName'],
      );

      notifyListeners();
      await _saveUserData();

// Call the onLoginSuccess callback if it's set
      onLoginSuccess?.call();
      return true; //Đăng nhập thành công
    } catch (error) {
      print('Login error: $error');
      return false; //Đăng nhập thất bại
    }
  }

  Future<bool> tryAutoLogin() async {
    final prefs = await SharedPreferences.getInstance();
    if (!prefs.containsKey('userData')) {
      return false;
    }
    final extractedUserData =
        json.decode(prefs.getString('userData')!) as Map<String, dynamic>;
    final expiryDate =
        DateTime.parse(extractedUserData['expiryDate'] as String);

    if (expiryDate.isBefore(DateTime.now())) {
      return false;
    }

    _accessToken = extractedUserData['accessToken'] as String;
    _refreshToken = extractedUserData['refreshToken'] as String;
    _userId = extractedUserData['userId'] as String;
    _expiryDate = expiryDate;
    _currentUser = Account(
      id: _userId as String,
      email: extractedUserData['email'] as String,
      fullName: extractedUserData['fullName'] as String,
    );
    notifyListeners();
    return true;
  }

  Future<void> logout() async {
    notifyLogout();
    final prefs = await SharedPreferences.getInstance();
    prefs.clear();
  }

  Future<void> _saveUserData() async {
    final prefs = await SharedPreferences.getInstance();
    final userData = json.encode({
      'accessToken': _accessToken,
      'refreshToken': _refreshToken,
      'userId': _userId,
      'expiryDate': _expiryDate!.toIso8601String(),
      'email': _currentUser!.email,
      'fullName': _currentUser!.fullName,
    });
    prefs.setString('userData', userData);
  }

  Future<Map<String, String>> getCustomHeaders() async {
    return {
      'Authorization': 'Bearer $_accessToken',
      'RefreshToken': _refreshToken!,
    };
  }

  Future<String> getAuthorizationString() async {
    return 'Bearer $_accessToken,$_refreshToken';
  }

  void notifyLogout() {
    _accessToken = null;
    _refreshToken = null;
    _userId = null;
    _expiryDate = null;
    _currentUser = null;
    notifyListeners();
  }
}
