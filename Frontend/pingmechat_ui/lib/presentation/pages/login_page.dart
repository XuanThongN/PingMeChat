import 'package:flutter/material.dart';
import 'package:pingmechat_ui/presentation/widgets/app_bar.dart';
import 'package:pingmechat_ui/presentation/widgets/custom_divider.dart';
import 'package:pingmechat_ui/presentation/widgets/custom_text_field.dart';

import '../../config/theme.dart';
import '../widgets/custom_button.dart';
import '../widgets/social_button.dart';

class LoginPage extends StatefulWidget {
  const LoginPage({Key? key}) : super(key: key);

  @override
  State<LoginPage> createState() => _LoginPageState();
}

class _LoginPageState extends State<LoginPage> {
  final TextEditingController _emailController = TextEditingController();
  final TextEditingController _passwordController = TextEditingController();
  bool _isButtonEnabled = false;
  String? _emailError;
  String? _passwordError;
  bool _isEmailTouched = false;
  bool _isPasswordTouched = false;

  @override
  void initState() {
    super.initState();
    _emailController.addListener(_validateFields);
    _passwordController.addListener(_validateFields);
  }

  void _validateFields() {
    setState(() {
      if (_isEmailTouched) {
        _emailError = _validateEmail(_emailController.text);
      }
      if (_isPasswordTouched) {
        _passwordError = _validatePassword(_passwordController.text);
      }
      _isButtonEnabled = _emailError == null && _passwordError == null;
    });
  }

  String? _validateEmail(String email) {
    if (email.isEmpty) {
      return 'Email cannot be empty';
    }
    final emailRegex = RegExp(r'^[^@]+@[^@]+\.[^@]+');
    if (!emailRegex.hasMatch(email)) {
      return 'Enter a valid email';
    }
    return null;
  }

  String? _validatePassword(String password) {
    if (password.isEmpty) {
      return 'Password cannot be empty';
    }
    if (password.length < 8) {
      return 'Password must be at least 8 characters';
    }
    return null;
  }

  @override
  void dispose() {
    _emailController.dispose();
    _passwordController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      // backgroundColor: AppColors.white,
      appBar: CustomAppBar(),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(24.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Center(
              child: Column(
                children: [
                  Text(
                    'Log in to Chatbox',
                    style: TextStyle(
                      fontSize: 24,
                      fontWeight: FontWeight.bold,
                      color: Colors.black,
                    ),
                  ),
                  Text(
                    'Welcome back! Sign in using your social\naccount or email to continue us',
                    textAlign: TextAlign.center,
                    style: TextStyle(
                      fontSize: 16,
                      color: Colors.grey,
                    ),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 32),
            ListSocialButtons(),
            const SizedBox(height: 32),
            CustomDivider(),
            const SizedBox(height: 32),
            CustomTextField(
              label: 'Your email',
              autoFocus: true,
              controller: _emailController,
              errorText: _emailError,
              onChanged: (value) {
                setState(() {
                  _isEmailTouched = true;
                });
              },
            ),
            const SizedBox(height: 16),
            CustomTextField(
              label: 'Password',
              isPassword: true,
              controller: _passwordController,
              errorText: _passwordError,
              onChanged: (value) {
                setState(() {
                  _isPasswordTouched = true;
                });
              },
            ),
            const SizedBox(height: 32),
            CustomElevatedButton(
              text: 'Log in',
              onPressed: _isButtonEnabled
                  ? () {
                      print("Log in button pressed");
                    }
                  : () {},
              foregroundColor: AppColors.tertiary,
              backgroundColor: _isButtonEnabled
                  ? AppColors.primary
                  : AppColors.disabledColor,
            ),
            const SizedBox(height: 16),
            Center(
              child: TextButton(
                onPressed: () {},
                child: const Text(
                  'Forgot password?',
                  style: TextStyle(color: Colors.teal),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
