// Create custom divider widget
import 'package:flutter/material.dart';
import '../../config/theme.dart';

class CustomDivider extends StatelessWidget {
  const CustomDivider({super.key});

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Row(
        children: [
          const Expanded(
            child: Divider(
              color: AppColors.tertiary,
              thickness: 1,
              indent: 20,
              endIndent: 10,
            ),
          ),
          Text(
            'OR',
            style: AppTypography.h4.copyWith(
              color: AppColors.tertiary,
            ),
          ),
          const Expanded(
            child: Divider(
              color: AppColors.tertiary,
              thickness: 1,
              indent: 10,
              endIndent: 20,
            ),
          ),
        ],
      ),
    );
  }
}
