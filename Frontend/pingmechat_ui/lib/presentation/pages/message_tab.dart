import 'package:flutter/material.dart';
import 'package:flutter_gen/gen_l10n/app_localizations.dart';
import 'package:flutter_slidable/flutter_slidable.dart';
import 'package:intl/intl.dart';
import 'package:pingmechat_ui/domain/models/chat.dart';
import 'package:pingmechat_ui/domain/models/contact.dart';
import 'package:pingmechat_ui/providers/auth_provider.dart';
import 'package:pingmechat_ui/providers/chat_provider.dart';
import 'package:provider/provider.dart';

import '../../config/theme.dart';
import '../../data/models/chat_model.dart';
import '../../providers/contact_provider.dart';
import '../widgets/custom_circle_avatar.dart';
import '../widgets/custom_icon.dart';
import 'chat_page.dart';
import 'create_group_page.dart';

class MessageTab extends StatefulWidget {
  @override
  State<MessageTab> createState() => _MessageTabState();
}

class _MessageTabState extends State<MessageTab> {
  late ChatProvider _chatProvider; // Khai báo provider
  late AuthProvider _authProvider; // Khai báo provider
  bool _isAddOptionsVisible =
      false; // Biến kiểm tra xem có hiển thị menu thêm mới không
  final ScrollController _scrollController = ScrollController();

  @override
  void initState() {
    super.initState();
    _scrollController.addListener(_onScroll);
    // _loadingMoreItems();
    _chatProvider = Provider.of<ChatProvider>(context,
        listen: false); // Lấy provider từ context
    _authProvider = Provider.of<AuthProvider>(context, listen: false);

    // Thiết lập callback để mở ChatPage
    _chatProvider.onOpenChatPage = (chat) {
      Navigator.push(
        context,
        MaterialPageRoute(builder: (context) => ChatScreen(chatId: chat.id)),
      );
    };

    WidgetsBinding.instance!.addPostFrameCallback((_) {
      _chatProvider.loadChats(); // Load danh sách chat

      Provider.of<ContactProvider>(context, listen: false)
          .fetchContacts(); // Load danh sách contact
    });
  }

  @override
  void dispose() {
    _scrollController.removeListener(_onScroll);
    _scrollController.dispose();
    super.dispose();
  }

  void _onScroll() {
    if (_scrollController.position.pixels ==
        _scrollController.position.maxScrollExtent) {
      _chatProvider.loadChats();
    }
  }

  @override
  Widget build(BuildContext context) {
    return Consumer<ChatProvider>(
      builder: (context, chatProvider, child) {
        return Column(
          children: [
            _buildAppBar(),
            _buildStatusList(),
            _buildRoundedChatList(),
          ],
        );
      },
    );
  }

  Widget _buildLoadingIndicator() {
    return Consumer<ChatProvider>(
      builder: (context, chatProvider, child) {
        return chatProvider.isLoading
            ? Center(child: CircularProgressIndicator())
            : SizedBox.shrink();
      },
    );
  }

  Widget _buildStatusList() {
    return Consumer<ContactProvider>(
      builder: (context, contactProvider, child) {
        final contacts = contactProvider.contacts;
        return SizedBox(
          height: 100,
          child: ListView.builder(
            scrollDirection: Axis.horizontal,
            itemCount: contacts.length,
            itemBuilder: (context, index) {
              final contact = contacts[index];
              return GestureDetector(
                onTap: () => _handleContactStatusTap(contact),
                child: _buildContactStatusItem(
                    contact, _authProvider.currentUser!.id),
              );
            },
          ),
        );
      },
    );
  }

  // Viết hàm xử lý khi nhấn vào trạng thái liên hệ sẽ tiến hành tạo cuộc trò chuyện mới nếu chưa có cuộc trò chuyện riêng tư, còn nếu có rồi thì hiển thị nội dung cuộc trò chuyện
  void _handleContactStatusTap(Contact contact) {
    // Lấy id của người dùng được chọn từ contact nếu contact.user.id == currentUser.id thì lấy contact.contactUser.id,
    // Nếu contact.contactUser.id == currentUser.id thì lấy contact.user.id
    String contactId;
    if (contact.user!.id == _authProvider.currentUser!.id) {
      contactId = contact.contactUser!.id;
    } else {
      contactId = contact.user!.id;
    }
    // Đầu tiên kiểm tra xem có cuộc trò chuyện riêng tư này chưa, nếu có thì trỏ tới cuộc trò chuyện đó, nếu không thì tạo mới
    // Nên check chat phải là isGroup = false
    final chat = _chatProvider.chats.firstWhere((element) =>
        element.userChats.any((userChat) => userChat.userId == contactId) &&
        !element.isGroup);
    if (chat != null) {
      Navigator.push(
        context,
        MaterialPageRoute(builder: (context) => ChatScreen(chatId: chat.id)),
      );
    } else {
      _chatProvider.startNewChat(ChatCreateDto(
        isGroup: false,
        userIds: [contactId],
      ));
    }
  }

