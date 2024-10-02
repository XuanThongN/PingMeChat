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
              const CachedNetworkImageProvider(ImageConstants.defaultAvatarUrl))
          : const CachedNetworkImageProvider(
              ImageConstants.defaultGroupAvatarUrl),
      radius: radius,
      child: child,
    );
  }
}
