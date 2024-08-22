import 'package:flutter/material.dart';

class SocialButton extends StatelessWidget {
  final String icon;
  final VoidCallback onPressed;

  const SocialButton({
    Key? key,
    required this.icon,
    required this.onPressed,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return IconButton(
      icon: Image.asset(icon, width: 46, height: 46),
      onPressed: onPressed,
      iconSize: 50,
      padding: EdgeInsets.zero,
      constraints: const BoxConstraints(
        minWidth: 50,
        minHeight: 50,
      ),
      style: ButtonStyle(
        shape: WidgetStateProperty.all(
          CircleBorder(
            side: BorderSide(color: Colors.grey.shade500),
          ),
        ),
        backgroundColor: WidgetStateProperty.all(Colors.white),
      ),
    );
  }
}