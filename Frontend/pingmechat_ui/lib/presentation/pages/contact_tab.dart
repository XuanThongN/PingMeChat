import 'package:flutter/material.dart';
import 'package:pingmechat_ui/config/theme.dart';

class ContactTab extends StatelessWidget {
  const ContactTab({super.key});

  @override
  Widget build(BuildContext context) {
    return DefaultTabController(
      length: 2,
      child: Scaffold(
        // backgroundColor: const Color(0xFF1E2746),
        backgroundColor: AppColors.background,
        body: SafeArea(
          child: Column(
            children: [
              _buildAppBar(),
              const TabBar(
                tabs: [
                  Tab(text: 'My Contacts'),
                  Tab(text: 'You May Know'),
                ],
                labelColor: Colors.white,
                unselectedLabelColor: Colors.white60,
                indicatorColor: Colors.white,
                dividerColor: Colors.transparent,
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
                      _buildMyContactsTab(),
                      _buildRecommendedFriendsTab(),
                    ],
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
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
            style: TextStyle(color: Colors.white, fontSize: 24, fontWeight: FontWeight.bold),
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

  Widget _buildMyContactsTab() {
    return ListView(
      padding: const EdgeInsets.only(top: 20),
      children: [
        _buildContactSection('A', [
          _ContactItem('Afrin Sabila', 'Life is beautiful 👌', 'assets/afrin.jpg'),
          _ContactItem('Adil Adnan', 'Be your own hero 💪', 'assets/adil.jpg'),
        ]),
        _buildContactSection('B', [
          _ContactItem('Bristy Haque', 'Keep working ✍️', 'assets/bristy.jpg'),
          _ContactItem('John Borino', 'Make yourself proud 😍', 'assets/john.jpg'),
          _ContactItem('Borsha Akther', 'Flowers are beautiful 🌸', 'assets/borsha.jpg'),
        ]),
        _buildContactSection('S', [
          _ContactItem('sheik Sadi', '', 'assets/sheik.jpg'),
        ]),
      ],
    );
  }

  Widget _buildRecommendedFriendsTab() {
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
        _buildRecommendedFriend('Emily Johnson', 'Based on your contacts', 'assets/emily.jpg'),
        _buildRecommendedFriend('Michael Lee', 'Works at Tech Co.', 'assets/michael.jpg'),
        _buildRecommendedFriend('Sarah Williams', 'Lives in your city', 'assets/sarah.jpg'),
      ],
    );
  }

  Widget _buildContactSection(String letter, List<_ContactItem> contacts) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 10),
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

  Widget _buildContactTile(_ContactItem contact) {
    return ListTile(
      leading: CircleAvatar(
        backgroundImage: AssetImage(contact.imagePath),
        radius: 25,
      ),
      title: Text(
        contact.name,
        style: const TextStyle(
          color: Color(0xFF1E2746),
          fontSize: 16,
          fontWeight: FontWeight.bold,
        ),
      ),
      subtitle: Text(
        contact.status,
        style: TextStyle(color: Colors.grey[600], fontSize: 14),
      ),
    );
  }

  Widget _buildRecommendedFriend(String name, String reason, String imagePath) {
    return ListTile(
      leading: CircleAvatar(
        backgroundImage: AssetImage(imagePath),
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

class _ContactItem {
  final String name;
  final String status;
  final String imagePath;

  _ContactItem(this.name, this.status, this.imagePath);
}