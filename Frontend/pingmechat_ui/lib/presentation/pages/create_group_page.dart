import 'package:flutter/material.dart';
import 'package:pingmechat_ui/presentation/widgets/custom_button.dart';

import '../../config/theme.dart';

class CreateGroupPage extends StatefulWidget {
  @override
  _CreateGroupPageState createState() => _CreateGroupPageState();
}

class _CreateGroupPageState extends State<CreateGroupPage> {
  List<User> users = [
    User(
        id: 1,
        name: 'Rashid Khan',
        imageUrl: 'https://via.placeholder.com/150'),
    User(
        id: 2,
        name: 'David Wayne',
        imageUrl: 'https://via.placeholder.com/150'),
    User(id: 3, name: 'John Doe', imageUrl: 'https://via.placeholder.com/150'),
    User(id: 4, name: 'Jane Doe', imageUrl: 'https://via.placeholder.com/150'),
    User(id: 5, name: 'Alice', imageUrl: 'https://via.placeholder.com/150'),
    User(id: 6, name: 'Bob', imageUrl: 'https://via.placeholder.com/150'),
    User(id: 7, name: 'User 1', imageUrl: 'https://via.placeholder.com/150'),
    User(id: 8, name: 'User 2', imageUrl: 'https://via.placeholder.com/150'),
    User(id: 9, name: 'User 3', imageUrl: 'https://via.placeholder.com/150'),
    User(id: 10, name: 'User 4', imageUrl: 'https://via.placeholder.com/150'),
    User(id: 11, name: 'User 5', imageUrl: 'https://via.placeholder.com/150'),
    User(id: 12, name: 'User 6', imageUrl: 'https://via.placeholder.com/150'),
    User(id: 13, name: 'User 7', imageUrl: 'https://via.placeholder.com/150'),
    User(id: 14, name: 'User 8', imageUrl: 'https://via.placeholder.com/150'),
  ];

  TextEditingController _searchController = TextEditingController();
  Set<int> _selectedUserIds = Set<int>(); // Store selected user IDs
  List<User> _filteredUsers = []; // Store filtered users based on search query

  @override
  void initState() {
    super.initState();
    _filteredUsers = users;
    _searchController.addListener(_filterUsers);
  }

  void _filterUsers() {
    setState(() {
      _filteredUsers = users.where((user) {
        return user.name
            .toLowerCase()
            .contains(_searchController.text.toLowerCase());
      }).toList();
    });
  }

  void _toggleUserSelection(int userId) {
    setState(() {
      if (_selectedUserIds.contains(userId)) {
        _selectedUserIds.remove(userId);
      } else {
        _selectedUserIds.add(userId);
      }
    });
  }

