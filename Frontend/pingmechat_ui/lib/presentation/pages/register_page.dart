import 'dart:async';

import 'package:flutter/material.dart';
import 'package:pingmechat_ui/core/utils/input_validator.dart';
import 'package:pingmechat_ui/presentation/pages/login_page.dart';
import 'package:pingmechat_ui/presentation/widgets/app_bar.dart';
import 'package:pingmechat_ui/presentation/widgets/custom_text_field.dart';
import 'package:provider/provider.dart';

import '../../config/theme.dart';
import '../../providers/auth_provider.dart';
import '../widgets/custom_button.dart';
import 'verify_code_page.dart';

class RegisterPage extends StatefulWidget {
  static const routeName = '/register';

  const RegisterPage({super.key});

  @override
  State<RegisterPage> createState() => _RegisterPageState();
}

class _RegisterPageState extends State<RegisterPage> {
  final _formKey = GlobalKey<FormState>();
  final _pageController = PageController();
  final _nameController = TextEditingController();
  final _emailController = TextEditingController();
  final _passwordController = TextEditingController();
  final _confirmPasswordController = TextEditingController();
  final _usernameController = TextEditingController();
  final _phoneController = TextEditingController();
  final _genderController = TextEditingController();
  final _isFormValid = ValueNotifier<bool>(false);
  final _isFirstPageValid = ValueNotifier<bool>(false);
  final _isSecondPageValid = ValueNotifier<bool>(false);
  final _isLoading = ValueNotifier<bool>(false);
  Timer? _debounce;

  final _nameFocusNode = FocusNode();
  final _emailFocusNode = FocusNode();
  final _passwordFocusNode = FocusNode();
  final _confirmPasswordFocusNode = FocusNode();
  final _usernameFocusNode = FocusNode();
  final _phoneFocusNode = FocusNode();

  bool _hasInteractedWithName = false;
  bool _hasInteractedWithEmail = false;
  bool _hasInteractedWithPassword = false;
  bool _hasInteractedWithConfirmPassword = false;
  bool _hasInteractedWithUsername = false;
  bool _hasInteractedWithPhone = false;
  bool _hasInteractedWithGender = false;

  @override
  void initState() {
    super.initState();
    _nameController.addListener(_validateForm);
    _emailController.addListener(_validateForm);
    _passwordController.addListener(_validateForm);
    _confirmPasswordController.addListener(_validateForm);
    _usernameController.addListener(_validateForm);
    _phoneController.addListener(_validateForm);
    _genderController.addListener(_validateForm);
  }

  void _validateForm() {
    if (_debounce?.isActive ?? false) _debounce?.cancel();
    _debounce = Timer(const Duration(milliseconds: 500), () {
      _isFormValid.value = _formKey.currentState?.validate() ?? false;
      _isFirstPageValid.value = _validateFirstPage();
      _isSecondPageValid.value = _validateSecondPage();
    });
  }

  bool _validateFirstPage() {
    return InputValidator.validateName(_nameController.text) == null &&
        InputValidator.validateUsername(_usernameController.text) == null &&
        InputValidator.validatePhone(_phoneController.text) == null &&
        InputValidator.validateGender(_genderController.text) == null;
  }
  
  bool _validateSecondPage() {
    return InputValidator.validateName(_emailController.text) == null &&
        InputValidator.validateUsername(_passwordController.text) == null &&
        InputValidator.validatePhone(_confirmPasswordController.text) == null;
  }

