import 'dart:async';

import 'package:flutter/material.dart';
import 'package:pingmechat_ui/core/utils/input_validator.dart';
import 'package:pingmechat_ui/presentation/widgets/app_bar.dart';
import 'package:pingmechat_ui/presentation/widgets/custom_divider.dart';
import 'package:pingmechat_ui/presentation/widgets/custom_text_field.dart';
import 'package:provider/provider.dart';

import '../../config/theme.dart';
import '../../providers/auth_provider.dart';
import '../widgets/custom_button.dart';
import '../widgets/social_button.dart';
import 'home.dart';

class LoginPage extends StatefulWidget {
  static const routeName = '/login';

  const LoginPage({Key? key}) : super(key: key);

  @override
  State<LoginPage> createState() => _LoginPageState();
}

class _LoginPageState extends State<LoginPage> {
  final _formKey =
      GlobalKey<FormState>(); // Được dùng để xác thực form trong Flutter
  final _emailController = TextEditingController();
  final _passwordController = TextEditingController();
  final _isFormValid = ValueNotifier<bool>(false);
  Timer? _debounce; // Được dùng để giữ cho việc gọi hàm không bị gián đoạn
  final _isLoading =
      ValueNotifier<bool>(false); // Để xác định trạng thái loading
  final _emailFocusNode = FocusNode();
  final _passwordFocusNode = FocusNode();

  @override
  void initState() {
    super.initState();
    _emailController.addListener(_validateForm);
    _passwordController.addListener(_validateForm);
  }

  void _validateForm() {
    if (_debounce?.isActive ?? false) {
      _debounce
          ?.cancel(); // Nếu _debounce chưa bắt đầu hoặc đã kết thúc thì hủy bỏ
    }
    _debounce = Timer(const Duration(milliseconds: 500), () {
      // Sau 500ms thì thực hiện hàm
      _isFormValid.value = _formKey.currentState?.validate() ??
          false; // Gán giá trị cho _isFormValid
    });
  }

  @override
  void dispose() {
    _debounce?.cancel(); // Hủy bỏ _debounce
    _emailController.dispose();
    _passwordController.dispose();
    _isFormValid.dispose();
    _emailFocusNode.dispose();
    _passwordFocusNode.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.white,
      appBar: CustomAppBar(),
      body: SingleChildScrollView(
        padding: const EdgeInsets.symmetric(
            vertical: 12.0, horizontal: 24), // Khoảng cách giữa các cạnh
        child: Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              const Center(
                child: Column(
                  children: [
                    Text(
                      'Log in to Ping Me Chat',
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
              Padding(
                padding:
                    const EdgeInsets.symmetric(vertical: 32, horizontal: 16),
                child: ListSocialButtons(),
              ),
              const CustomDivider(),
              const SizedBox(height: 32),
              _buildEmailField(),
              const SizedBox(height: 16),
              _buildPasswordField(),
              const SizedBox(height: 32),
              // ValueListenableBuilder<bool>(
              //   valueListenable: _isFormValid,
              //   builder: (context, isFormValid, child) {
              //     return CustomElevatedButton(
              //       text: 'Log in',
              //       onPressed: isFormValid ? _handleLogin : () {},
              //       foregroundColor:
              //           isFormValid ? AppColors.white : AppColors.tertiary,
              //       backgroundColor: isFormValid
              //           ? AppColors.primary
              //           : AppColors.disabledColor,
              //     );
              //   },
              // ),
              ValueListenableBuilder<bool>(
                valueListenable: _isLoading,
                builder: (context, isLoading, child) {
                  return ValueListenableBuilder<bool>(
                    valueListenable: _isFormValid,
                    builder: (context, isFormValid, child) {
                      return CustomElevatedButton(
                        text: 'Log in',
                        onPressed: _handleLogin,
                        foregroundColor: AppColors.white,
                        backgroundColor: AppColors.primary,
                        isLoading: isLoading,
                      );
                    },
                  );
                },
              ),
              Center(
                child: TextButton(
                  onPressed: () {},
                  child: const Text(
                    'Forgot password?',
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

  Widget _buildEmailField() {
    return CustomTextField(
      label: 'Email or Username',
      controller: _emailController,
      validator: (value) {
        if (value == null || value.isEmpty) {
          return 'Please enter your email or username';
        }
        if (InputValidator.validateEmail(value) == null ||
            InputValidator.validateUsername(value) == null) {
          return null;
        }
        return 'Please enter a valid email or username';
      },
      keyboardType: TextInputType.emailAddress,
      autoFocus: true,
      focusNode: _emailFocusNode,
      onFieldSubmitted: (_) => FocusScope.of(context).requestFocus(
          _passwordFocusNode), // Move focus to password field when Enter is pressed
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
          _handleLogin(), // Gọi hàm _handleLogin khi nhấn Enter
    );
  }

  void _handleLogin() async {
    if (_formKey.currentState!.validate()) {
      _isLoading.value = true;
      try {
        final authProvider = Provider.of<AuthProvider>(context, listen: false);
        final success = await authProvider.login(
          _emailController.text,
          _passwordController.text,
        );
        if (success) {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text('Login successful')),
          );
          Navigator.of(context).pushReplacementNamed(HomePage.routeName);
        } else {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text('Login failed')),
          );
        }
      } catch (e) {
        // Xử lý lỗi
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Login failed: $e')),
        );
      } finally {
        _isLoading.value = false;
      }
    }
  }
}
