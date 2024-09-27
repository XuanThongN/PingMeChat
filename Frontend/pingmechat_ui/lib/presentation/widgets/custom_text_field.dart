import 'package:flutter/material.dart';
import 'package:pingmechat_ui/config/theme.dart';

class CustomTextField extends StatelessWidget {
  const CustomTextField({
    super.key,
    required this.label,
    this.isPassword = false,
    this.autoFocus = false,
    required this.controller,
    this.validator,
    this.keyboardType,
    this.focusNode,
    this.onFieldSubmitted,
    this.onChanged
  });

  final String label;
  final bool isPassword;
  final bool autoFocus;
  final TextEditingController controller;
  final String? Function(String?)? validator;
  final TextInputType? keyboardType;
  final FocusNode? focusNode;
  final void Function(String)? onFieldSubmitted;
  final void Function(String)? onChanged;

  @override
  Widget build(BuildContext context) {
    return TextFormField(
      controller: controller,
      autofocus: autoFocus,
      obscureText: isPassword,
      validator: validator,
      keyboardType: keyboardType,
      autovalidateMode: AutovalidateMode.onUserInteraction,
      focusNode: focusNode,
      onFieldSubmitted: onFieldSubmitted,
      onChanged: onChanged,
      decoration: InputDecoration(
        border: const UnderlineInputBorder(),
        labelText: label,
        labelStyle: const TextStyle(color: AppColors.primary),
        focusedBorder: const UnderlineInputBorder(
          borderSide: BorderSide(color: AppColors.primary),
        ),
        enabledBorder: const UnderlineInputBorder(
          borderSide: BorderSide(color: AppColors.tertiary),
        ),
      ),
    );
  }
}
