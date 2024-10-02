import 'dart:async';

import 'package:flutter/material.dart';
import 'package:pingmechat_ui/presentation/widgets/custom_circle_avatar.dart';
import 'package:pingmechat_ui/providers/search_provider.dart';
import 'package:provider/provider.dart';

import '../../config/theme.dart';
import '../../core/constants/constant.dart';
import '../../data/models/chat_model.dart';
import '../../data/models/search_result.dart';
import '../../domain/models/chat.dart';
import '../../providers/chat_provider.dart';
import '../../providers/contact_provider.dart';
import 'chat_page.dart';

class SearchResultsScreen extends StatefulWidget {
  static const routeName = '/search';

  const SearchResultsScreen({super.key});
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
                    return const Center(child: CircularProgressIndicator());
                  }
                  if (searchProvider.searchResult == null) {
                    return const Center(child: Text('No results found'));
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

  const SearchBar({
    super.key,
    required this.controller,
    required this.onBackPressed,
    required this.onChanged,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      decoration: BoxDecoration(
        color: AppColors.surface,
        borderRadius: BorderRadius.circular(20),
        boxShadow: [
          BoxShadow(
            color: AppColors.primary.withOpacity(0.1),
            blurRadius: 10,
            offset: const Offset(0, 3),
          ),
        ],
      ),
      child: Row(
        children: [
          IconButton(
            icon: const Icon(Icons.arrow_back, color: AppColors.primary),
            onPressed: onBackPressed,
          ),
          Expanded(
            child: TextField(
              controller: controller,
              onChanged: onChanged,
              decoration: const InputDecoration(
                hintText: 'Search people or groups...',
                border: InputBorder.none,
                hintStyle: TextStyle(color: AppColors.tertiary, fontSize: 16),
              ),
              style: const TextStyle(color: AppColors.primary, fontSize: 16),
            ),
          ),
          AnimatedOpacity(
            opacity: controller.text.isNotEmpty ? 1.0 : 0.0,
            duration: const Duration(milliseconds: 200),
            child: IconButton(
              icon: const Icon(Icons.close, color: AppColors.primary),
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

  const SectionTitle({super.key, required this.title});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      child: Text(
        title,
        style: const TextStyle(fontSize: 22, fontWeight: FontWeight.bold),
      ),
    );
  }
}

class UserTile extends StatefulWidget {
  final User user;

  const UserTile({Key? key, required this.user}) : super(key: key);

  @override
  _UserTileState createState() => _UserTileState();
}

class _UserTileState extends State<UserTile> {
  late String contactStatus;

  @override
  void initState() {
    super.initState();
    contactStatus = widget.user.contactStatus!;
  }

  @override
  Widget build(BuildContext context) {
    return ListTile(
      contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      leading: CustomCircleAvatar(
        radius: 28,
        backgroundImage: widget.user.avatarUrl!.isNotEmpty
            ? NetworkImage(widget.user.avatarUrl!)
            : null,
      ),
      title: Text(widget.user.fullName,
          style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 18)),
      subtitle:
          Text(widget.user.userName, style: const TextStyle(fontSize: 14)),
      trailing: _buildActionButton(context, contactStatus, widget.user),
      onTap: () {
        if (contactStatus == ContactStatus.ACCEPTED) {
          _handleUserSearchTap(widget.user.id);
        } else {
          ScaffoldMessenger.of(context).showSnackBar(
            // Chỉ hiển thị thông báo khi người dùng chưa là bạn bè
            const SnackBar(content: Text('You must be friends to chat')),
          );
        }
      },
    );
  }

  Widget? _buildActionButton(BuildContext context, String status, User user) {
    final contactProvider =
        Provider.of<ContactProvider>(context, listen: false);
    switch (status) {
      case ContactStatus.ACCEPTED:
        return null; // Không hiển thị nút nào nếu đã là bạn bè
      case ContactStatus.PENDING:
        return IconButton(
          icon: const Icon(Icons.check_circle_outline),
          onPressed: () => _acceptFriendRequest(context, contactProvider, user),
        );
      case ContactStatus.REQUESTED:
        return IconButton(
          icon: const Icon(Icons.remove_circle_outline_outlined),
          onPressed: () => _cancelFriendRequest(context, contactProvider, user),
        );
      default:
        return IconButton(
          icon: const Icon(Icons.person_add_alt_1),
          onPressed: () => _sendFriendRequest(context, contactProvider, user),
        );
    }
  }

  Future<void> _acceptFriendRequest(
      BuildContext context, ContactProvider contactProvider, User user) async {
    try {
      final newStatus = await contactProvider.acceptFriendRequest(user.id);
      setState(() {
        contactStatus = newStatus;
      });
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
            content: Text('Accepted friend request from ${user.fullName}')),
      );
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
            content:
                Text('Failed to accept friend request from ${user.fullName}')),
      );
    }
  }

  Future<void> _cancelFriendRequest(
      BuildContext context, ContactProvider contactProvider, User user) async {
    try {
      final newStatus = await contactProvider.cancelFriendRequest(user.id);
      setState(() {
        contactStatus = newStatus;
      });
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Canceled friend request to ${user.fullName}')),
      );
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
            content:
                Text('Failed to cancel friend request to ${user.fullName}')),
      );
    }
  }

  Future<void> _sendFriendRequest(
      BuildContext context, ContactProvider contactProvider, User user) async {
    try {
      final newStatus = await contactProvider.sendFriendRequest(user.id);
      setState(() {
        contactStatus = newStatus;
      });
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Sent friend request to ${user.fullName}')),
      );
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
            content: Text('Failed to send friend request to ${user.fullName}')),
      );
    }
  }

  void _handleUserSearchTap(String userId) {
    final _chatProvider = Provider.of<ChatProvider>(context, listen: false);
    // Duyệt tất cả các cuộc trò chuyện riêng tư và tìm cuộc trò chuyện có chứa user đang nhấn
    // Tìm chat có chứa user với userId
    final chat = _chatProvider.chats.firstWhere(
      (chat) =>
          chat.isGroup == false &&
          chat.userChats.any((uc) => uc.userId == userId),
      orElse: () => Chat(
          isGroup: false,
          userChats: [],
          id: ''), // Cung cấp giá trị mặc định khi không tìm thấy phần tử nào
    );

// Kiểm tra nếu chat không phải là null và lấy chatId
    if (chat.id.isNotEmpty) {
      Navigator.push(
        context,
        MaterialPageRoute(builder: (context) => ChatPage(chatId: chat.id)),
      );
    } else {
      _chatProvider.startNewChat(ChatCreateDto(
        isGroup: false,
        userIds: [userId],
      ));
    }
  }
}

