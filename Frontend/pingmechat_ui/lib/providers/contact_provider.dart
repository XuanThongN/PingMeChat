import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:pingmechat_ui/domain/models/chat.dart';
import 'package:pingmechat_ui/providers/auth_provider.dart';
import 'package:pingmechat_ui/providers/badge_provider.dart';
import 'dart:convert';

import '../core/constants/constant.dart';
import '../domain/models/account.dart';
import '../domain/models/contact.dart';
import '../domain/models/userchat.dart';
import 'chat_provider.dart';

class ContactProvider extends ChangeNotifier {
  final AuthProvider _authProvider;
  final BadgeProvider _badgeProvider;
  final ChatProvider _chatProvider;
  ContactProvider(this._authProvider, this._badgeProvider, this._chatProvider);

  List<Contact> _contacts = [];
  List<Account> _contactUsers = [];
  List<Contact> _friendRequests = [];
  List<Contact> _friends = [];
  List<Account> _recommendedFriends = [];
  bool _isLoading = false;

  List<Contact> get contacts => _contacts;
  List<Account> get contactUsers => _contactUsers;
  bool get isLoading => _isLoading;
  Account get currentUser => _authProvider.currentUser!;
  List<Contact> get friendRequests => _friendRequests;
  List<Contact> get friends => _friends;
  List<Account> get recommendedFriends => _recommendedFriends;
  List<Contact> getAllContacts() {
    return _contacts
        .map((contact) => _getContactUser(contact, currentUser.id))
        .toList();
  }

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