  Widget _buildContactStatusItem(Contact contact, String currentUserId) {
    final isMe = contact.id == currentUserId;
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 8),
      child: Column(
        children: [
          Stack(
            children: [
              Container(
                decoration: BoxDecoration(
                  shape: BoxShape.circle,
                  border: Border.all(
                    color: contact.isOnline ? Colors.green : Colors.transparent,
                    width: 2,
                  ),
                ),
                child: CustomCircleAvatar(
                  backgroundImage: AssetImage(contact.avatarUrl!),
                  radius: 30,
                ),
              ),
              if (isMe)
                Positioned(
                  right: 0,
                  bottom: 0,
                  child: Container(
                    padding: const EdgeInsets.all(4),
                    decoration: const BoxDecoration(
                      color: Colors.blue,
                      shape: BoxShape.circle,
                    ),
                    child: const Icon(Icons.add, color: Colors.white, size: 14),
                  ),
                ),
            ],
          ),
          const SizedBox(height: 4),
          Text(
            contact.fullName!.isNotEmpty
                ? contact.fullName!
                : (isMe
                    ? contact.contactUser!.fullName
                    : contact.user!.fullName),
            style: AppTypography.caption,
            overflow: TextOverflow.ellipsis,
          ),
        ],
      ),
    );
  }

  Widget _buildAppBar() {
    return Padding(
      padding: const EdgeInsets.all(16.0),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          _buildCircularButton(Icons.search, () {
            // Handle search button press
          }),
          Text(
            AppLocalizations.of(context)!.home,
            style: AppTypography.h2,
          ),
          _buildCircularButton(_isAddOptionsVisible ? Icons.close : Icons.add,
              () {
            _showAddOptions(context);
          }),
        ],
      ),
    );
  }

  Widget _buildRoundedChatList() {
    return Expanded(
      child: Container(
        padding: const EdgeInsets.only(top: 20),
        decoration: const BoxDecoration(
          color: AppColors.surface,
          borderRadius: BorderRadius.only(
            topLeft: Radius.circular(30),
            topRight: Radius.circular(30),
          ),
        ),
        child: RefreshIndicator(
          color: AppColors.primary,
          backgroundColor: AppColors.surface,
          onRefresh: _refreshChatList,
          child: ClipRRect(
            borderRadius: const BorderRadius.only(
              topLeft: Radius.circular(30),
              topRight: Radius.circular(30),
            ),
            child: SlidableAutoCloseBehavior(
              child: ListView.builder(
                controller: _scrollController,
                itemCount: _chatProvider.chats.length,
                itemBuilder: (context, index) =>
                    _buildSlidableChatItem(_chatProvider.chats[index]),
                itemExtent: 64, // Assuming each item has a fixed height
              ),
            ),
          ),
        ),
      ),
    );
  }

  Future<void> _refreshChatList() async {
    await _chatProvider.loadChats(refresh: true);
  }

  // Widget _buildSlidableChatItem(ChatItem item) {
  Widget _buildSlidableChatItem(Chat item) {
    return Slidable(
      key: ValueKey(item),
      endActionPane: ActionPane(
        motion: const ScrollMotion(),
        extentRatio: 0.45, // Giảm kích thước của khu vực action
        children: [
          CustomSlidableAction(
            onPressed: (context) => _handleNotification(item),
            backgroundColor: AppColors.surface,
            child: const CustomActionIcon(
              color: AppColors.secondary,
              svgPath:
                  'assets/icons/notification.svg', // Đường dẫn đến file SVG của bạn
            ),
          ),
          CustomSlidableAction(
            onPressed: (context) => _handleDelete(item),
            backgroundColor: AppColors.surface,
            child: const CustomActionIcon(
              color: AppColors.red,
              svgPath:
                  'assets/icons/trash.svg', // Đường dẫn đến file SVG của bạn
            ),
          ),
        ],
      ),
      child: Padding(
          padding: const EdgeInsets.only(bottom: 14),
          child: _buildChatItem(item)),
    );
  }

  Widget _buildChatItem(Chat item) {
    return GestureDetector(
      onTap: () {
        // Handle chat item tap
        Navigator.push(
            context,
            MaterialPageRoute(
                builder: (context) => ChatScreen(chatId: item.id)));
      },
      child: ListTile(
        leading: Stack(children: [
          CircleAvatar(
            radius: 24,
            backgroundImage:
                item.avatarUrl != null ? AssetImage(item.avatarUrl!) : null,
          ),
          // if (item.isActive) // Người dùng hoạt động
          if (item
              .isGroup) // Tạm thời sử dụng trường isGroup để hiển thị người dùng hoạt động
            Positioned(
              bottom: 0,
              right: 0,
              child: Container(
                width: 12,
                height: 12,
                decoration: BoxDecoration(
                  color: Colors.green,
                  shape: BoxShape.circle,
                  border: Border.all(
                    color: AppColors.surface,
                    width: 2,
                  ),
                ),
              ),
            ),
        ]),
        title: Text(
          item.isGroup
              ? item.name!
              : (item.userChats.first.user?.fullName ?? ''),
          style: AppTypography.chatName,
        ),
        subtitle: Text(
          item.messages!.length >= 1 ? item.messages!.last.content! : '',
          style: AppTypography.chatMessage,
          maxLines: 1,
          overflow: TextOverflow.ellipsis,
        ),
        trailing: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          crossAxisAlignment: CrossAxisAlignment.end,
          children: [
            Text(
              item.messages!.length >= 1
                  ? DateFormat('hh:mm a')
                      .format(item.messages!.last.createdDate)
                  : '', // Chỉ hiển thị giờ và ngày gửi tin nhắn cuối cùng

              style: AppTypography.caption,
            ),
            // if (item.unreadCount != null && item.unreadCount! > 0)
            //   Container(
            //     padding: EdgeInsets.all(6),
            //     decoration: const BoxDecoration(
            //       color: AppColors.red,
            //       shape: BoxShape.circle,
            //     ),
            //     child: Text(
            //       item.unreadCount.toString(),
            //       style: AppTypography.badge,
            //     ),
            //   ), // Hiển thị số lượng tin nhắn chưa đọc tạm thời chưa có dữ liệu
          ],
        ),
      ),
    );
  }

  Widget _buildCircularButton(IconData icon, VoidCallback onPressed) {
    return Container(
      decoration: BoxDecoration(
        shape: BoxShape.circle,
        border: Border.all(color: Colors.grey[800]!, width: 1),
      ),
      child: IconButton(
        icon: Icon(icon, color: Colors.white, size: 20),
        onPressed: onPressed,
        tooltip: AppLocalizations.of(context)!.search,
      ),
    );
  }

  void _showAddOptions(BuildContext context) {
    setState(() {
      _isAddOptionsVisible = !_isAddOptionsVisible;
    });
    if (_isAddOptionsVisible) {
      showMenu(
        context: context,
        position: RelativeRect.fromLTRB(100, 100, 0, 0),
        items: const [
          PopupMenuItem(
            value: 'new_chat',
            //Child include icon and text
            child: Row(
              children: [
                Icon(Icons.account_circle),
                const SizedBox(width: 8),
                Text('Add New Chat'),
              ],
            ),
          ),
          PopupMenuItem(
            value: 'new_group',
            child: Row(
              children: [
                Icon(Icons.supervised_user_circle_outlined),
                const SizedBox(width: 8),
                Text('Create Group'),
              ],
            ),
          ),
        ],
      ).then((value) {
        setState(() {
          _isAddOptionsVisible = false;
        });
        if (value == 'new_chat') {
          // Handle add new chat
          //Show snack bar
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(
              content: Text('Add New Chat'),
              duration: const Duration(seconds: 2),
            ),
          );
        } else if (value == 'new_group') {
          // Chuyển tới trang tạo nhóm
          Navigator.push(
            context,
            MaterialPageRoute(
              builder: (context) => CreateGroupPage(),
            ),
          );
        }
      });
    }
  }

  void _handleNotification(Chat item) {
    // Xử lý khi nhấn vào nút thông báo, thông báo bằng snackbar
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text('Notification sent to ${item.name}'),
        duration: const Duration(seconds: 2),
      ),
    );
    // Thêm logic xử lý thông báo ở đây
  }

  void _handleDelete(Chat item) {
    // Xử lý khi nhấn vào nút xóa
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text('Delete ${item.name}'),
        duration: const Duration(seconds: 2),
      ),
    );
  }
}
