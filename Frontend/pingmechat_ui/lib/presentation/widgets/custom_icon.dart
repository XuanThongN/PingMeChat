// Create a custom icon widget that can be reused in multiple places in the app.
import 'package:flutter/material.dart';
import 'package:flutter_svg/svg.dart';

class CustomActionIcon extends StatelessWidget {
  final Color color;
  final String svgPath;

  const CustomActionIcon({
    super.key,
    required this.svgPath,
    required this.color,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      width: 40,
      height: 40,
      decoration: BoxDecoration(
        color: color,
        shape: BoxShape.circle,
      ),
      child: Center(
        child: SvgPicture.asset(
          svgPath,
          width: 20,
          height: 20,
          color: Colors.white,
        ),
      ),
    );
  }
}

class CustomSvgIcon extends StatelessWidget {
  final String svgPath;
  double size;
  Color? color;

  CustomSvgIcon({
    super.key,
    required this.svgPath,
    this.size = 30,
    this.color,
  });

  @override
  Widget build(BuildContext context) {
    return SvgPicture.asset(
      svgPath,
      width: size,
      height: size,
      color: color,
    );
  }
}