  void _toggleAllUserSelection() {
    setState(() {
      if (_selectedUserIds.length == users.length) {
        _selectedUserIds.clear();
      } else {
        _selectedUserIds = users.map((user) => user.id).toSet();
      }
    });
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('Create Group'),
        leading: IconButton(
          icon: Icon(Icons.arrow_back),
          onPressed: () => Navigator.of(context).pop(),
        ),
        elevation: 0,
        backgroundColor: Colors.transparent,
        foregroundColor: Colors.black,
      ),
      body: Padding(
        padding: const EdgeInsets.symmetric(horizontal: 20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            SizedBox(height: 20),
            Text(
              'Group Name',
              style: TextStyle(color: Colors.grey, fontSize: 16),
            ),
            // Group Name TextField
            TextField(
              decoration: InputDecoration(
                hintText: 'Enter group name',
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(8),
                ),
                // Add padding to the text field
                contentPadding:
                    EdgeInsets.symmetric(horizontal: 16, vertical: 12),
              ),
            ),
            SizedBox(height: 30),
            Text(
              'Invited Members',
              style: TextStyle(color: Colors.grey, fontSize: 16),
            ),
            SizedBox(height: 10),
            _buildInvitedMembers(context),
            Spacer(),
            SizedBox(
              width: double.infinity,
              child: CustomElevatedButton(
                text: 'Create',
                onPressed: () {},
                foregroundColor: AppColors.white,
                backgroundColor: AppColors.primary,
              ),
            ),
            SizedBox(height: 20),
          ],
        ),
      ),
    );
  }

  Widget _buildChip(String label) {
    return Chip(
      label: Text(label),
      backgroundColor: Colors.teal.shade50,
      padding: EdgeInsets.symmetric(horizontal: 10, vertical: 5),
    );
  }

  Widget _buildInvitedMembers(BuildContext context) {
    List<String> memberImages = users
        .where((user) => _selectedUserIds.contains(user.id))
        .map((user) => user.imageUrl)
        .toList();

    return Wrap(
      spacing: 15,
      runSpacing: 10,
      children: [
        ...memberImages.map((imageUrl) {
          return Stack(
            children: [
              CircleAvatar(
                backgroundImage: NetworkImage(imageUrl),
                radius: 30,
              ),
              Positioned(
                bottom: 0,
                right: 0,
                child: CircleAvatar(
                  backgroundColor: AppColors.grey,
                  radius: 12,
                  child: Icon(Icons.add, color: AppColors.secondary, size: 20),
                ),
              ),
            ],
          );
        }).toList(),
        GestureDetector(
          onTap: () {
            showModalBottomSheet(
              context: context,
              isScrollControlled: true,
              builder: (context) {
                return StatefulBuilder(
                  builder: (BuildContext context, StateSetter setState) {
                    return Container(
                      height: MediaQuery.of(context).size.height * 0.9,
                      padding:
                          EdgeInsets.symmetric(horizontal: 20, vertical: 20),
                      child: Column(
                        children: [
                          Container(
                            height: 4,
                            width: 40,
                            margin: EdgeInsets.symmetric(vertical: 8),
                            decoration: BoxDecoration(
                              color: AppColors.tertiary,
                              borderRadius: BorderRadius.circular(2),
                            ),
                          ),
                          const Text(
                            'Add members to group',
                            style: TextStyle(
                              fontSize: 16,
                              fontWeight: FontWeight.bold,
                            ),
                          ),
                          TextField(
                            controller: _searchController,
                            decoration: InputDecoration(
                              hintText: 'Search members',
                              prefixIcon: Icon(Icons.search,
                                  size: 24, color: AppColors.primary),
                              border: OutlineInputBorder(
                                borderRadius: BorderRadius.circular(12),
                              ),
                            ),
                          ),
                          Expanded(
                            child: SingleChildScrollView(
                              child: Column(
                                children: [
                                  CheckboxListTile(
                                    title: Text('Select all'),
                                    // Check if all users are selected
                                    value:
                                        _selectedUserIds.length == users.length,
                                    onChanged: (value) {
                                      setState(() {
                                        _toggleAllUserSelection();
                                      });
                                    },
                                  ),
                                  ..._filteredUsers.map((user) {
                                    return ListTile(
                                      leading: CircleAvatar(
                                        backgroundImage:
                                            NetworkImage(user.imageUrl),
                                      ),
                                      title: Text(user.name),
                                      trailing: Checkbox(
                                        activeColor: AppColors.primary,
                                        value:
                                            _selectedUserIds.contains(user.id),
                                        onChanged: (value) {
                                          setState(() {
                                            _toggleUserSelection(user.id);
                                          });
                                        },
                                      ),
                                    );
                                  }).toList(),
                                ],
                              ),
                            ),
                          ),
                        ],
                      ),
                    );
                  },
                );
              },
            );
          },
          child: Column(
            children: [
              CircleAvatar(
                backgroundColor: Colors.grey.shade200,
                radius: 30,
                child: Icon(Icons.add, color: Colors.grey),
              ),
              SizedBox(height: 5),
            ],
          ),
        ),
      ],
    );
  }
}

class User {
  final int id;
  final String name;
  final String imageUrl;

  User({required this.id, required this.name, required this.imageUrl});
}
