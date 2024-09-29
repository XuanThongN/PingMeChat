import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:pingmechat_ui/providers/auth_provider.dart';
import 'dart:convert';

import '../core/constants/constant.dart';
import '../domain/models/account.dart';
import '../domain/models/contact.dart';

class ContactProvider extends ChangeNotifier {
  final AuthProvider _authProvider;
  ContactProvider(this._authProvider);

  List<Contact> _contacts = [];
  List<Account> _contactUsers = [];
  bool _isLoading = false;

  List<Contact> get contacts => _contacts;
  List<Account> get contactUsers => _contactUsers;
  bool get isLoading => _isLoading;
  Account get currentUser => _authProvider.currentUser!;
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
          _updateContactUsers();
        } else {
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

  // Hàm mới để cập nhật danh sách contactUsers
  void _updateContactUsers() {
    _contactUsers = _contacts.map((contact) {
      return contact.user?.id != currentUser.id
          ? contact.user!
          : contact.contactUser!;
    }).toList();
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
        _updateContactUsers();
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
        _updateContactUsers();
        notifyListeners();
      } else {
        throw Exception('Failed to remove contact');
      }
    } catch (error) {
      print('Error removing contact: $error');
    }
  }

  // Hàm tiện ích để lấy contactUser dựa trên id
  Account? getContactUserById(String id) {
    return _contactUsers.firstWhere((user) => user.id == id,
        orElse: () => Account(id: '', fullName: '', email: ''));
  }

  // Xử lý kết bạn
  Future<String> acceptFriendRequest(String contactId) async {
    try {
      final response = await http.put(
        Uri.parse('${ApiConstants.acceptFriendRequestEndpoint}/$contactId'),
        headers: await _authProvider.getCustomHeaders(),
      );

      if (response.statusCode == 200) {
        final responseData = json.decode(response.body);
        final result = responseData['result'];
        final updatedContact = Contact.fromJson(result);
        final index =
            _contacts.indexWhere((contact) => contact.id == contactId);
        if (index != -1) {
          _contacts[index] = updatedContact;
          _updateContactUsers();
          notifyListeners();
        }
        return ContactStatus.ACCEPTED;
      } else {
        throw Exception('Failed to accept friend request');
      }
    } catch (error) {
      print('Error accepting friend request: $error');
    }
    return ContactStatus.STRANGER;
  }

  // Hủy yêu cầu kết bạn
  Future<String> cancelFriendRequest(String contactId) async {
    try {
      final response = await http.delete(
        Uri.parse('${ApiConstants.cancelFriendRequestEndpoint}/$contactId'),
        headers: await _authProvider.getCustomHeaders(),
      );

      if (response.statusCode == 200) {
        _contacts.removeWhere((contact) => contact.id == contactId);
        _updateContactUsers();
        notifyListeners();
        return ContactStatus.CANCELLED;
      } else {
        throw Exception('Failed to cancel friend request');
      }
    } catch (error) {
      print('Error cancelling friend request: $error');
    }
    return ContactStatus.ACCEPTED;
  }

  // Gửi yêu cầu kết bạn  
  Future<String> sendFriendRequest(String userId) async {
    try {
      final response = await http.post(
        Uri.parse(ApiConstants.sendFriendRequestEndpoint),
        headers: {
          'Content-Type': 'application/json ; charset=UTF-8',
          ...await _authProvider.getCustomHeaders(),
        },
        body: json.encode({'contactUserId': userId}),
      );

      if (response.statusCode == 201) {
        final responseData = json.decode(response.body);
        final result = responseData['result'];
        final newContact = Contact.fromJson(result);
        _contacts.add(newContact);
        _updateContactUsers();
        notifyListeners();
        return ContactStatus.REQUESTED;
      } else {
        throw Exception('Failed to send friend request');
      }
    } catch (error) {
      print('Error sending friend request: $error');
    }
    return ContactStatus.STRANGER;
  }
}
