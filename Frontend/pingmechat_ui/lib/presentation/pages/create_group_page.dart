import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../config/theme.dart';
import '../../data/models/chat_model.dart';
import '../../domain/models/account.dart';
import '../../providers/contact_provider.dart';
import '../../providers/chat_provider.dart';
import '../widgets/custom_circle_avatar.dart';
import '../widgets/custom_button.dart';

class CreateGroupPage extends StatefulWidget {
  const CreateGroupPage({super.key});

  @override
  _CreateGroupPageState createState() => _CreateGroupPageState();
}

class _CreateGroupPageState extends State<CreateGroupPage> {
  final TextEditingController _searchController = TextEditingController();
  final TextEditingController _groupNameController = TextEditingController();
  final ValueNotifier<Set<String>> _selectedContactIds = ValueNotifier<Set<String>>({});
  final ValueNotifier<List<Account>> _filteredContactUsers = ValueNotifier<List<Account>>([]);

  @override
  void initState() {
    super.initState();
    _searchController.addListener(_filterContacts);
  }

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();
    final contactProvider = Provider.of<ContactProvider>(context);
    _filteredContactUsers.value = contactProvider.contactUsers;
  }

  void _filterContacts() {
    final searchTerm = _searchController.text.toLowerCase();
    final contactProvider = Provider.of<ContactProvider>(context, listen: false);
    _filteredContactUsers.value = contactProvider.contactUsers
        .where((user) => user.fullName.toLowerCase().contains(searchTerm))
        .toList();
  }

  void _toggleContactSelection(String contactId) {
    final Set<String> updatedSelection = Set<String>.from(_selectedContactIds.value);
    if (updatedSelection.contains(contactId)) {
      updatedSelection.remove(contactId);
    } else {
      updatedSelection.add(contactId);
    }
    _selectedContactIds.value = updatedSelection;
  }

  Future<void> _createGroup() async {
    if (_selectedContactIds.value.length < 2) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Please select at least 2 members.')),
      );
      return;
    }

    final groupName = _groupNameController.text;
    final groupMembers = _selectedContactIds.value.toList();

    final chatProvider = Provider.of<ChatProvider>(context, listen: false);
    final chatCreateDto = ChatCreateDto(
      name: groupName,
      isGroup: true,
      userIds: groupMembers,
    );

    await chatProvider.startNewChat(chatCreateDto);
    // Navigator.of(context).pushNamedAndRemoveUntil('/home', (route) => false);
  }

  void _showAddMembersModal(BuildContext context) {
    _filterContacts();
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      builder: (BuildContext context) {
        return Container(
          height: MediaQuery.of(context).size.height * 0.9,
          padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 20),
          child: Column(
            children: [
              Container(
                height: 4,
                width: 40,
                margin: const EdgeInsets.symmetric(vertical: 8),
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
                  prefixIcon: const Icon(Icons.search, size: 24, color: AppColors.primary),
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(12),
                  ),
                ),
              ),
              Expanded(
                child: ValueListenableBuilder<List<Account>>(
                  valueListenable: _filteredContactUsers,
                  builder: (context, filteredUsers, child) {
                    return ListView.builder(
                      itemCount: filteredUsers.length,
                      itemBuilder: (context, index) {
                        final user = filteredUsers[index];
                        return ValueListenableBuilder<Set<String>>(
                          valueListenable: _selectedContactIds,
                          builder: (context, selectedIds, child) {
                            final isSelected = selectedIds.contains(user.id);
                            return ListTile(
                              leading: CustomCircleAvatar(
                                backgroundImage: user.avatarUrl != null ? NetworkImage(user.avatarUrl!) : null,
                              ),
                              title: Text(user.fullName),
                              trailing: Checkbox(
                                activeColor: AppColors.primary,
                                value: isSelected,
                                onChanged: (value) => _toggleContactSelection(user.id),
                              ),
                              onTap: () => _toggleContactSelection(user.id),
                            );
                          },
                        );
                      },
                    );
                  },
                ),
              ),
            ],
          ),
        );
      },
    );
  }

  @override
  void dispose() {
    _searchController.dispose();
    _groupNameController.dispose();
    _selectedContactIds.dispose();
    _filteredContactUsers.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Create Group'),
        leading: IconButton(
          icon: const Icon(Icons.arrow_back),
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
            const SizedBox(height: 20),
            const Text(
              'Group Name',
              style: TextStyle(color: Colors.grey, fontSize: 16),
            ),
            TextField(
              controller: _groupNameController,
              decoration: InputDecoration(
                hintText: 'Enter group name',
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(8),
                ),
                contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
              ),
            ),
            const SizedBox(height: 30),
            const Text(
              'Invited Members',
              style: TextStyle(color: Colors.grey, fontSize: 16),
            ),
            const SizedBox(height: 10),
            _buildInvitedMembers(context),
            const Spacer(),
            SizedBox(
              width: double.infinity,
              child: CustomElevatedButton(
                text: 'Create',
                onPressed: _createGroup,
                foregroundColor: AppColors.white,
                backgroundColor: AppColors.primary,
              ),
            ),
            const SizedBox(height: 20),
          ],
        ),
      ),
    );
  }

  Widget _buildInvitedMembers(BuildContext context) {
    return ValueListenableBuilder<Set<String>>(
      valueListenable: _selectedContactIds,
      builder: (context, selectedIds, child) {
        final contactProvider = Provider.of<ContactProvider>(context);
        List<Account> selectedMembers = selectedIds
            .map((id) => contactProvider.getContactUserById(id))
            .where((user) => user != null)
            .cast<Account>()
            .toList();

        return Wrap(
          spacing: 15,
          runSpacing: 10,
          children: [
            ...selectedMembers.map((user) {
              return Stack(
                children: [
                  CircleAvatar(
                    backgroundImage: user.avatarUrl != null ? NetworkImage(user.avatarUrl!) : null,
                    radius: 30,
                    child: user.avatarUrl == null ? Text(user.fullName[0]) : null,
                  ),
                  Positioned(
                    bottom: 0,
                    right: 0,
                    child: GestureDetector(
                      onTap: () => _toggleContactSelection(user.id),
                      child: Container(
                        decoration: BoxDecoration(
                          color: Colors.white,
                          shape: BoxShape.circle,
                          border: Border.all(color: Colors.grey),
                        ),
                        child: const Icon(Icons.close, color: Colors.grey, size: 20),
                      ),
                    ),
                  ),
                ],
              );
            }),
            GestureDetector(
              onTap: () => _showAddMembersModal(context),
              child: const CircleAvatar(
                backgroundColor: Colors.grey,
                radius: 30,
                child: Icon(Icons.add, color: Colors.white),
              ),
            ),
          ],
        );
      },
    );
  }
}