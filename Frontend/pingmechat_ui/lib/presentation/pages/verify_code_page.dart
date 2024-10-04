import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../providers/auth_provider.dart';

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
      final response = await ApiService.verifyCode(widget.email, code);
      setState(() {
        _isVerified = response['success'];
        _isLoading = false;
      });
    } catch (e) {
      setState(() {
        _isLoading = false;
      });
    }
  }

  void _resendCode() async {
    await ApiService.sendVerificationCode(widget.email);
    ScaffoldMessenger.of(context)
        .showSnackBar(SnackBar(content: Text('Verification code resent')));
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text('Verify Code')),
      body: Center(
        child: _isLoading
            ? CircularProgressIndicator()
            : _isVerified
                ? Text('Email verified successfully!')
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
                      ElevatedButton(
                        onPressed: _verifyCode,
                        child: Text('Verify'),
                      ),
                      TextButton(
                        onPressed: _resendCode,
                        child: Text('Resend Code'),
                      ),
                    ],
                  ),
      ),
    );
  }

  Widget _buildCodeInputField(int index) {
    return Container(
      width: 40,
      child: TextField(
        controller: _controllers[index],
        decoration: InputDecoration(
          border: OutlineInputBorder(),
        ),
        textAlign: TextAlign.center,
        keyboardType: TextInputType.number,
        maxLength: 1,
        onChanged: (value) {
          if (value.length == 1 && index < 5) {
            FocusScope.of(context).nextFocus();
          }
        },
      ),
    );
  }
}
