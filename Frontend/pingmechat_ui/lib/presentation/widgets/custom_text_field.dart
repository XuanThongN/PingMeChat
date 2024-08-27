import 'package:flutter/material.dart';
import 'package:pingmechat_ui/config/theme.dart';

class CustomTextField extends StatelessWidget {
  const CustomTextField({
    super.key,
    required this.label,
    this.isPassword = false,
    this.autoFocus = false,
    required this.controller,
    this.errorText,
    this.onChanged,
  });

  final String label;
  final bool isPassword;
  final bool autoFocus;
  final TextEditingController controller;
  final String? errorText;
  final void Function(String)? onChanged;

  @override
  Widget build(BuildContext context) {
    return TextFormField(
      controller: controller,
      autofocus: autoFocus,
      obscureText: isPassword,
      decoration: InputDecoration(
        border: const UnderlineInputBorder(),
        labelText: label,
        labelStyle: const TextStyle(color: AppColors.primary),
        focusedBorder: const UnderlineInputBorder(
          borderSide: BorderSide(color: AppColors.primary),
        ),
        errorText: errorText,
      ),
      onChanged: onChanged,
    );
  }
}
