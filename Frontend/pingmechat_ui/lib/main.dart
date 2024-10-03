import 'package:flutter/material.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:flutter_gen/gen_l10n/app_localizations.dart';
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

import 'presentation/pages/login_page.dart';
import 'providers/badge_provider.dart';
import 'providers/search_provider.dart';

final GlobalKey<NavigatorState> navigatorKey = GlobalKey<NavigatorState>();
void main() async {
  WidgetsFlutterBinding.ensureInitialized();

  final authProvider = AuthProvider();

  final chatService = ChatService(authProvider: authProvider);

  runApp(
    MultiProvider(
      providers: [
        ChangeNotifierProvider(create: (context) => BadgeProvider()),
        ChangeNotifierProvider(create: (_) => authProvider),
        Provider<ChatService>.value(value: chatService),
        ChangeNotifierProxyProvider<ChatService, ChatProvider>(
          create: (_) => ChatProvider(chatService),
          update: (_, chatService, previous) =>
              previous ?? ChatProvider(chatService),
        ),
        ChangeNotifierProxyProvider2<AuthProvider, BadgeProvider, ContactProvider>(
          create: (context) => ContactProvider(authProvider, context.read<BadgeProvider>()),
          update: (context, authProvider, badgeProvider, previous) =>
              previous ?? ContactProvider(authProvider, badgeProvider),
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

// void showIncomingCallDialog(BuildContext context, String callerId,
//     String chatId, bool isVideoCall, CallProvider callProvider) {
//   showDialog(
//     context: context,
//     barrierDismissible: false,
//     builder: (BuildContext context) {
//       return AlertDialog(
//         title: Text('Incoming ${isVideoCall ? 'Video' : 'Audio'} Call'),
//         content: Text('Call from $callerId'),
//         actions: <Widget>[
//           TextButton(
//             child: Text('Reject'),
//             onPressed: () {
//               callProvider.endCall();
//               Navigator.of(context).pop();
//             },
//           ),
//           TextButton(
//             child: Text('Accept'),
//             onPressed: () {
//               callProvider.acceptCall();
//               Navigator.of(context).pop();
//               Navigator.push(
//                 context,
//                 MaterialPageRoute(
//                   builder: (context) => CallPage(
//                     chatId: chatId,
//                     isVideo: isVideoCall,
//                     onEndCall: () {
//                       callProvider.endCall();
//                       Navigator.pop(context);
//                     },
//                   ),
//                 ),
//               );
//             },
//           ),
//         ],
//       );
//     },
//   );
// }

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
