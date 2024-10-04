import 'dart:async';
import 'dart:convert';
import 'dart:io';
import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:mime/mime.dart';
import 'package:http_parser/http_parser.dart';
import 'package:provider/provider.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../core/constants/constant.dart';
import '../domain/models/account.dart';
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
        headers: {'Content-Type': 'application/json; charset=UTF-8'},
      ).timeout(const Duration(seconds: 10)); // Thời gian chờ là 10 giây

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
        phoneNumber: responseData['result']['phoneNumber'],
        userName: responseData['result']['userName'],
        avatarUrl: responseData['result']['avatarUrl'],
        fullName: responseData['result']['fullName'],
      );

      // After successful login, get and send FCM token to server
      String? fcmToken = await FirebaseMessaging.instance.getToken();
      if (fcmToken != null) {
        await _sendFCMTokenToServer(fcmToken);
      }

      notifyListeners();
      await _handleSuccessfulLogin();
      return true; // Đăng nhập thành công
    } on TimeoutException catch (_) {
      print('Login request timed out');
      return false; // Đăng nhập thất bại do hết thời gian chờ
    } catch (error) {
      print('Login error: $error');
      return false; // Đăng nhập thất bại
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
      phoneNumber: extractedUserData['phoneNumber'] as String,
      userName: extractedUserData['userName'] as String,
      avatarUrl: extractedUserData['avatarUrl'] as String,
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

  // Thêm hàm gửi mã xác thực
  Future<void> sendVerificationCode(String email) async {
    try {
      final response = await http.post(
        Uri.parse(ApiConstants.reSendVerificationCodeEndpoint),
        body: json.encode(email),
        headers: {
          'content-type': 'application/json; charset=utf-8',
        },
      );

      final responseData = json.decode(response.body);
      if (responseData['statusCode'] != 200) {
        throw Exception(responseData['message']);
      }
    } catch (error) {
      print('Send verification code error: $error');
    }
  }

  // Thêm hàm xác thực mã
  Future<bool> verifyCode(String email, String code) async {
    try {
      final response = await http.post(
        Uri.parse(ApiConstants.verifyCodeEndpoint),
        body: json.encode({
          'email': email,
          'code': code,
        }),
        headers: {'Content-Type': 'application/json'},
      );

      final responseData = json.decode(response.body);
      if (responseData['statusCode'] != 200) {
        throw Exception(responseData['message']);
      }

      return true;
    } catch (error) {
      print('Verify code error: $error');
      return false;
    }
  }

  Future<bool> forgotPassword(String email) async {
    try {
      final response = await http.post(
        Uri.parse(ApiConstants.forgotPasswordEndpoint),
        body: json.encode(email),
        headers: {'Content-Type': 'application/json'},
      );

      final responseData = json.decode(response.body);
      if (responseData['statusCode'] != 200) {
        throw Exception(responseData['message']);
      }

      return true;
    } catch (error) {
      print('Forgot password error: $error');
      return false;
    }
  }

  Future<bool> verifyResetCode(String email, String code) async {
    try {
      final response = await http.post(
        Uri.parse(ApiConstants.verifyResetCodeEndpoint),
        body: json.encode({
          'email': email,
          'code': code,
        }),
        headers: {'Content-Type': 'application/json'},
      );

      final responseData = json.decode(response.body);
      if (responseData['statusCode'] != 200) {
        throw Exception(responseData['message']);
      }

      return true;
    } catch (error) {
      print('Verify reset code error: $error');
      return false;
    }
  }

  Future<bool> resetPassword(String email, String newPassword) async {
    try {
      final response = await http.post(
        Uri.parse(ApiConstants.resetPasswordEndpoint),
        body: json.encode({
          'email': email,
          'newPassword': newPassword,
        }),
        headers: {'Content-Type': 'application/json'},
      );

      final responseData = json.decode(response.body);
      if (responseData['statusCode'] != 200) {
        throw Exception(responseData['message']);
      }

      return true;
    } catch (error) {
      print('Reset password error: $error');
      return false;
    }
  }

  Future<bool> updateUserInfo(Map<String, dynamic> userInfo) async {
    try {
      final response = await http.put(
        Uri.parse(ApiConstants.updateUserInfoEndpoint),
        headers: {
          'Content-Type': 'application/json; charset=utf-8',
          ...await getCustomHeaders(),
        },
        body: json.encode(userInfo),
      );

      if (response.statusCode == 200) {
        final responseData = json.decode(response.body);
        // Update the local user data
        _currentUser = Account.fromJson(responseData['result']);
        notifyListeners();
        return true;
      } else {
        throw Exception('Failed to update user info');
      }
    } catch (error) {
      print('Update user info error: $error');
      return false;
    }
  }

  Future<bool> updateAvatar(File imageFile) async {
    try {
      var request = http.MultipartRequest(
        'POST',
        Uri.parse(ApiConstants.updateUserAvatarEndpoint),
      );

      request.headers.addAll(await getCustomHeaders());

      final mimeType = lookupMimeType(imageFile.path);
      request.files.add(await http.MultipartFile.fromPath(
        'avatar',
        imageFile.path,
        contentType: mimeType != null ? MediaType.parse(mimeType) : null,
      ));

      var response = await request.send();

      if (response.statusCode == 200) {
        final responseData = json.decode(await response.stream.bytesToString());
        // Update the local user data
        _currentUser = Account.fromJson(responseData['result']);
        notifyListeners();
        return true;
      } else {
        throw Exception('Failed to update avatar');
      }
    } catch (error) {
      print('Update avatar error: $error');
      return false;
    }
  }
}
