import 'package:flutter/material.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:flutter_gen/gen_l10n/app_localizations.dart';
import 'package:pingmechat_ui/data/datasources/file_upload_service.dart';
import 'package:pingmechat_ui/data/datasources/search_service.dart';
import 'package:pingmechat_ui/presentation/pages/home.dart';
import 'package:pingmechat_ui/presentation/pages/register_page.dart';
import 'package:pingmechat_ui/presentation/pages/search_page.dart';
import 'package:pingmechat_ui/providers/auth_provider.dart';
import 'package:pingmechat_ui/providers/call_provider.dart';
import 'package:pingmechat_ui/providers/chat_provider.dart';
import 'package:pingmechat_ui/providers/contact_provider.dart';
import 'package:pingmechat_ui/splash_screen.dart';
import 'package:provider/provider.dart';
import 'package:pingmechat_ui/data/datasources/chat_service.dart';

import 'config/theme.dart';
import 'data/datasources/chat_hub_service.dart';

import 'presentation/pages/call_page.dart';
import 'presentation/pages/login_page.dart';
import 'providers/search_provider.dart';

final GlobalKey<NavigatorState> navigatorKey = GlobalKey<NavigatorState>();
void main() {
  runApp(
    MultiProvider(
      providers: [
        ChangeNotifierProvider(
            create: (_) => AuthProvider()), // Initialize AuthProvider
        ProxyProvider<AuthProvider, ChatHubService>(
          update: (context, authProvider, previous) =>
              ChatHubService(authProvider),
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
        ),
        ChangeNotifierProxyProvider<ChatHubService, CallProvider>(
          create: (context) => CallProvider(context.read<ChatHubService>()),
          update: (context, chatHubService, previous) {
            final callProvider = previous ?? CallProvider(chatHubService);
            // Không cần set onIncomingCall callback ở đây nữa
            return callProvider;
          },
        ),
        ChangeNotifierProvider(
          create: (context) => SearchProvider(
              SearchService(authProvider: context.read<AuthProvider>())),
          child: SearchResultsScreen(),
        ),
      ],
      child: const MyApp(),
    ),
  );
}

void showIncomingCallDialog(BuildContext context, String callerId, String chatId, bool isVideoCall, CallProvider callProvider) {
  showDialog(
    context: context,
    barrierDismissible: false,
    builder: (BuildContext context) {
      return AlertDialog(
        title: Text('Incoming ${isVideoCall ? 'Video' : 'Audio'} Call'),
        content: Text('Call from $callerId'),
        actions: <Widget>[
          TextButton(
            child: Text('Reject'),
            onPressed: () {
              callProvider.endCall();
              Navigator.of(context).pop();
            },
          ),
          TextButton(
            child: Text('Accept'),
            onPressed: () {
              callProvider.acceptIncomingCall(chatId, isVideoCall);
              Navigator.of(context).pop();
              Navigator.push(
                context,
                MaterialPageRoute(
                  builder: (context) => CallPage(
                    chatId: chatId,
                    isVideo: isVideoCall,
                    localRenderer: callProvider.localRenderer,
                    remoteRenderer: callProvider.remoteRenderer,
                    onEndCall: () {
                      callProvider.endCall();
                      Navigator.pop(context);
                    },
                  ),
                ),
              );
            },
          ),
        ],
      );
    },
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
