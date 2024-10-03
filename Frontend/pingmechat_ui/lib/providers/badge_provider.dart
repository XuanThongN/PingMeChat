import 'package:flutter/material.dart';

class BadgeProvider with ChangeNotifier {
  int _messageCount = 0;
  int _contactsCount = 0;

  int get messageCount => _messageCount;
  int get contactsCount => _contactsCount;

  void updateMessageCount(int count) {
    _messageCount = count;
    notifyListeners();
  }

  void updateContactsCount(int count) {
    _contactsCount = count;
    notifyListeners();
  }
}