class GroupTile extends StatelessWidget {
  final GroupChat groupChat;

  const GroupTile({super.key, required this.groupChat});

  @override
  Widget build(BuildContext context) {
    return ListTile(
      contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
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
        style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 18),
        overflow: TextOverflow.ellipsis,
      ),
      subtitle: Text('${groupChat.userChats.length} participants',
          style: TextStyle(fontSize: 14, color: Colors.grey[600])),
      trailing: Container(
        width: 10,
        height: 10,
        decoration: const BoxDecoration(
          color: Colors.green,
          shape: BoxShape.circle,
        ),
      ),
      onTap: () {
        _handleGroupSearchTap(context, groupChat.id);
      },
    );
  }

  void _handleGroupSearchTap(BuildContext context, String groupChatId) {
    final _chatProvider = Provider.of<ChatProvider>(context, listen: false);
    // Duyệt tất cả các cuộc trò chuyện riêng tư và tìm cuộc trò chuyện có chứa user đang nhấn
    // Tìm chat có chứa user với groupChatId
    final chat = _chatProvider.chats.firstWhere(
      (chat) => chat.isGroup && chat.id == groupChatId,
      orElse: () => Chat(
          isGroup: false,
          userChats: [],
          id: ''), // Cung cấp giá trị mặc định khi không tìm thấy phần tử nào
    );

// Kiểm tra nếu chat không phải là null và lấy groupChatId
    if (chat.id.isEmpty) return;
    // Mở trang chat với chatId
    Navigator.push(
      context,
      MaterialPageRoute(builder: (context) => ChatPage(chatId: chat.id)),
    );
  }
}
