import 'package:flutter/material.dart';
import 'package:pingmechat_ui/config/theme.dart';
import 'package:pingmechat_ui/presentation/pages/home.dart';
import 'package:pingmechat_ui/presentation/pages/login_page.dart';
import 'package:pingmechat_ui/splash_screen.dart';

import 'app.dart';

void main() {
  runApp(MyApp());
}

class MyApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false,
      home: FutureBuilder(
        future: checkLoginStatus(),
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const CircularProgressIndicator();
          } else {
            if (snapshot.data == true) {
              return HomePage();
            } else {
              return const LoginPage();
            }
          }
        },
      ),
    );
  }

  Future<bool> checkLoginStatus() async {
    // Simulate a network call
    await Future.delayed(Duration(seconds: 3));

    // Mock login status
    bool isLoggedIn = true; // Change this to false to simulate a logged-out state

    return isLoggedIn;
  }
}
