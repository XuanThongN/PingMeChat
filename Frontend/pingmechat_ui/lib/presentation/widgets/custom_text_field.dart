import 'package:flutter/material.dart';

Widget _buildTextField(String label, {bool isPassword = false}) {
  return TextField(
    obscureText: isPassword,
    decoration: InputDecoration(
      labelText: label,
      labelStyle: const TextStyle(color: Colors.teal),
      focusedBorder: const UnderlineInputBorder(
        borderSide: BorderSide(color: Colors.teal),
      ),
    ),
  );
}