  @override
  void dispose() {
    _debounce?.cancel();
    _nameController.dispose();
    _emailController.dispose();
    _passwordController.dispose();
    _confirmPasswordController.dispose();
    _usernameController.dispose();
    _phoneController.dispose();
    _genderController.dispose();
    _isFormValid.dispose();
    _isFirstPageValid.dispose();
    _isSecondPageValid.dispose();
    _nameFocusNode.dispose();
    _emailFocusNode.dispose();
    _passwordFocusNode.dispose();
    _confirmPasswordFocusNode.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: const CustomAppBar(),
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
              SizedBox(
                height: 600, // Adjust height as needed
                child: PageView(
                  controller: _pageController,
                  physics: NeverScrollableScrollPhysics(),
                  children: [
                    _buildFirstPage(),
                    _buildSecondPage(),
                  ],
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildFirstPage() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        _buildNameField(),
        const SizedBox(height: 16),
        _buildUsernameField(),
        const SizedBox(height: 16),
        _buildPhoneField(),
        const SizedBox(height: 16),
        _buildGenderField(),
        const SizedBox(height: 32),
        Center(
          child: ValueListenableBuilder<bool>(
            valueListenable: _isFirstPageValid,
            builder: (context, isFirstPageValid, child) {
              return CustomElevatedButton(
                text: 'Next',
                onPressed: isFirstPageValid
                    ? () {
                        _pageController.nextPage(
                          duration: const Duration(milliseconds: 300),
                          curve: Curves.easeInOut,
                        );
                      }
                    : () {
                        setState(() {
                          _hasInteractedWithName = true;
                          _hasInteractedWithUsername = true;
                          _hasInteractedWithPhone = true;
                          _hasInteractedWithGender = true;
                        });
                        _formKey.currentState?.validate();
                      },
                foregroundColor:
                    isFirstPageValid ? AppColors.white : AppColors.tertiary,
                backgroundColor: isFirstPageValid
                    ? AppColors.primary
                    : AppColors.disabledColor,
              );
            },
          ),
        ),
      ],
    );
  }

