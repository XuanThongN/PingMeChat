import 'package:flutter/material.dart';

class AppTypography {
  static const TextStyle h1 = TextStyle(
    fontFamily: 'Caros',
    fontSize: 40,
    color: AppColors.surface,
  );

  static const TextStyle h2 = TextStyle(
    fontFamily: 'Caros',
    fontSize: 20,
    color: AppColors.white,
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
    fontFamily: 'CircularStd',
    fontSize: 12,
  );

  static const TextStyle subH2 = TextStyle(
    fontFamily: 'CircularStd',
    fontSize: 14,
    color: AppColors.white,
  );

  static const TextStyle subH3 = TextStyle(
    fontFamily: 'CircularStd',
    fontSize: 16,
    color: AppColors.tertiary,
  );

  static const TextStyle p1 = TextStyle(
    fontFamily: 'CircularStd',
    fontSize: 16,
  );

  static const TextStyle message = TextStyle(
    fontFamily: 'CircularStd',
    fontSize: 12,
    color: AppColors.secondary,
  );
  static const TextStyle otherMessage = TextStyle(
    fontFamily: 'CircularStd',
    fontSize: 12,
    color: AppColors.secondary,
  );
  static const TextStyle myMessage = TextStyle(
    fontFamily: 'CircularStd',
    fontSize: 12,
    color: AppColors.white,
  );

  static const TextStyle p3 = TextStyle(
    fontFamily: 'CircularStd',
    fontSize: 12,
  );

  //Tạo style cho hiển thị tên người dùng đoạn chat
  static const TextStyle chatName = TextStyle(
    fontFamily: 'Caros',
    fontSize: 20,
    color: AppColors.secondary,
    fontWeight: FontWeight.w500,
  );
  static const TextStyle headline = TextStyle(
    fontFamily: 'Caros',
    fontSize: 24,
    fontWeight: FontWeight.bold,
    color: AppColors.primary,
  );

  static const TextStyle caption = TextStyle(
    fontFamily: 'CircularStd',
    fontSize: 12,
    color: AppColors.tertiary,
  );

  static const TextStyle badge = TextStyle(
    fontFamily: 'CircularStd',
    fontSize: 10,
    color: AppColors.white,
    fontWeight: FontWeight.bold,
  );

  //Tạo style cho hiển thị tin nhắn cuối cùng trong danh sách chat
  static const TextStyle chatMessage = TextStyle(
    fontFamily: 'CircularStd',
    fontSize: 12,
    color: AppColors.tertiary,
    fontWeight: FontWeight.w300,
  );

  //Tạo style cho hiển thị thời gian cho tin nhắn trong đoạn chat
  static const TextStyle chatTime = TextStyle(
    fontFamily: 'CircularStd',
    fontSize: 10,
    color: AppColors.tertiary,
    fontWeight: FontWeight.w300,
  );

  // Method to get medium font style
  static TextStyle getMediumStyle(TextStyle style) {
    return style.copyWith(fontWeight: FontWeight.w500);
  }

  // Method to get book font style
  static TextStyle getBookStyle(TextStyle style) {
    return style.copyWith(fontWeight: FontWeight.w300);
  }

  // Method to get italic font style
  static TextStyle getItalicStyle(TextStyle style) {
    return style.copyWith(fontStyle: FontStyle.italic);
  }
}

class AppColors {
  static const Color primary = Color(0xFF24786D);
  static const Color primary_chat = Color(0xFF20A090);
  static const Color secondary = Color(0xFF000E08);
  static const Color tertiary = Color(0xFF797C7B);
  static const Color background = Color(0xFF121414);
  static const Color background2 = Color(0xFFF5F5F5);
  static const Color surface = Color(0xFFF2F7FB);
  static const Color white = Color(0xFFFFFFFF);
  static const Color disabledColor = Color(0xFFF3F6F6);
  static const Color red = Color(0xFFEA3736);
  static const Color rippleColor = Color(0xFFD9D9D9);
  static const Color hoverColor = Color(0xFFE0E0E0);
  static const Color colorOtherMessage = Color(0xFFE0E0E0);
  static const Color grey = Color(0xFFF0F3F5);
}

final ThemeData appTheme = ThemeData(
  primaryColor: AppColors.primary,
  colorScheme: ColorScheme.fromSwatch().copyWith(
    surface: AppColors.surface,
  ),
  scaffoldBackgroundColor: AppColors.surface,
  fontFamily: 'CircularStd',
  textTheme: const TextTheme(
    headlineLarge: AppTypography.h1,
    headlineMedium: AppTypography.h2,
    headlineSmall: AppTypography.h3,
    titleLarge: AppTypography.h4,
    titleMedium: AppTypography.subH3,
    titleSmall: AppTypography.subH2,
    bodyLarge: AppTypography.p1,
    // bodyMedium: AppTypography.p2,
    bodySmall: AppTypography.p3,
  ),
);
