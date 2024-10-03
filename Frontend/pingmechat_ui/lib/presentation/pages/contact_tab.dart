import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';
import 'package:pingmechat_ui/config/theme.dart';
import 'package:pingmechat_ui/core/constants/constant.dart';
import 'package:pingmechat_ui/domain/models/contact.dart';
import 'package:pingmechat_ui/presentation/widgets/custom_circle_avatar.dart';
import 'package:pingmechat_ui/providers/auth_provider.dart';
import 'package:pingmechat_ui/providers/contact_provider.dart';
import 'package:provider/provider.dart';

import '../../main.dart';
import '../../providers/badge_provider.dart';
import '../../providers/chat_provider.dart';

class ContactTab extends StatefulWidget {
  const ContactTab({super.key});

  @override
  State<ContactTab> createState() => _ContactTabState();
}

class _ContactTabState extends State<ContactTab> {
  late ContactProvider _contactProvider;
  late ChatProvider _chatProvider;
  @override
  void initState() {
    super.initState();
    _contactProvider = context.read<ContactProvider>();
    _chatProvider = context.read<ChatProvider>();
  }

  @override
  Widget build(BuildContext context) {
    return DefaultTabController(
        length: 2,
        child: Scaffold(
          backgroundColor: AppColors.background,
          body: SafeArea(
            child: Consumer2<ContactProvider, AuthProvider>(
              builder: (context, contactProvider, authProvider, child) {
                final contacts = contactProvider.getAllContacts();
                final recommendContacts =
                    contactProvider.getRecommendContacts();
                final pendingContacts = contactProvider.friendRequests;
                final friends = contactProvider.friends;

                // Hiển thị danh sách liên hệ và bạn bè
                return Column(
                  children: [
                    _buildAppBar(),
                    const TabBar(
                      labelColor: Colors.white,
                      unselectedLabelColor: Colors.white60,
                      indicatorColor: Colors.white,
                      dividerColor: Colors.transparent,
                      tabs: [
                        Tab(text: 'My Contacts'),
                        Tab(text: 'You May Know'),
                      ],
                    ),
                    Expanded(
                      child: Container(
                        decoration: const BoxDecoration(
                          color: Colors.white,
                          borderRadius: BorderRadius.only(
                            topLeft: Radius.circular(30),
                            topRight: Radius.circular(30),
                          ),
                        ),
                        child: TabBarView(
                          children: [
                            _buildMyContactsTab(friends, pendingContacts),
                            _buildRecommendedFriendsTab(recommendContacts),
                          ],
                        ),
                      ),
                    ),
                  ],
                );
              },
            ),
          ),
        ));
  }

  Widget _buildAppBar() {
    return Padding(
      padding: const EdgeInsets.all(16),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          IconButton(
            icon: const Icon(Icons.search, color: Colors.white, size: 30),
            onPressed: () {
              // TODO: Implement search functionality
            },
          ),
          const Text(
            'Contacts',
            style: TextStyle(
                color: Colors.white, fontSize: 24, fontWeight: FontWeight.bold),
          ),
          IconButton(
            icon: const Icon(Icons.person_add, color: Colors.white, size: 30),
            onPressed: () {
              // TODO: Implement add contact functionality
            },
          ),
        ],
      ),
    );
  }

  Widget _buildMyContactsTab(
      List<Contact> acceptedContacts, List<Contact> pendingSortedContacts) {
    // Sắp xếp danh sách liên hệ đã chấp nhận theo ngày thêm mới nhất
    pendingSortedContacts
        .sort((a, b) => b.createdDate!.compareTo(a.createdDate!));

    // Tạo map chứa danh sách liên hệ đã chấp nhận theo chữ cái đầu tiên của tên
    final acceptedContactsByLetter = _groupContactsByLetter(acceptedContacts);

    return ListView(
      padding: const EdgeInsets.only(top: 20),
      children: [
        _buildPendingSection(
          title: 'Pending Invitations',
          pendingContacts: pendingSortedContacts,
        ),
        const SizedBox(height: 20),
        _buildContactListSection(
          title: 'Friends',
          contactsByLetter: acceptedContactsByLetter,
        ),
      ],
    );
  }

  Widget _buildContactListSection({
    required String title,
    required Map<String, List<Contact>> contactsByLetter,
  }) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 10),
          child: Text(
            title,
            style: const TextStyle(
              color: Color(0xFF1E2746),
              fontSize: 20,
              fontWeight: FontWeight.bold,
            ),
          ),
        ),
        ...contactsByLetter.entries
            .map((entry) => _buildContactSection(entry.key, entry.value))
            .toList(),
      ],
    );
  }

  Widget _buildContactSection(String letter, List<Contact> contacts) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 5),
          child: Text(
            letter,
            style: const TextStyle(
              color: Color(0xFF1E2746),
              fontSize: 18,
              fontWeight: FontWeight.bold,
            ),
          ),
        ),
        ...contacts.map((contact) => _buildContactTile(contact)).toList(),
      ],
    );
  }

  Map<String, List<Contact>> _groupContactsByLetter(List<Contact> contacts) {
    final contactsByLetter = <String, List<Contact>>{};
    contacts.sort((a, b) => a.fullName!.compareTo(b.fullName!)); // Sắp xếp tên
    for (var contact in contacts) {
      final letter = contact.fullName![0].toUpperCase(); // Lấy chữ cái đầu
      if (contactsByLetter.containsKey(letter)) {
        contactsByLetter[letter]!.add(contact);
      } else {
        contactsByLetter[letter] = [contact];
      }
    }
    return contactsByLetter;
  }

