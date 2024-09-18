import 'package:flutter/material.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:flutter_gen/gen_l10n/app_localizations.dart';
import 'package:pingmechat_ui/presentation/pages/home.dart';
import 'package:pingmechat_ui/providers/chat_provider.dart';
import 'package:provider/provider.dart';

import 'config/theme.dart';

void main() {
  runApp(
    MultiProvider(providers: [
      // Add providers here
      ChangeNotifierProvider(create: (_) => ChatProvider(_chatService))
    ], child: MyApp()), // Add providers here
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
