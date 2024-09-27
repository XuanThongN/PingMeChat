import 'dart:async';

import 'package:flutter/material.dart';
import 'package:pingmechat_ui/presentation/widgets/custom_circle_avatar.dart';
import 'package:pingmechat_ui/providers/search_provider.dart';
import 'package:provider/provider.dart';

import '../../config/theme.dart';
import '../../data/models/search_result.dart';

class SearchResultsScreen extends StatefulWidget {
  static const routeName = '/search';
  @override
  _SearchResultsScreenState createState() => _SearchResultsScreenState();
}

class _SearchResultsScreenState extends State<SearchResultsScreen> {
  final TextEditingController _searchController = TextEditingController();
  Timer? _debounce;

  @override
  void initState() {
    super.initState();
    _searchController.addListener(_onSearchChanged);
  }

  _onSearchChanged() {
    if (_debounce?.isActive ?? false) _debounce!.cancel();
    _debounce = Timer(const Duration(milliseconds: 1000), () {
      if (_searchController.text.isNotEmpty) {
        Provider.of<SearchProvider>(context, listen: false)
            .search(_searchController.text.trim());
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      body: SafeArea(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            SearchBar(
                controller: _searchController,
                onBackPressed: () => Navigator.pop(context),
                onChanged: (value) => _onSearchChanged()),
            Expanded(
              child: Consumer<SearchProvider>(
                builder: (context, searchProvider, child) {
                  if (searchProvider.isLoading) {
                    return Center(child: CircularProgressIndicator());
                  }
                  if (searchProvider.searchResult == null) {
                    return Center(child: Text('No results found'));
                  }
                  return ListView(
                    children: [
                      if (searchProvider.searchResult!.users.isNotEmpty) ...[
                        SectionTitle(title: 'People'),
                        ...searchProvider.searchResult!.users
                            .map((user) => UserTile(user: user)),
                      ],
                      if (searchProvider
                          .searchResult!.groupChats.isNotEmpty) ...[
                        SectionTitle(title: 'Group Chat'),
                        ...searchProvider.searchResult!.groupChats
                            .map((chat) => GroupTile(groupChat: chat)),
                      ],
                    ],
                  );
                },
              ),
            ),
          ],
        ),
      ),
    );
  }

  @override
  void dispose() {
    _searchController.removeListener(_onSearchChanged);
    _searchController.dispose();
    _debounce?.cancel();
    super.dispose();
  }
}

class SearchBar extends StatelessWidget {
  final TextEditingController controller;
  final VoidCallback onBackPressed;
  final ValueChanged<String> onChanged;

  SearchBar({
    required this.controller,
    required this.onBackPressed,
    required this.onChanged,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      decoration: BoxDecoration(
        color: AppColors.surface,
        borderRadius: BorderRadius.circular(20),
        boxShadow: [
          BoxShadow(
            color: AppColors.primary.withOpacity(0.1),
            blurRadius: 10,
            offset: Offset(0, 3),
          ),
        ],
      ),
      child: Row(
        children: [
          IconButton(
            icon: Icon(Icons.arrow_back, color: AppColors.primary),
            onPressed: onBackPressed,
          ),
          Expanded(
            child: TextField(
              controller: controller,
              onChanged: onChanged,
              decoration: InputDecoration(
                hintText: 'Search people...',
                border: InputBorder.none,
                hintStyle: TextStyle(color: AppColors.tertiary, fontSize: 16),
              ),
              style: TextStyle(color: AppColors.primary, fontSize: 16),
            ),
          ),
          AnimatedOpacity(
            opacity: controller.text.isNotEmpty ? 1.0 : 0.0,
            duration: Duration(milliseconds: 200),
            child: IconButton(
              icon: Icon(Icons.close, color: AppColors.primary),
              onPressed: () {
                controller.clear();
                onChanged('');
              },
            ),
          ),
        ],
      ),
    );
  }
}