  Widget _buildSecondPage() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
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
                      : () {
                          setState(() {
                            _hasInteractedWithEmail = true;
                            _hasInteractedWithPassword = true;
                            _hasInteractedWithConfirmPassword = true;
                            _hasInteractedWithName = true;
                            _hasInteractedWithUsername = true;
                            _hasInteractedWithPhone = true;
                            _hasInteractedWithGender = true;
                          });
                        },
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
                MaterialPageRoute(builder: (context) => const LoginPage()),
              );
            },
            child: const Text(
              'Already have an account? Log in',
              style: TextStyle(color: AppColors.primary),
            ),
          ),
        ),
        Center(
          child: CustomElevatedButton(
            text: 'Back',
            onPressed: () {
              _pageController.previousPage(
                duration: const Duration(milliseconds: 300),
                curve: Curves.easeInOut,
              );
            },
            foregroundColor: AppColors.white,
            backgroundColor: AppColors.primary,
          ),
        ),
      ],
    );
  }

  Widget _buildNameField() {
    return CustomTextField(
      label: 'Full Name',
      autoFocus: true,
      controller: _nameController,
      validator: (value) {
        if (!_hasInteractedWithName) return null;
        return InputValidator.validateName(value);
      },
      keyboardType: TextInputType.name,
      focusNode: _nameFocusNode,
      onFieldSubmitted: (_) =>
          FocusScope.of(context).requestFocus(_emailFocusNode),
      onChanged: (value) {
        setState(() {
          _hasInteractedWithName = true;
        });
      },
    );
  }

  Widget _buildEmailField() {
    return CustomTextField(
      label: 'Your email',
      controller: _emailController,
      validator: (value) {
        if (!_hasInteractedWithEmail) return null;
        return InputValidator.validateEmail(value);
      },
      keyboardType: TextInputType.emailAddress,
      focusNode: _emailFocusNode,
      onFieldSubmitted: (_) =>
          FocusScope.of(context).requestFocus(_passwordFocusNode),
      onChanged: (value) {
        setState(() {
          _hasInteractedWithEmail = true;
        });
      },
    );
  }

  Widget _buildPasswordField() {
    return CustomTextField(
      label: 'Password',
      isPassword: true,
      controller: _passwordController,
      validator: (value) {
        if (!_hasInteractedWithPassword) return null;
        return InputValidator.validatePassword(value);
      },
      keyboardType: TextInputType.visiblePassword,
      focusNode: _passwordFocusNode,
      onChanged: (value) {
        setState(() {
          _hasInteractedWithPassword = true;
        });
      },
      onFieldSubmitted: (_) =>
          FocusScope.of(context).requestFocus(_confirmPasswordFocusNode),
    );
  }

  Widget _buildConfirmPasswordField() {
    return CustomTextField(
      label: 'Confirm Password',
      isPassword: true,
      controller: _confirmPasswordController,
      validator: (value) {
        if (!_hasInteractedWithConfirmPassword) return null;
        return InputValidator.validateConfirmPassword(
            value, _passwordController.text);
      },
      keyboardType: TextInputType.visiblePassword,
      focusNode: _confirmPasswordFocusNode,
      onChanged: (value) {
        setState(() {
          _hasInteractedWithConfirmPassword = true;
        });
      },
      onFieldSubmitted: (_) => _handleRegister(),
    );
  }

  Widget _buildUsernameField() {
    return CustomTextField(
      label: 'Username',
      controller: _usernameController,
      validator: (value) {
        if (!_hasInteractedWithUsername) return null;
        return InputValidator.validateUsername(value);
      },
      keyboardType: TextInputType.text,
      focusNode: _usernameFocusNode,
      onChanged: (value) {
        setState(() {
          _hasInteractedWithUsername = true;
        });
      },
      onFieldSubmitted: (_) =>
          FocusScope.of(context).requestFocus(_passwordFocusNode),
    );
  }

  Widget _buildPhoneField() {
    return CustomTextField(
      label: 'Phone',
      controller: _phoneController,
      validator: (value) {
        if (!_hasInteractedWithPhone) return null;
        return InputValidator.validatePhone(value);
      },
      keyboardType: TextInputType.phone,
      focusNode: _phoneFocusNode,
      onChanged: (value) {
        setState(() {
          _hasInteractedWithPhone = true;
        });
      },
      onFieldSubmitted: (_) => _handleRegister(),
    );
  }

  Widget _buildGenderField() {
    return DropdownButtonFormField<String>(
      decoration: InputDecoration(
        labelText: 'Gender',
        border: OutlineInputBorder(),
      ),
      value: _genderController.text.isEmpty ? null : _genderController.text,
      items: ['Male', 'Female', 'Other'].map((String value) {
        return DropdownMenuItem<String>(
          value: value,
          child: Text(value),
        );
      }).toList(),
      onChanged: (String? newValue) {
        setState(() {
          _genderController.text = newValue ?? '';
          _hasInteractedWithGender = true;
        });
      },
      validator: (value) {
        if (!_hasInteractedWithGender) return null;
        return InputValidator.validateGender(value);
      },
    );
  }

  void _handleRegister() async {
    setState(() {
      _hasInteractedWithEmail = true;
      _hasInteractedWithPassword = true;
      _hasInteractedWithConfirmPassword = true;
      _hasInteractedWithName = true;
      _hasInteractedWithUsername = true;
      _hasInteractedWithPhone = true;
      _hasInteractedWithGender = true;
    });
    if (_formKey.currentState!.validate()) {
      _isLoading.value = true;
      try {
        final authProvider = Provider.of<AuthProvider>(context, listen: false);
        // Gọi API để đăng ký tài khoản
        final success = await authProvider.signup(
          _emailController.text.trim(),
          _usernameController.text.trim(),
          _passwordController.text.trim(),
          _confirmPasswordController.text.trim(),
          _nameController.text.trim(),
          _phoneController.text.trim(),
        );
        if (success) {
          // Đăng ký thành công thì thông báo và chuyển hướng đến trang xác nhận mã
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text('Registration successful')),
          );
          // Đăng ký thành công thì thông báo và chuyển hướng đến trang xác nhận mã
          Navigator.of(context).push(MaterialPageRoute(builder: (context) => VerifyCodePage(email: _emailController.text.trim())));
        } else {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text('Registration failed')),
          );
        }
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