      _friendRequests = getFriendRequests(); // Lấy danh sách yêu cầu kết bạn
      _friends = getFriends(); // Lấy danh sách bạn bè
      // Cập nhật badge count
      _badgeProvider.updateContactsCount(_friendRequests.length);
    } catch (error) {
      print('Error fetching contacts: $error');
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

// hàm để lấy danh sách bạn bè từ danh sách contacts
  List<Contact> getFriends() {
    return contacts.where((c) => c.status == ContactStatus.ACCEPTED).toList();
  }

  // Hàm để lấy danh sách yêu cầu kết bạn từ danh sách contacts
  List<Contact> getFriendRequests() {
    return contacts.where((c) => c.status == ContactStatus.PENDING).toList();
  }

  void _updateListFriendAndFriendRequest() {
    _friendRequests = getFriendRequests();
    _friends = getFriends();
  }

  // Hàm mới để cập nhật danh sách contactUsers
  void _updateContactUsers() {
    _contactUsers.clear(); // clear danh sách contactUsers
    _contactUsers = _contacts
        .map((contact) {
          if (contact.status == ContactStatus.ACCEPTED) {
            return contact.user?.id != currentUser.id
                ? contact.user
                : contact.contactUser;
          }
        })
        .where((user) => user != null)
        .cast<Account>()
        .toList();
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
            _contacts.indexWhere((contact) => contact.id == updatedContact.id);
        if (index != -1) {
          _contacts[index].status = updatedContact.status;
          _updateContactUsers();
          // Cập nhật danh sách bạn bè và yêu cầu kết bạn
          _updateListFriendAndFriendRequest();
          // Cập nhật badge count
          _badgeProvider.updateContactsCount(_friendRequests.length);
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
        _contacts.removeWhere((contact) => contact.user!.id == contactId);
        _updateContactUsers();
        // Cập nhật danh sách bạn bè và yêu cầu kết bạn
        _updateListFriendAndFriendRequest();
        // Cập nhật badge count
        _badgeProvider.updateContactsCount(_friendRequests.length);
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
        // Thay đổi trạng thái của contact
        newContact.status = ContactStatus.REQUESTED;
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

  Contact? getContact(String userId) {
    return _contacts.firstWhere((contact) => contact.id == userId);
  }

  void clearData() {
    _contacts = [];
    _contactUsers = [];
    notifyListeners();
  }

  Contact _getContactUser(Contact contact, String currentUserId) {
    final contactUser =
        contact.user!.id == currentUserId ? contact.contactUser : contact.user;
    if (contactUser != null) {
      contact.fullName = contactUser.fullName;
      contact.avatarUrl = contactUser.avatarUrl;
      contact.nickname = contactUser.email;
      contact.phoneNumber = contactUser.phoneNumber;
      contact.email = contactUser.email;
    }
    return contact;
  }

  Future<void> fetchRecommendedFriends() async {
    try {
      final response = await http.get(
        Uri.parse(ApiConstants.getRecommendedFriendsEndpoint),
        headers: await _authProvider.getCustomHeaders(),
      );

      if (response.statusCode == 200) {
        final jsonResponse = json.decode(response.body);

        // Lấy phần 'result' và truy cập vào 'data' trong đó
        final List<dynamic> data = jsonResponse['result'];
        if (data.isNotEmpty) {
          _recommendedFriends =
              data.map((data) => Account.fromJson(data)).toList();
        } else {
          _recommendedFriends = [];
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

  // Lấy danh sách bạn bè kèm theo trạng thái online/offline từ api
  Future<void> fetchFriendStatus() async {
    try {
      final response = await http.get(
        Uri.parse(ApiConstants.getFriendStatusEndpoint),
        headers: await _authProvider.getCustomHeaders(),
      );

      if (response.statusCode == 200) {
        final jsonResponse = json.decode(response.body);

        // Lấy phần 'result' và truy cập vào 'data' trong đó
        final Map<String, dynamic> data = jsonResponse['result'];
        List<FriendStatus> _friendsStatus = [];
        if (data.isNotEmpty) {
          _friendsStatus = data
              .map((key, value) =>
                  MapEntry(key, FriendStatus(userId: key, status: value)))
              .values
              .toList();

          // Cập nhật trạng thái của bạn bè
          updateFriendStatus(_friendsStatus);
        }
      } else {
        throw Exception('Failed to load friends statuses');
      }
    } catch (error) {
      print('Error fetching friends statuses: $error');
    }
  }

  // Cập nhật lại trạng thái của bạn bè trong _friends dựa vào trạng thái online/offline từ api
  void updateFriendStatus(List<FriendStatus> friendStatuses) {
    final currentUserId = _authProvider.currentUser!.id;
    for (var friendStatus in friendStatuses) {
      final index = _friends.indexWhere((friend) =>
          friendStatus.userId ==
          (friend.user!.id == currentUserId
              ? friend.contactUser!.id
              : friend.user!.id));
      if (index != -1) {
        _friends[index].isOnline = friendStatus.status;
      }
    }

    // Cập nhật lại trạng thái của contactUsers
    for (var friendStatus in friendStatuses) {
      final index = _contactUsers
          .indexWhere((friend) => friend.id == friendStatus.userId);
      if (index != -1) {
        _contactUsers[index].isOnline = friendStatus.status;
      }
    }

    // Cập nhật lại trạng thái của các userchat của tất cả các chat
    final chats = _chatProvider.chats;
    for (var friendStatus in friendStatuses) {
      for (var chat in chats) {
        final userChat = chat.userChats.firstWhere(
            (user) => user.id == friendStatus.userId,
            orElse: () =>
                UserChat(id: '', chatId: '', userId: '', isAdmin: false));
        if (userChat.id.isNotEmpty) {
          userChat.isOnline = friendStatus.status;
        }
      }
    }

    sortFriendsByOnlineStatus(); // Sắp xếp danh sách bạn bè theo trạng thái online/offline
    notifyListeners();
  }

// Sắp xếp danh sách bạn bè theo trạng thái online/offline
  void sortFriendsByOnlineStatus() {
    _friends.sort((a, b) {
      if (a.isOnline == b.isOnline) {
        return 0;
      } else if (a.isOnline) {
        return -1;
      } else {
        return 1;
      }
    });

    contactUsers.sort((a, b) {
      if (a.isOnline == b.isOnline) {
        return 0;
      } else if (a.isOnline) {
        return -1;
      } else {
        return 1;
      }
    });
    notifyListeners();
  }
}
