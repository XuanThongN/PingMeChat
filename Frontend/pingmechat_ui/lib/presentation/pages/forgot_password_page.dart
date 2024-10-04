import 'package:flutter/material.dart';
import 'package:pingmechat_ui/presentation/widgets/app_bar.dart';
import 'package:provider/provider.dart';
import '../../providers/auth_provider.dart';
import '../../core/utils/input_validator.dart';
import '../widgets/custom_text_field.dart';
import '../widgets/custom_button.dart';
import '../../config/theme.dart';

class ForgotPasswordPage extends StatefulWidget {
  static const routeName = '/forgot-password';

  @override
  _ForgotPasswordPageState createState() => _ForgotPasswordPageState();
}

class _ForgotPasswordPageState extends State<ForgotPasswordPage> {
  final _formKey = GlobalKey<FormState>();
  final _emailController = TextEditingController();
  final _codeController = TextEditingController();
  final _newPasswordController = TextEditingController();
  final _confirmPasswordController = TextEditingController();

  bool _codeSent = false;
  bool _codeVerified = false;
  bool _isLoading = false;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CustomAppBar(
        title: 'Forgot Password',
        backgroundColor: AppColors.primary,
        textColor: AppColors.white,
      ),
      body: Stack(
        children: [
          SingleChildScrollView(
            padding: EdgeInsets.all(16),
            child: Form(
              key: _formKey,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  if (!_codeSent) _buildEmailField(),
                  if (_codeSent && !_codeVerified) _buildCodeField(),
                  if (_codeVerified) ...[
                    _buildNewPasswordField(),
                    SizedBox(height: 16),
                    _buildConfirmPasswordField(),
                  ],
                  SizedBox(height: 24),
                  CustomElevatedButton(
                    text: _getButtonText(),
                    onPressed: _isLoading ? () {} : _handleSubmit,
                    backgroundColor: AppColors.primary,
                    foregroundColor: Colors.white,
                  ),
                ],
              ),
            ),
          ),
          if (_isLoading)
            Container(
              color: Colors.black.withOpacity(0.5),
              child: Center(
                child: CircularProgressIndicator(
                  valueColor: AlwaysStoppedAnimation<Color>(AppColors.primary),
                ),
              ),
            ),
        ],
      ),
    );
  }

  Widget _buildEmailField() {
    return CustomTextField(
      label: 'Email',
      controller: _emailController,
      validator: InputValidator.validateEmail,
      keyboardType: TextInputType.emailAddress,
    );
  }

  Widget _buildCodeField() {
    return CustomTextField(
      label: 'Verification Code',
      controller: _codeController,
      validator: InputValidator.validateCode,
      keyboardType: TextInputType.number,
    );
  }

  Widget _buildNewPasswordField() {
    return CustomTextField(
      label: 'New Password',
      controller: _newPasswordController,
      validator: InputValidator.validatePassword,
      isPassword: true,
    );
  }

  Widget _buildConfirmPasswordField() {
    return CustomTextField(
      label: 'Confirm New Password',
      controller: _confirmPasswordController,
      validator: (value) => InputValidator.validateConfirmPassword(
        value,
        _newPasswordController.text,
      ),
      isPassword: true,
    );
  }

  String _getButtonText() {
    if (!_codeSent) return 'Send Reset Code';
    if (!_codeVerified) return 'Verify Code';
    return 'Reset Password';
  }

  void _handleSubmit() async {
    if (_formKey.currentState!.validate()) {
      setState(() => _isLoading = true);
      final authProvider = Provider.of<AuthProvider>(context, listen: false);

      try {
        if (!_codeSent) {
          final success =
              await authProvider.forgotPassword(_emailController.text);
          if (success) {
            setState(() => _codeSent = true);
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(content: Text('Reset code sent to your email')),
            );
          }
        } else if (!_codeVerified) {
          final success = await authProvider.verifyResetCode(
            _emailController.text,
            _codeController.text,
          );
          if (success) {
            setState(() => _codeVerified = true);
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(content: Text('Code verified successfully')),
            );
          }
        } else {
          final success = await authProvider.resetPassword(
            _emailController.text,
            _newPasswordController.text,
          );
          if (success) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(content: Text('Password reset successfully')),
            );
            Navigator.of(context).pop();
          }
        }
      } catch (error) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('An error occurred: $error')),
        );
      } finally {
        setState(() => _isLoading = false);
      }
    }
  }
}
