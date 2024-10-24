import 'package:firebase_core/firebase_core.dart';
import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/material.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:pingmechat_ui/data/datasources/search_service.dart';
import 'package:pingmechat_ui/presentation/pages/home.dart';
import 'package:pingmechat_ui/presentation/pages/register_page.dart';
import 'package:pingmechat_ui/presentation/pages/search_page.dart';
import 'package:pingmechat_ui/providers/auth_provider.dart';
import 'package:pingmechat_ui/providers/chat_provider.dart';
import 'package:pingmechat_ui/providers/contact_provider.dart';
import 'package:pingmechat_ui/splash_screen.dart';
import 'package:provider/provider.dart';
import 'package:pingmechat_ui/data/datasources/chat_service.dart';

import 'config/theme.dart';

import 'data/datasources/file_upload_service.dart';
import 'data/datasources/notification_service.dart';
import 'presentation/pages/forgot_password_page.dart';
import 'presentation/pages/login_page.dart';
import 'presentation/pages/verify_code_page.dart';
import 'providers/badge_provider.dart';
import 'providers/search_provider.dart';

final GlobalKey<NavigatorState> navigatorKey = GlobalKey<NavigatorState>();

Future<void> _firebaseMessagingBackgroundHandler(RemoteMessage message) async {
  await Firebase.initializeApp();
  print("Handling a background message: ${message.messageId}");
}

void main() async {
  WidgetsFlutterBinding.ensureInitialized();

  // Khởi tạo Firebase
  await Firebase.initializeApp();

  FirebaseMessaging.onBackgroundMessage(_firebaseMessagingBackgroundHandler);

  FirebaseMessaging messaging = FirebaseMessaging.instance;

  NotificationSettings settings = await messaging.requestPermission(
    alert: true,
    announcement: false,
    badge: true,
    carPlay: false,
    criticalAlert: false,
    provisional: false,
    sound: true,
  );

  print('User granted permission: ${settings.authorizationStatus}');

  FirebaseMessaging.onMessage.listen((RemoteMessage message) {
    print('Got a message whilst in the foreground!');
    print('Message data: ${message.data}');

    if (message.notification != null) {
      print('Message also contained a notification: ${message.notification}');
    }
  });

  // Khoi tao notification service
  NotificationService notificationService = NotificationService();
  await notificationService.initialize();

  // Khoi tao auth provider
  final authProvider = AuthProvider();

  final chatService = ChatService(authProvider: authProvider);
  final fileUploadService = ChunkedUploader(
      authProvider: authProvider); // Khởi tạo file upload service

  runApp(
    MultiProvider(
      providers: [
        ChangeNotifierProvider(create: (context) => BadgeProvider()),
        ChangeNotifierProvider(create: (_) => authProvider),
        Provider<ChatService>.value(value: chatService),
        Provider<ChunkedUploader>.value(value: fileUploadService),
        ChangeNotifierProxyProvider<ChatService, ChatProvider>(
          create: (_) => ChatProvider(chatService,
              fileUploadService), // Dùng để tạo một instance mới của ChatProvider với chatService và fileUploadService
          update: (_, chatService, previous) =>
              previous ??
              ChatProvider(chatService,
                  fileUploadService), // Dùng để cập nhật lại ChatProvider nếu đã tồn tại
        ),
        ChangeNotifierProxyProvider3<AuthProvider, BadgeProvider, ChatProvider,
            ContactProvider>(
          create: (context) => ContactProvider(authProvider,
              context.read<BadgeProvider>(), context.read<ChatProvider>()),
          update:
              (context, authProvider, badgeProvider, chatProvider, previous) =>
                  previous ??
                  ContactProvider(authProvider, badgeProvider, chatProvider),
        ),
        ChangeNotifierProxyProvider<AuthProvider, SearchProvider>(
          create: (context) =>
              SearchProvider(SearchService(authProvider: authProvider)),
          update: (context, authProvider, previous) =>
              previous ??
              SearchProvider(SearchService(authProvider: authProvider)),
        ),
      ],
      child: const MyApp(),
    ),
  );
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      navigatorKey: navigatorKey,
      theme: appTheme,
      debugShowCheckedModeBanner: false,
      home: const OnboardingScreen(),
      routes: {
        OnboardingScreen.routeName: (context) => const OnboardingScreen(),
        LoginPage.routeName: (context) => const LoginPage(),
        RegisterPage.routeName: (context) => const RegisterPage(),
        HomePage.routeName: (context) => const HomePage(),
        SearchResultsScreen.routeName: (context) => SearchResultsScreen(),
        ForgotPasswordPage.routeName: (context) => ForgotPasswordPage(),
      },
      // localizationsDelegates: [
      //   AppLocalizations.delegate,
      //   GlobalMaterialLocalizations.delegate,
      //   GlobalWidgetsLocalizations.delegate,
      //   GlobalCupertinoLocalizations.delegate,
      // ],
      supportedLocales: const [
        Locale('en', ''), // English
        Locale('vi', ''), // Vietnamese
      ],
    );
  }
}
