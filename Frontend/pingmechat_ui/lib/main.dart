import 'package:flutter/material.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:flutter_gen/gen_l10n/app_localizations.dart';
import 'package:pingmechat_ui/data/repositories/chat_repository_impl.dart';
import 'package:pingmechat_ui/presentation/pages/home.dart';
import 'package:pingmechat_ui/providers/chat_provider.dart';
import 'package:provider/provider.dart';
import 'package:pingmechat_ui/data/datasources/chat_service.dart';

import 'config/theme.dart';
import 'data/datasources/chat_hub_service.dart';
import 'data/datasources/chat_remote_datasource.dart';
import 'package:http/http.dart' as http;

void main() {
  runApp(
    MultiProvider(
      providers: [
        Provider<ChatService>(
          create: (_) => ChatService(
            chatRepository: ChatRepositoryImpl(
              ChatRemoteDataSource(
                client: http.Client(),
                baseUrl: 'https://localhost:7043',
                chatHubService: ChatHubService(),
              ),
            ),
            chatHubService: ChatHubService(),
          ), // Initialize ChatService
        ),
        ChangeNotifierProxyProvider<ChatService, ChatProvider>(
          create: (context) => ChatProvider(context.read<ChatService>()),
          update: (context, chatService, previous) =>
              previous ?? ChatProvider(chatService),
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
      theme: appTheme,
      debugShowCheckedModeBanner: false,
      home: const HomePage(),
      // home: const Scaffold(
      //   body: OnboardingScreen(),
      // ),
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