// Phần hiển thị danh sách liên hệ đang chờ với action button
  Widget _buildPendingSection({
    required String title,
    required List<Contact> pendingContacts,
  }) {
    if (pendingContacts.isEmpty) {
      return const SizedBox.shrink();
    }
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 10),
          child: Text(
            title,
            style: const TextStyle(
              color: Color(0xFF1E2746),
              fontSize: 20,
              fontWeight: FontWeight.bold,
            ),
          ),
        ),
        ...pendingContacts
            .map((contact) => _buildPendingContactTile(contact))
            .toList(),
      ],
    );
  }

// Widget hiển thị contact trong phần "Pending"
  Widget _buildPendingContactTile(Contact contact) {
    return ListTile(
      title: Text(contact.fullName ?? 'Unnamed Contact',
          style: const TextStyle(
            color: Color(0xFF1E2746),
            fontSize: 16,
            fontWeight: FontWeight.bold,
          )),
      subtitle: const Text('Sent you a friend request',
          style: TextStyle(
            color: Color(0xFF1E2746),
            fontSize: 14,
            fontWeight: FontWeight.bold,
          )),
      leading: CustomCircleAvatar(
        backgroundImage: contact.avatarUrl != null
            ? CachedNetworkImageProvider(contact.avatarUrl!)
            : null,
        radius: 25,
      ),
      trailing: IconButton(
        icon: const Icon(Icons.more_vert),
        onPressed: () => _showActionSheet(contact),
      ),
    );
  }

  void _showActionSheet(Contact contact) {
    // Sử dụng showModalBottomSheet để hiển thị Bottom Sheet
    showModalBottomSheet(
      context: navigatorKey
          .currentContext!, // Đảm bảo sử dụng context đúng của widget
      builder: (BuildContext context) {
        return Container(
          padding: const EdgeInsets.symmetric(vertical: 20),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              ListTile(
                leading: const Icon(Icons.check_circle, color: Colors.green),
                title: const Text('Accept'),
                onTap: () async {
                  // Thực hiện logic "Accept" ở đây
                  String status = await _contactProvider
                      .acceptFriendRequest(contact.user!.id);
                  // Nếu status là ACCEPTED thì thông báo
                  if (status == ContactStatus.ACCEPTED) {
                    ScaffoldMessenger.of(context).showSnackBar(
                      SnackBar(
                        content: Text(
                            'Accepted friend request from ${contact.user!.fullName}'),
                      ),
                    );
                  }
                  // Đóng Bottom Sheet
                  Navigator.pop(context);
                },
              ),
              ListTile(
                leading: const Icon(Icons.cancel, color: Colors.red),
                title: const Text('Reject'),
                onTap: () async {
                  // Thực hiện logic "Reject" ở đây
                  // Thực hiện logic "Accept" ở đây
                  String status = await _contactProvider
                      .cancelFriendRequest(contact.user!.id);
                  // Nếu status là ACCEPTED thì thông báo
                  if (status == ContactStatus.CANCELLED) {
                    ScaffoldMessenger.of(context).showSnackBar(SnackBar(
                      content: Text(
                          'Rejected friend request from ${contact.user!.fullName}'),
                    ));
                  }
                  // Đóng Bottom Sheet
                  Navigator.pop(context);
                },
              ),
            ],
          ),
        );
      },
    );
  }

  Widget _buildRecommendedFriendsTab(List<Contact> recommendContacts) {
    return ListView(
      padding: const EdgeInsets.only(top: 20),
      children: [
        const Padding(
          padding: EdgeInsets.symmetric(horizontal: 20, vertical: 10),
          child: Text(
            'People You May Know',
            style: TextStyle(
              color: Color(0xFF1E2746),
              fontSize: 18,
              fontWeight: FontWeight.bold,
            ),
          ),
        ),
        ...recommendContacts
            .map((contact) => _buildRecommendedFriend(
                contact.fullName!,
                'Based on your contacts', // You can customize this based on your logic
                contact.avatarUrl!))
            .toList(),
      ],
    );
  }

  Widget _buildContactTile(Contact contact) {
    return InkWell(
      onTap: () {
        print('Start chat with ${contact.fullName}');
          // _chatProvider.startNewChat(contact);
      },
      highlightColor: Colors.grey[200],
      splashColor: Colors.grey[300], // Màu splash khi click
      child: ListTile(
        leading: CustomCircleAvatar(
          backgroundImage: contact.avatarUrl != null
              ? CachedNetworkImageProvider(contact.avatarUrl!)
              : null,
          radius: 25,
        ),
        title: Text(
          contact.fullName!,
          style: const TextStyle(
            color: Color(0xFF1E2746),
            fontSize: 16,
            fontWeight: FontWeight.bold,
          ),
        ),
        subtitle: Text(
          contact.email ?? contact.phoneNumber!,
          style: TextStyle(color: Colors.grey[600], fontSize: 14),
        ),
      ),
    );
  }

  Widget _buildRecommendedFriend(String name, String reason, String avatarUrl) {
    return ListTile(
      leading: CustomCircleAvatar(
        backgroundImage:
            avatarUrl.isNotEmpty ? CachedNetworkImageProvider(avatarUrl) : null,
        radius: 25,
      ),
      title: Text(
        name,
        style: const TextStyle(
          color: Color(0xFF1E2746),
          fontSize: 16,
          fontWeight: FontWeight.bold,
        ),
      ),
      subtitle: Text(
        reason,
        style: TextStyle(color: Colors.grey[600], fontSize: 14),
      ),
      trailing: ElevatedButton(
        onPressed: () {
          // TODO: Implement add friend functionality
        },
        child: const Text('Add', style: TextStyle(color: AppColors.white)),
        style: ElevatedButton.styleFrom(
          backgroundColor: AppColors.primary,
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(20),
          ),
        ),
      ),
    );
  }
}
