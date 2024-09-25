import 'package:flutter/material.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:flutter_gen/gen_l10n/app_localizations.dart';
import 'package:pingmechat_ui/data/datasources/file_upload_service.dart';
import 'package:pingmechat_ui/presentation/pages/home.dart';
import 'package:pingmechat_ui/presentation/pages/register_page.dart';
import 'package:pingmechat_ui/providers/auth_provider.dart';
import 'package:pingmechat_ui/providers/chat_provider.dart';
import 'package:pingmechat_ui/providers/contact_provider.dart';
import 'package:pingmechat_ui/splash_screen.dart';
import 'package:provider/provider.dart';
import 'package:pingmechat_ui/data/datasources/chat_service.dart';

import 'config/theme.dart';
import 'data/datasources/chat_hub_service.dart';

import 'presentation/pages/login_page.dart';

void main() {
  runApp(
    MultiProvider(
      providers: [
        ChangeNotifierProvider(
            create: (_) => AuthProvider()), // Initialize AuthProvider
        ProxyProvider<AuthProvider, ChatHubService>(
          update: (context, authProvider, previous) => ChatHubService(authProvider),
        ),
        ProxyProvider2<AuthProvider, ChatHubService, ChatService>(
          update: (context, authProvider, chatHubService, previous) =>
              ChatService(
            chatHubService: chatHubService,
            authProvider: authProvider,
          ),
        ),
        ChangeNotifierProxyProvider<ChatService, ChatProvider>(
          create: (context) => ChatProvider(context.read<ChatService>()),
          update: (context, chatService, previous) =>
              previous ?? ChatProvider(chatService),
        ),
        ChangeNotifierProxyProvider<AuthProvider, ContactProvider>(
          create: (context) => ContactProvider(context.read<AuthProvider>()),
          update: (context, authProvider, previous) =>
              previous ?? ContactProvider(authProvider),
        )

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
      theme: appTheme,
      debugShowCheckedModeBanner: false,
      home: const OnboardingScreen(),
      routes: {
        LoginPage.routeName: (context) => const LoginPage(),
        RegisterPage.routeName: (context) => const RegisterPage(),
        HomePage.routeName: (context) => const HomePage(),
      },
      localizationsDelegates: [
        AppLocalizations.delegate,
        GlobalMaterialLocalizations.delegate,
        GlobalWidgetsLocalizations.delegate,
        GlobalCupertinoLocalizations.delegate,
      ],
      supportedLocales: const [
        Locale('en', ''), // English
        Locale('vi', ''), // Vietnamese
      ],
    );
  }
}
