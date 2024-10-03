import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';

import '../../core/constants/constant.dart';

class CustomCircleAvatar extends StatelessWidget {
  final ImageProvider? backgroundImage;
  final double radius;
  final Widget? child;
  final bool isGroupChat;

  const CustomCircleAvatar({
    super.key,
    this.backgroundImage,
    this.radius = 20.0,
    this.child,
    this.isGroupChat = false,
  });

  @override
  Widget build(BuildContext context) {
    return CircleAvatar(
      backgroundImage: !isGroupChat
          ? (backgroundImage ??
              // If no image is provided, show a placeholder image in assets/images/default_avatar.png
              const AssetImage(ImageConstants.defaultAvatarPath))
          : const AssetImage(ImageConstants.defaultGroupAvatarPath),
      radius: radius,
      child: child,
    );
  }
}
