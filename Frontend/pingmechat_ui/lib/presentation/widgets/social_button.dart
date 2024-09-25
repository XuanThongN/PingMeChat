import 'package:flutter/material.dart';

class SocialButton extends StatelessWidget {
  final String icon;
  final VoidCallback onPressed;

  const SocialButton({
    super.key,
    required this.icon,
    required this.onPressed,
  });

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

class ListSocialButtons extends StatelessWidget {
  const ListSocialButtons({super.key});

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceEvenly,
      children: [
        SocialButton(
          icon: 'assets/icons/facebook_icon.png',
          onPressed: () {},
        ),
        SocialButton(
          icon: 'assets/icons/google_icon.png',
          onPressed: () {},
        ),
        SocialButton(
          icon: 'assets/icons/apple_icon.png',
          onPressed: () {},
        ),
      ],
    );
  }
}
