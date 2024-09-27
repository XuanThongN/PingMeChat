//Create a custom app bar widget
import 'package:flutter/material.dart';

import '../../config/theme.dart';

class CustomAppBar extends StatelessWidget implements PreferredSizeWidget {
  // add custom Function to handle back button  
  final Function()? onBackButtonPressed;
  const CustomAppBar({super.key, this.onBackButtonPressed});

  @override
  Widget build(BuildContext context) {
    return AppBar(
      leading: IconButton(
        icon: const Icon(Icons.arrow_back, color: Colors.black),
        onPressed: onBackButtonPressed ?? () => Navigator.of(context).pop(),
      ),
      backgroundColor: AppColors.white,
      elevation: 0,
    );
  }

  @override
  Size get preferredSize => const Size.fromHeight(kToolbarHeight);
}
