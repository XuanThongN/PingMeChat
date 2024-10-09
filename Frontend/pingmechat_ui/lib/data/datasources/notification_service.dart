import 'dart:ui';

import 'package:flutter/material.dart';
import 'package:flutter_local_notifications/flutter_local_notifications.dart';
import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';
import 'dart:typed_data';
import 'dart:ui' as ui;

import '../../main.dart';
import '../../presentation/pages/chat_page.dart';

class NotificationService {
  final FlutterLocalNotificationsPlugin _flutterLocalNotificationsPlugin =
      FlutterLocalNotificationsPlugin();

  Future<void> initialize() async {
    const AndroidInitializationSettings initializationSettingsAndroid =
        AndroidInitializationSettings('@mipmap/ic_launcher');

    final InitializationSettings initializationSettings =
        InitializationSettings(android: initializationSettingsAndroid);

    await _flutterLocalNotificationsPlugin.initialize(initializationSettings);

    FirebaseMessaging.onMessage.listen(_handleMessage);
    FirebaseMessaging.onMessageOpenedApp.listen(_handleMessageOpenedApp);
  }

  void _handleMessage(RemoteMessage message) async {
    RemoteNotification? notification = message.notification;
    AndroidNotification? android = message.notification?.android;

    if (notification != null && android != null) {
      // Tải avatar từ URL và bo tròn nó
      final String avatarUrl = message.data['avatarUrl'];
      final ByteData avatarData = await _loadAvatar(avatarUrl);
      final ByteData roundedAvatarData = await _getRoundedAvatar(avatarData);

      final InboxStyleInformation inboxStyleInformation = InboxStyleInformation(
        [notification.body ?? ''],
        contentTitle: notification.title,
      );

      final AndroidNotificationDetails androidPlatformChannelSpecifics =
          AndroidNotificationDetails(
        'chat_messages_channel', // Thay thế bằng channelId thực tế
        'Chat Messages', // Thay thế bằng channelName thực tế
        channelDescription: 'Channel for chat message notifications',
        styleInformation: inboxStyleInformation,
        largeIcon: ByteArrayAndroidBitmap(roundedAvatarData.buffer.asUint8List()),
        importance: Importance.max,
        priority: Priority.high,
        showWhen: false,
      );

      final NotificationDetails platformChannelSpecifics =
          NotificationDetails(android: androidPlatformChannelSpecifics);

      await _flutterLocalNotificationsPlugin.show(
        notification.hashCode,
        notification.title,
        notification.body,
        platformChannelSpecifics,
      );
    }
  }

  Future<ByteData> _loadAvatar(String url) async {
    final http.Response response = await http.get(Uri.parse(url));
    if (response.statusCode == 200) {
      return ByteData.view(Uint8List.fromList(response.bodyBytes).buffer);
    } else {
      throw Exception('Failed to load avatar');
    }
  }

  Future<ByteData> _getRoundedAvatar(ByteData avatarData) async {
    final ui.Codec codec = await ui.instantiateImageCodec(avatarData.buffer.asUint8List());
    final ui.FrameInfo frameInfo = await codec.getNextFrame();
    final ui.Image image = frameInfo.image;

    final ui.PictureRecorder recorder = ui.PictureRecorder();
    final Canvas canvas = Canvas(recorder);
    final Paint paint = Paint();
    final double size = image.width.toDouble();

    paint.isAntiAlias = true;
    canvas.drawCircle(Offset(size / 2, size / 2), size / 2, paint);
    paint.blendMode = BlendMode.srcIn;
    canvas.drawImage(image, Offset.zero, paint);

    final ui.Image roundedImage = await recorder.endRecording().toImage(image.width, image.height);
    final ByteData? roundedAvatarData = await roundedImage.toByteData(format: ui.ImageByteFormat.png);

    if (roundedAvatarData == null) {
      throw Exception('Failed to create rounded avatar');
    }

    return roundedAvatarData;
  }

  void _handleMessageOpenedApp(RemoteMessage message) {
    // Xử lý khi người dùng mở ứng dụng từ thông báo
    _navigateToChatScreen(message.data);
  }

  Future<void> _onSelectNotification(String? payload) async {
    if (payload != null) {
      final Map<String, dynamic> data = jsonDecode(payload);
      _navigateToChatScreen(data);
    }
  }

  void _navigateToChatScreen(Map<String, dynamic> data) {
    final String chatId = data['chatId'];
    // Điều hướng đến màn hình chat với chatId
    navigatorKey.currentState?.push(
      MaterialPageRoute(
        builder: (context) => ChatPage(chatId: chatId),
      ),
    );
  }
}