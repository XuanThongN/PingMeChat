import 'dart:async';

import 'package:flutter/material.dart';
import 'package:pingmechat_ui/core/utils/input_validator.dart';
import 'package:pingmechat_ui/presentation/pages/login_page.dart';
import 'package:pingmechat_ui/presentation/widgets/app_bar.dart';
import 'package:pingmechat_ui/presentation/widgets/custom_divider.dart';
import 'package:pingmechat_ui/presentation/widgets/custom_text_field.dart';

import '../../config/theme.dart';
import '../widgets/custom_button.dart';
import '../widgets/social_button.dart';

class RegisterPage extends StatefulWidget {
  const RegisterPage({Key? key}) : super(key: key);

  @override
  State<RegisterPage> createState() => _RegisterPageState();
}

class _RegisterPageState extends State<RegisterPage> {
  final _formKey = GlobalKey<FormState>();
  final _nameController = TextEditingController();
  final _emailController = TextEditingController();
  final _passwordController = TextEditingController();
  final _confirmPasswordController = TextEditingController();
  final _isFormValid = ValueNotifier<bool>(false);
  final _isLoading = ValueNotifier<bool>(false);
  Timer? _debounce;

  final _nameFocusNode = FocusNode();
  final _emailFocusNode = FocusNode();
  final _passwordFocusNode = FocusNode();
  final _confirmPasswordFocusNode = FocusNode();

  @override
  void initState() {
    super.initState();
    _nameController.addListener(_validateForm);
    _emailController.addListener(_validateForm);
    _passwordController.addListener(_validateForm);
    _confirmPasswordController.addListener(_validateForm);
  }

  void _validateForm() {
    if (_debounce?.isActive ?? false) _debounce?.cancel();
    _debounce = Timer(const Duration(milliseconds: 500), () {
      _isFormValid.value = _formKey.currentState?.validate() ?? false;
    });
  }

  @override
  void dispose() {
    _debounce?.cancel();
    _nameController.dispose();
    _emailController.dispose();
    _passwordController.dispose();
    _confirmPasswordController.dispose();
    _isFormValid.dispose();
    _nameFocusNode.dispose();
    _emailFocusNode.dispose();
    _passwordFocusNode.dispose();
    _confirmPasswordFocusNode.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CustomAppBar(),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(24.0),
        child: Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              const Center(
                child: Column(
                  children: [
                    Text(
                      'Create your Account',
                      style: TextStyle(
                        fontSize: 24,
                        fontWeight: FontWeight.bold,
                        color: Colors.black,
                      ),
                    ),
                    Text(
                      'Please fill in the form to continue',
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
              _buildNameField(),
              const SizedBox(height: 16),
              _buildEmailField(),
              const SizedBox(height: 16),
              _buildPasswordField(),
              const SizedBox(height: 16),
              _buildConfirmPasswordField(),
              const SizedBox(height: 32),
              ValueListenableBuilder<bool>(
                valueListenable: _isLoading,
                builder: (context, isLoading, child) {
                  return ValueListenableBuilder<bool>(
                    valueListenable: _isFormValid,
                    builder: (context, isFormValid, child) {
                      return CustomElevatedButton(
                        text: 'Register',
                        onPressed: (isFormValid && !isLoading)
                            ? _handleRegister
                            : () {},
                        foregroundColor: (isFormValid && !isLoading)
                            ? AppColors.white
                            : AppColors.tertiary,
                        backgroundColor: (isFormValid && !isLoading)
                            ? AppColors.primary
                            : AppColors.disabledColor,
                        isLoading: isLoading,
                      );
                    },
                  );
                },
              ),
              const SizedBox(height: 16),
              Center(
                child: TextButton(
                  onPressed: () {
                    // Navigate to login screen here
                    Navigator.push(
                      context,
                      MaterialPageRoute(
                          builder: (context) => const LoginPage()),
                    );
                  },
                  child: const Text(
                    'Already have an account? Log in',
                    style: TextStyle(color: AppColors.primary),
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildNameField() {
    return CustomTextField(
      label: 'Full Name',
      controller: _nameController,
      validator: InputValidator.validateName,
      keyboardType: TextInputType.name,
      focusNode: _nameFocusNode,
      onFieldSubmitted: (_) =>
          FocusScope.of(context).requestFocus(_emailFocusNode),
    );
  }

  Widget _buildEmailField() {
    return CustomTextField(
      label: 'Your email',
      controller: _emailController,
      validator: InputValidator.validateEmail,
      keyboardType: TextInputType.emailAddress,
      focusNode: _emailFocusNode,
      onFieldSubmitted: (_) =>
          FocusScope.of(context).requestFocus(_passwordFocusNode),
    );
  }

  Widget _buildPasswordField() {
    return CustomTextField(
      label: 'Password',
      isPassword: true,
      controller: _passwordController,
      validator: InputValidator.validatePassword,
      keyboardType: TextInputType.visiblePassword,
      focusNode: _passwordFocusNode,
      onFieldSubmitted: (_) =>
          FocusScope.of(context).requestFocus(_confirmPasswordFocusNode),
    );
  }

  Widget _buildConfirmPasswordField() {
    return CustomTextField(
      label: 'Confirm Password',
      isPassword: true,
      controller: _confirmPasswordController,
      validator: (value) => InputValidator.validateConfirmPassword(
          value, _passwordController.text),
      keyboardType: TextInputType.visiblePassword,
      focusNode: _confirmPasswordFocusNode,
      onFieldSubmitted: (_) => _handleRegister(),
    );
  }

  void _handleRegister() async {
    if (_formKey.currentState!.validate()) {
      _isLoading.value = true;
      try {
        // Call API here
        await Future.delayed(const Duration(seconds: 2)); // Fake API call
        // Xử lý kết quả đăng ký thành công
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Registration successful')),
        );
        // Navigate to next screen or login screen
      } catch (e) {
        // Xử lý lỗi
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Registration failed: $e')),
        );
      } finally {
        _isLoading.value = false;
      }
    }
  }
}
