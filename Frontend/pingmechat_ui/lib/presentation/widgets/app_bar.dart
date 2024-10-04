import 'package:flutter/material.dart';
import '../../config/theme.dart';

class CustomAppBar extends StatelessWidget implements PreferredSizeWidget {
  final Function()? onBackButtonPressed;
  final String? title;
  final Color? backgroundColor;
  final Color? textColor;

  const CustomAppBar({
    Key? key, 
    this.onBackButtonPressed,
    this.title,
    this.backgroundColor,
    this.textColor,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return AppBar(
      leading: IconButton(
        icon: Icon(Icons.arrow_back, color: textColor ?? AppColors.secondary),
        onPressed: onBackButtonPressed ?? () => Navigator.of(context).pop(),
      ),
      title: title != null ? Text(
        title!,
        style: TextStyle(color: textColor ?? AppColors.secondary),
      ) : null,
      backgroundColor: backgroundColor ?? AppColors.white,
      elevation: 0,
    );
  }

  @override
  Size get preferredSize => const Size.fromHeight(kToolbarHeight);
}
