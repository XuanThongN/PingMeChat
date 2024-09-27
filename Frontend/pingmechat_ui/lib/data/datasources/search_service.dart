import 'dart:convert';
import 'package:http/http.dart' as http;
import 'package:pingmechat_ui/core/constants/constant.dart';
import 'package:pingmechat_ui/data/models/search_result.dart';
import 'package:pingmechat_ui/providers/auth_provider.dart';

class SearchService {
  final AuthProvider authProvider;

  SearchService({required this.authProvider});

  Future<SearchResult> search(
      {required String keyword, int pageNumber = 1, int pageSize = 20}) async {
    final response = await http.get(
        Uri.parse(
            '${ApiConstants.searchEndpoint}?keyword=$keyword&pageNumber=$pageNumber&pageSize=$pageSize'),
        headers: await authProvider.getCustomHeaders());

    if (response.statusCode == 200) {
      final Map<String, dynamic> data = json.decode(response.body);
      final result = SearchResult.fromJson(data['result']);
      return result;
    } else {
      throw Exception('Failed to load search results');
    }
  }
}
