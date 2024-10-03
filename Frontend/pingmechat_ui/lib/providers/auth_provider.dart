import 'dart:convert';
import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:provider/provider.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../core/constants/constant.dart';
import '../core/constants/constant.dart';
import '../domain/models/account.dart';
import '../presentation/pages/login_page.dart';
import 'chat_provider.dart';
import 'contact_provider.dart';
import 'search_provider.dart';

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


      // After successful login, get and send FCM token to server
      String? fcmToken = await FirebaseMessaging.instance.getToken();
      if (fcmToken != null) {
        await _sendFCMTokenToServer(fcmToken);
      }

      
      notifyListeners();
      await _handleSuccessfulLogin();
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
    await _handleSuccessfulLogin();
    return true;
  }

  Future<void> _handleSuccessfulLogin() async {
    await _saveUserData();
    onLoginSuccess?.call();
    notifyListeners();
  }

  Future<void> logout(BuildContext context) async {
    notifyLogout();

    // Clear shared preferences
    final prefs = await SharedPreferences.getInstance();
    prefs.clear();

    // Notify other providers to clear their data
    final chatProvider = Provider.of<ChatProvider>(context, listen: false);
    final contactProvider =
        Provider.of<ContactProvider>(context, listen: false);
    final searchProvider = Provider.of<SearchProvider>(context, listen: false);

    chatProvider.clearData();
    contactProvider.clearData();
    searchProvider.clearData();
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

  // Firebase
  Future<void> _sendFCMTokenToServer(String fcmToken) async {
    final response = await http.post(
      Uri.parse(ApiConstants.updateFCMTokenEndpoint),
      headers: <String, String>{
        'Content-Type': 'application/json; charset=UTF-8',
        ...await getCustomHeaders(),
      },
      body: jsonEncode(<String, String>{
        'FCMToken': fcmToken,
      }),
    );

    if (response.statusCode != 200) {
      throw Exception('Failed to update FCM token');
    }
  }
}