class SectionTitle extends StatelessWidget {
  final String title;

  SectionTitle({required this.title});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      child: Text(
        title,
        style: TextStyle(fontSize: 22, fontWeight: FontWeight.bold),
      ),
    );
  }
}

class UserTile extends StatelessWidget {
  final User user;

  UserTile({required this.user});

  @override
  Widget build(BuildContext context) {
    return ListTile(
      contentPadding: EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      leading: CustomCircleAvatar(
        radius: 28,
        backgroundImage: user.avatarUrl!.isNotEmpty
            ? NetworkImage(user.avatarUrl!)
            : null,
      ),
      title: Text(user.fullName,
          style: TextStyle(fontWeight: FontWeight.bold, fontSize: 18)),
      subtitle: Text(user.userName, style: TextStyle(fontSize: 14)),
      trailing: !user.isFriend
          ? IconButton(
              icon: Icon(Icons.person_add),
              onPressed: () {
                // TODO: Implement add friend functionality
                ScaffoldMessenger.of(context).showSnackBar(
                  SnackBar(content: Text('Add friend request sent to ${user.fullName}')),
                );
              },
            )
          : null,
      onTap: () {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Selected ${user.fullName}')),
        );
      },
    );
  }
}

class GroupTile extends StatelessWidget {
  final GroupChat groupChat;

  GroupTile({required this.groupChat});

  @override
  Widget build(BuildContext context) {
    return ListTile(
      contentPadding: EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      leading: SizedBox(
        width: 56,
        height: 56,
        child: groupChat.avatarUrl != null
            ? CustomCircleAvatar(
                backgroundImage: NetworkImage(groupChat.avatarUrl!),
              )
            : Stack(
                children: [
                  if (groupChat.userChats.length >= 3) ...[
                    Positioned(
                      left: 0,
                      top: 0,
                      child: CustomCircleAvatar(
                        backgroundImage:
                            NetworkImage(groupChat.userChats[0].avatarUrl!),
                        radius: 20,
                      ),
                    ),
                    Positioned(
                      right: 0,
                      top: 0,
                      child: CustomCircleAvatar(
                        backgroundImage:
                            NetworkImage(groupChat.userChats[1].avatarUrl!),
                        radius: 20,
                      ),
                    ),
                    Positioned(
                      left: 8,
                      bottom: 0,
                      child: CustomCircleAvatar(
                        backgroundImage:
                            NetworkImage(groupChat.userChats[2].avatarUrl!),
                        radius: 20,
                      ),
                    ),
                    if (groupChat.userChats.length >= 4)
                      Positioned(
                        right: 8,
                        bottom: 0,
                        child: CustomCircleAvatar(
                          backgroundImage:
                              NetworkImage(groupChat.userChats[3].avatarUrl!),
                          radius: 20,
                        ),
                      ),
                  ] else ...[
                    CustomCircleAvatar(
                      backgroundImage:
                          NetworkImage(groupChat.userChats[0].avatarUrl!),
                      radius: 28,
                    ),
                  ],
                ],
              ),
      ),
      title: Text(
        // if groupChat.name is not empty, use it, otherwise use the first name of the participants
        groupChat.name.isNotEmpty
            ? groupChat.name
            : groupChat.userChats
                .map((user) => user.fullName.split(' ').first)
                .join(', '),
        style: TextStyle(fontWeight: FontWeight.bold, fontSize: 18),
        overflow: TextOverflow.ellipsis,
      ),
      subtitle: Text('${groupChat.userChats.length} participants',
          style: TextStyle(fontSize: 14, color: Colors.grey[600])),
      trailing: Container(
        width: 10,
        height: 10,
        decoration: BoxDecoration(
          color: Colors.green,
          shape: BoxShape.circle,
        ),
      ),
      onTap: () {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Selected ${groupChat.name}')),
        );
      },
    );
  }
}
