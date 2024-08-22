import 'package:flutter/material.dart';

class AppTypography {
  static const TextStyle h1 = TextStyle(
    fontFamily: 'Caros',
    fontSize: 40,
  );

  static const TextStyle h2 = TextStyle(
    fontFamily: 'Caros',
    fontSize: 20,
  );

  static const TextStyle h3 = TextStyle(
    fontFamily: 'Caros',
    fontSize: 18,
  );

  static const TextStyle h4 = TextStyle(
    fontFamily: 'Caros',
    fontSize: 16,
  );

  static const TextStyle subH1 = TextStyle(
    fontFamily: 'Circular Std',
    fontSize: 12,
  );

  static const TextStyle subH2 = TextStyle(
    fontFamily: 'Circular Std',
    fontSize: 14,
  );

  static const TextStyle subH3 = TextStyle(
    fontFamily: 'Circular Std',
    fontSize: 16,
  );

  static const TextStyle p1 = TextStyle(
    fontFamily: 'Circular Std',
    fontSize: 16,
  );

  static const TextStyle p2 = TextStyle(
    fontFamily: 'Circular Std',
    fontSize: 14,
  );

  static const TextStyle p3 = TextStyle(
    fontFamily: 'Circular Std',
    fontSize: 12,
  );
}

class AppColors {
  static const Color primary = Color(0xFF24786D);
  static const Color secondary = Color(0xFF000E08);
  static const Color tertiary = Color(0xFF797C7B);
  static const Color background = Color(0xFF121414);
  static const Color surface = Color(0xFFF2F7FB);
  static const Color white = Color(0xFFFFFFFF);
}

final ThemeData appTheme = ThemeData(
  primaryColor: AppColors.primary,
  colorScheme: ColorScheme.fromSwatch().copyWith(
    surface: AppColors.surface,
  ),
  scaffoldBackgroundColor: AppColors.surface,
  textTheme: const TextTheme(
    headlineLarge: AppTypography.h1,
    headlineMedium: AppTypography.h2,
    headlineSmall: AppTypography.h3,
    titleLarge: AppTypography.h4,
    titleMedium: AppTypography.subH3,
    titleSmall: AppTypography.subH2,
    bodyLarge: AppTypography.p1,
    bodyMedium: AppTypography.p2,
    bodySmall: AppTypography.p3,
  ),
);