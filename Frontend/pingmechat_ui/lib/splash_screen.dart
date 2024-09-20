import 'package:flutter/material.dart';
import 'package:pingmechat_ui/config/theme.dart';
import 'package:pingmechat_ui/presentation/pages/login_page.dart';
import 'package:pingmechat_ui/presentation/pages/register_page.dart';
import 'package:pingmechat_ui/presentation/widgets/custom_button.dart';
import 'package:pingmechat_ui/presentation/widgets/custom_divider.dart';
import 'package:pingmechat_ui/presentation/widgets/social_button.dart';
import 'package:provider/provider.dart';

import 'presentation/pages/home.dart';
import 'providers/auth_provider.dart';

class OnboardingScreen extends StatefulWidget {
  const OnboardingScreen({super.key});

  @override
  State<OnboardingScreen> createState() => _OnboardingScreenState();
}

class _OnboardingScreenState extends State<OnboardingScreen> {

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance?.addPostFrameCallback((_) {
      _checkLoginStatus();
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      body: SafeArea(
        child: SingleChildScrollView(
          padding: const EdgeInsets.fromLTRB(24, 16, 24, 32),
          child: Stack(
            children: [
              Transform.rotate(
                angle: 180,
                // Convert degrees to radians
                child: Container(
                  width: 800,
                  height: 400,
                  decoration: BoxDecoration(
                    shape: BoxShape.rectangle,
                    borderRadius: BorderRadius.circular(180),
                    // Half of height to make it ellipse
                    gradient: LinearGradient(
                      colors: [
                        Color(0xFF43116A).withOpacity(0),
                        Color(0xFF0A1832).withOpacity(1),
                      ],
                      begin: Alignment.bottomLeft,
                      end: Alignment.topRight,
                    ),
                  ),
                ),
              ),
              Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Padding(
                    padding: EdgeInsets.only(top: 20.0),
                    child: Center(
                      child: Text(
                        'Ping Me Chat',
                        style: AppTypography.subH2,
                      ),
                    ),
                  ),
                  RichText(
                    text: const TextSpan(
                      children: [
                        TextSpan(
                          text: 'Connect\nfriends\n',
                          style: TextStyle(
                            color: Colors.white,
                            fontSize: 50,
                            fontFamily: 'Caros',
                            letterSpacing: 5,
                          ),
                        ),
                        TextSpan(
                          text: 'easily &\nquickly',
                          style: TextStyle(
                            color: Colors.white,
                            fontSize: 60,
                            fontFamily: 'Caros',
                            fontWeight: FontWeight.w900,
                            letterSpacing: 5,
                          ),
                        ),
                      ],
                    ),
                  ),
                  const Padding(
                    padding: EdgeInsets.only(top: 20.0),
                    child: Text(
                      'Our chat app is the perfect way to stay\nconnected with friends and family.',
                      style: TextStyle(
                        color: AppColors.tertiary,
                        fontSize: 16,
                        height: 1.5,
                        // letterSpacing: ,
                      ),
                    ),
                  ),
                  Padding(
                    padding: const EdgeInsets.symmetric(
                        vertical: 20.0, horizontal: 10.0),
                    child: ListSocialButtons(),
                  ),
                  const Padding(
                    padding: EdgeInsets.only(bottom: 10.0),
                    child: CustomDivider(),
                  ),
                  CustomElevatedButton(
                    text: 'Sign up with mail',
                    backgroundColor: AppColors.white,
                    foregroundColor: AppColors.secondary,
                    onPressed: () {
                      Navigator.push(
                        context,
                        MaterialPageRoute(
                            builder: (context) => const RegisterPage()),
                      );
                    },
                  ),
                  Center(
                    child: TextButton(
                      onPressed: () {
                        Navigator.push(
                          context,
                          MaterialPageRoute(
                              builder: (context) => const LoginPage()),
                        );
                      },
                      child: Row(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          Text(
                            'Existing account ?',
                            style: AppTypography.subH3,
                          ),
                          Padding(
                            padding: EdgeInsets.only(left: 8),
                            child: Text(
                              'Log in',
                              style: AppTypography.subH3.copyWith(
                                color: AppColors.white,
                              ),
                            ),
                          ),
                        ],
                      ),
                    ),
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }

  Future<void> _checkLoginStatus() async {
    final isAuth =
        await Provider.of<AuthProvider>(context, listen: false).tryAutoLogin();
    if (isAuth) {
      Navigator.of(context).pushReplacementNamed(HomePage.routeName);
    } else {
      Navigator.of(context).pushReplacementNamed(LoginPage.routeName);
    }
  }
}
