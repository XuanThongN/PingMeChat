import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:pingmechat_ui/providers/auth_provider.dart';
import 'package:provider/provider.dart';
import 'dart:convert';

import '../core/constants/constant.dart';
import '../domain/models/contact.dart';

class ContactProvider extends ChangeNotifier {
  final AuthProvider _authProvider;
  ContactProvider(this._authProvider);

  List<Contact> _contacts = [];
  bool _isLoading = false;

  List<Contact> get contacts => _contacts;
  bool get isLoading => _isLoading;

  Future<void> fetchContacts() async {
    _isLoading = true;
    notifyListeners();

    try {
      final response = await http.get(
        Uri.parse(ApiConstants.getContactListByCurrentUserEndpoint),
        headers: await _authProvider.getCustomHeaders(),
      );

      if (response.statusCode == 200) {
        final jsonResponse = json.decode(response.body);

        // Lấy phần 'result' và truy cập vào 'data' trong đó
        final List<dynamic> data = jsonResponse['result'];
        if (data.isNotEmpty) {
          _contacts = data.map((data) => Contact.fromJson(data)).toList();
        }else {
          _contacts = [];
        }
      } else {
        throw Exception('Failed to load contacts');
      }
    } catch (error) {
      print('Error fetching contacts: $error');
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

  Future<void> addContact(AddContactRequest input) async {
    try {
      final response = await http.post(
        Uri.parse(ApiConstants.addContactEndpoint),
        headers: await _authProvider.getCustomHeaders(),
        body: json.encode(input.toJson()),
      );

      if (response.statusCode == 201) {
        //lấy dữ liệu trả về từ server
        final responseData = json.decode(response.body);
        final contact = Contact.fromJson(responseData);
        _contacts.add(contact);
        notifyListeners();
      } else {
        throw Exception('Failed to add contact');
      }
    } catch (error) {
      print('Error adding contact: $error');
    }
  }

  Future<void> removeContact(String contactId) async {
    try {
      final response = await http.delete(
        Uri.parse('${ApiConstants.removeContactEndpoint}/$contactId'),
        headers: await _authProvider.getCustomHeaders(),
      );

      if (response.statusCode == 200) {
        //Lấy dữ liệu trả về từ server
        final responseData = json.decode(response.body);
        final removedContactId = responseData['contactId'];
        _contacts.removeWhere((contact) => contact.id == removedContactId);
        notifyListeners();
      } else {
        throw Exception('Failed to remove contact');
      }
    } catch (error) {
      print('Error removing contact: $error');
    }
  }
}
