import 'package:flutter/material.dart';

class CustomCircleAvatar extends StatelessWidget {
  final ImageProvider? backgroundImage;
  final double radius;
  final Widget? child;

  const CustomCircleAvatar({
    super.key,
    this.backgroundImage,
    this.radius = 20.0,
    this.child,
  });

  @override
  Widget build(BuildContext context) {
    return CircleAvatar(
      backgroundImage: backgroundImage ?? const AssetImage('assets/images/default_avatar.png'),
      radius: radius,
      child: child,
    );
  }
}