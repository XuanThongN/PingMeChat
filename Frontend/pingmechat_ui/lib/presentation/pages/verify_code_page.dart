import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../config/theme.dart';
import '../../providers/auth_provider.dart';
import '../widgets/custom_button.dart';
import 'login_page.dart';

class VerifyCodePage extends StatefulWidget {
  final String email;

  const VerifyCodePage({Key? key, required this.email}) : super(key: key);

  @override
  _VerifyCodePageState createState() => _VerifyCodePageState();
}

class _VerifyCodePageState extends State<VerifyCodePage> {
  final List<TextEditingController> _controllers =
      List.generate(6, (_) => TextEditingController());
  bool _isLoading = false;
  bool _isVerified = false;
  late AuthProvider _authProvider;

  @override
  void initState() {
    super.initState();
    _authProvider = Provider.of<AuthProvider>(context, listen: false);
  }

  Future<void> _verifyCode() async {
    setState(() {
      _isLoading = true;
    });

    final code = _controllers.map((controller) => controller.text).join();
    try {
      final response = await _authProvider.verifyCode(widget.email, code);
      setState(() {
        _isVerified = response;
        _isLoading = false;
      });
    } catch (e) {
      setState(() {
        _isLoading = false;
      });
    }
  }

  void _resendCode() async {
    await _authProvider.sendVerificationCode(widget.email);
    ScaffoldMessenger.of(context)
        .showSnackBar(SnackBar(content: Text('Verification code resent')));
  }

  @override
  Widget build(BuildContext context) {
    if (_isVerified) {
      WidgetsBinding.instance.addPostFrameCallback((_) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Email verified successfully!')),
        );
        Navigator.of(context).pushReplacementNamed(LoginPage.routeName);
      });
      return Container(); // This will be replaced immediately
    }

    return Scaffold(
      appBar: AppBar(title: Text('Verify Code')),
      body: Center(
        child: _isLoading
            ? CircularProgressIndicator()
            : Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Text('Enter the 6-digit code sent to your email',
                      style: TextStyle(fontSize: 16)),
                  SizedBox(height: 20),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                    children: List.generate(6, (index) {
                      return _buildCodeInputField(index);
                    }),
                  ),
                  SizedBox(height: 20),
                  CustomElevatedButton(
                    text: 'Verify',
                    onPressed: _verifyCode,
                    foregroundColor: AppColors.white,
                    backgroundColor: AppColors.primary,
                  ),
                  SizedBox(height: 20),
                  TextButton.icon(
                    onPressed: _resendCode,
                    icon: Icon(Icons.refresh, color: AppColors.primary),
                    label: const Text(
                      'Resend Code',
                      style: TextStyle(
                        color: AppColors.primary,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                    style: TextButton.styleFrom(
                      padding:
                          EdgeInsets.symmetric(horizontal: 16, vertical: 8),
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(20),
                        side: BorderSide(color: AppColors.primary),
                      ),
                    ),
                  ),
                ],
              ),
      ),
    );
  }

  Widget _buildCodeInputField(int index) {
    return Container(
      width: 40,
      height: 50,
      margin: EdgeInsets.symmetric(horizontal: 5),
      decoration: BoxDecoration(
        color: Colors.grey[200],
        borderRadius: BorderRadius.circular(10),
        boxShadow: [
          BoxShadow(
            color: Colors.grey.withOpacity(0.3),
            spreadRadius: 1,
            blurRadius: 3,
            offset: Offset(0, 2),
          ),
        ],
      ),
      child: TextField(
        controller: _controllers[index],
        decoration: InputDecoration(
          border: InputBorder.none,
          counterText: "",
        ),
        style: TextStyle(fontSize: 24, fontWeight: FontWeight.bold),
        textAlign: TextAlign.center,
        keyboardType: TextInputType.number,
        maxLength: 1,
        onChanged: (value) {
          if (value.length == 1 && index < 5) {
            FocusScope.of(context).nextFocus();
          } else if (value.isEmpty && index > 0) {
            FocusScope.of(context).previousFocus();
          }
        },
      ),
    );
  }
}
