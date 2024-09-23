import 'package:flutter/material.dart';

class CustomCircleAvatar extends StatelessWidget {
  final ImageProvider? backgroundImage;
  final double radius;
  final Widget? child;

  const CustomCircleAvatar({
    Key? key,
    this.backgroundImage,
    this.radius = 20.0,
    this.child,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return CircleAvatar(
      backgroundImage: backgroundImage ?? AssetImage('assets/images/default_avatar.png'),
      radius: radius,
      child: child,
    );
  }
}