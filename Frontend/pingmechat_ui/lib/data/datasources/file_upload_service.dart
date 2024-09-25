import 'dart:io';
import 'package:http/http.dart' as http;
import 'dart:convert';

import '../../core/constants/constant.dart';
import '../../providers/auth_provider.dart';


class UploadResult {
  final String url;
  UploadResult({required this.url});
}
