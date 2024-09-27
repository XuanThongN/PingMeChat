import 'package:flutter/material.dart';
import 'package:pingmechat_ui/data/datasources/search_service.dart';
import 'package:pingmechat_ui/data/models/search_result.dart';

class SearchProvider extends ChangeNotifier {
  final SearchService _searchService;
  SearchResult? _searchResult;
  bool _isLoading = false;

  SearchProvider(this._searchService);

  SearchResult? get searchResult => _searchResult;
  bool get isLoading => _isLoading;

  Future<void> search(String keyword) async {
    _isLoading = true;
    notifyListeners();

    try {
      _searchResult = await _searchService.search(keyword: keyword);
    } catch (e) {
      // Handle error
      _searchResult = null;
    }

    _isLoading = false;
    notifyListeners();
  }
}