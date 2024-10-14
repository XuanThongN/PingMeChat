import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';
import 'package:flutter_gen/gen_l10n/app_localizations.dart';
import 'package:flutter_slidable/flutter_slidable.dart';
import 'package:intl/intl.dart';
import 'package:pingmechat_ui/domain/models/chat.dart';
import 'package:pingmechat_ui/domain/models/contact.dart';
import 'package:pingmechat_ui/providers/auth_provider.dart';
import 'package:pingmechat_ui/providers/chat_provider.dart';
import 'package:provider/provider.dart';
import 'package:uuid/uuid.dart';

import '../../config/theme.dart';
import '../../data/models/chat_model.dart';
import '../../domain/models/account.dart';
import '../../domain/models/attachment.dart';
import '../../domain/models/message.dart';
import '../../providers/contact_provider.dart';
import '../widgets/custom_circle_avatar.dart';
import '../widgets/custom_icon.dart';
import '../widgets/shimmer_loading.dart';
import 'chat_page.dart';
import 'create_group_page.dart';

class MessageTab extends StatefulWidget {
  const MessageTab({super.key});

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
        MaterialPageRoute(builder: (context) => ChatPage(chatId: chat.id)),
      );
    };

    WidgetsBinding.instance.addPostFrameCallback((_) {
      _chatProvider.loadChats(); // Load danh sách chat

      Provider.of<ContactProvider>(context, listen: false)
          .fetchContacts()
          .then((_) {
        Provider.of<ContactProvider>(context, listen: false)
            .fetchFriendStatus(); // Lấy trạng thái bạn bè
      }); // Load danh sách contact
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
        return Scaffold(
          backgroundColor: AppColors.background,
          body: SafeArea(
            child: Column(
              children: [
                _buildAppBar(),
                _buildStatusList(),
                Expanded(
                  child: Container(
                    // margin: const EdgeInsets.only(top: 10),
                    decoration: const BoxDecoration(
                      color: Colors.white,
                      borderRadius: BorderRadius.only(
                        topLeft: Radius.circular(30),
                        topRight: Radius.circular(30),
                      ),
                    ),
                    child: _buildRoundedChatList(),
                  ),
                ),
              ],
            ),
          ),
        );
      },
    );
  }

  Widget _buildLoadingIndicator() {
    return Consumer<ChatProvider>(
      builder: (context, chatProvider, child) {
        // return chatProvider.isLoading
        //     ? const Center(child: CircularProgressIndicator())
        //     : const SizedBox.shrink();
        return chatProvider.isLoading
            ? ShimmerLoading()
            : const SizedBox.shrink();
      },
    );
  }

  Widget _buildStatusList() {
    return Consumer<ContactProvider>(
      builder: (context, contactProvider, child) {
        final contactUsers = contactProvider.contactUsers;
        return SizedBox(
          height: 80,
          child: ListView.builder(
            scrollDirection: Axis.horizontal,
            itemCount: contactUsers.length,
            itemBuilder: (context, index) {
              final contact = contactUsers[index];
              return GestureDetector(
                onTap: () => _handleContactStatusTap(contact.id),
                child: _buildContactStatusItem(contact),
              );
            },
          ),
        );
      },
    );
  }

  // Viết hàm xử lý khi nhấn vào trạng thái liên hệ sẽ tiến hành tạo cuộc trò chuyện mới nếu chưa có cuộc trò chuyện riêng tư, còn nếu có rồi thì hiển thị nội dung cuộc trò chuyện
  void _handleContactStatusTap(String contactUserId) {
    // Duyệt tất cả các cuộc trò chuyện riêng tư và tìm cuộc trò chuyện có chứa user đang nhấn
    // Tìm chat có chứa user với contactUserId
    final chat = _chatProvider.chats.firstWhere(
      (chat) =>
          chat.isGroup == false &&
          chat.userChats.any((uc) => uc.userId == contactUserId),
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
        userIds: [contactUserId],
      ));
    }
  }

  Widget _buildContactStatusItem(Account contactUser) {
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
                    color: Colors.green ?? Colors.transparent,
                    width: 2,
                  ),
                ),
                child: CustomCircleAvatar(
                  backgroundImage: contactUser.avatarUrl != null
                      ? CachedNetworkImageProvider(contactUser.avatarUrl!)
                      : null,
                  radius: 26,
                ),
              ),
              Positioned(
                bottom: 0,
                right: 0,
                child: Container(
                  width: 12,
                  height: 12,
                  decoration: BoxDecoration(
                    color: contactUser.isOnline ? Colors.green : Colors.grey,
                    shape: BoxShape.circle,
                    border: Border.all(
                      color: AppColors.surface,
                      width: 2,
                    ),
                  ),
                ),
              ),
            ],
          ),
          const SizedBox(height: 4),
          Text(
            // Chỉ hiển thị tên người dùng với 10 ký tự đầu tiên còn lại hiển thị dấu ba chấm
            contactUser.fullName.length > 10
                ? '${contactUser.fullName.substring(0, 10)}...'
                : contactUser.fullName,
            style: AppTypography
                .caption, // Tránh tràn dòng và hiển thị dấu ba chấm
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
          ),
        ],
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
            icon: const Icon(Icons.search, color: Colors.white, size: 28),
            onPressed: () {
              Navigator.pushNamed(context, '/search');
            },
          ),
          Text(
            AppLocalizations.of(context)!.home,
            style: const TextStyle(
                color: Colors.white, fontSize: 24, fontWeight: FontWeight.bold),
          ),
          IconButton(
            icon: Icon(_isAddOptionsVisible ? Icons.close : Icons.add,
                color: Colors.white, size: 28),
            onPressed: () {
              _showAddOptions(context);
            },
          ),
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
    // lấy tên của người dùng hoặc tên nhóm
    final chatName = item.isGroup
        // Nếu là nhóm thì hiển thị tên nhóm hoặc nếu tên nhóm không có thì hiển thị tên đầu tiên của các thành viên nối lại bằng dấu phẩy
        ? item.name != ''
            ? item.name
            : item.userChats
                .map((uc) => uc.user!.fullName.split(' ').first)
                .join(', ')
        : item.userChats
            .firstWhere((uc) => uc.userId != _authProvider.currentUser!.id)
            .user
            ?.fullName;
    final avatarUrl = item.isGroup
        ? item.avatarUrl
        : item.userChats
            .firstWhere((uc) => uc.userId != _authProvider.currentUser!.id)
            .user
            ?.avatarUrl;
    final isOnline = item.isGroup
        ? false
        : item.userChats
            .firstWhere((uc) => uc.userId != _authProvider.currentUser!.id)
            .isOnline;
    // Kiểm tra xem tin nhắn cuối cùng đã được đánh dấu là đã đọc chưa
    final messages = item.messages;
    final readers = [];
    var isRead = false;
    if (messages != null) {
      isRead = true;
    } else {
      isRead = readers != null
          ? readers
              .any((reader) => reader.readerId == _authProvider.currentUser!.id)
          : true;
    }
    return GestureDetector(
      onTap: () {
        // Handle chat item tap
        Navigator.push(context,
            MaterialPageRoute(builder: (context) => ChatPage(chatId: item.id)));
        // Mark message as read
        if (!isRead) {
          final message = item.messages!.last;
          _chatProvider.markMessageAsRead(
              item.id,
              message.id ??
                  Uuid()
                      .v4()); // Gửi id của tin nhắn cuối cùng để đánh dấu đã đọc
          // Giả sử tin nhắn cuối cùng chưa có id thì sử dụng id ngẫu nhiên (giả lập)
        }
      },
      child: ListTile(
        leading: Stack(children: [
          CustomCircleAvatar(
            radius: 24,
            backgroundImage: avatarUrl != null
                ? CachedNetworkImageProvider(avatarUrl)
                : null,
            isGroupChat: item.isGroup,
          ),
          // if (item.isActive) // Người dùng hoạt động
          if (!item.isGroup)
            Positioned(
              bottom: 0,
              right: 0,
              child: Container(
                width: 12,
                height: 12,
                decoration: BoxDecoration(
                  color: isOnline! ? Colors.green : Colors.grey,
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
          chatName ?? '',
          style: AppTypography.chatName,
          maxLines: 1,
          overflow: TextOverflow.ellipsis,
        ),
        subtitle: Row(
          children: [
            Expanded(
              child: Row(
                children: [
                  Expanded(
                    child: Consumer<AuthProvider>(
                      builder: (context, authProvider, child) {
                        final currentUserId = authProvider.currentUser?.id;
                        final lastMessage = item.messages!.isNotEmpty
                            ? item.messages!.last
                            : null;
                        if (lastMessage == null) {
                          return const Text('');
                        }
                        final isCurrentUser =
                            lastMessage.senderId == currentUserId;
                        final senderName = isCurrentUser
                            ? 'You'
                            : (lastMessage.sender?.fullName.split(' ').first ??
                                'Unknown');
                        return Text(
                          '$senderName: ${_createMessageContent(lastMessage)}',
                          style: AppTypography.chatMessage.copyWith(
                            fontWeight:
                                isRead ? FontWeight.w300 : FontWeight.bold,
                          ),
                          maxLines: 1,
                          overflow: TextOverflow.ellipsis,
                        );
                      },
                    ),
                  ),
                  const SizedBox(width: 10),
                  const Text(
                    '•',
                    style: TextStyle(
                      fontSize: 6,
                      color: Colors.grey,
                    ),
                  ),
                  const SizedBox(width: 5),
                ],
              ),
            ),
            Text(
              item.messages!.isNotEmpty
                  ? _showTimeOfChat(item.messages!.last.createdDate)
                  : '', // Chỉ hiển thị giờ và ngày gửi tin nhắn cuối cùng
              style: AppTypography.caption,
            ),
          ],
        ),
        trailing: Column(
          mainAxisAlignment: MainAxisAlignment.spaceAround,
          crossAxisAlignment: CrossAxisAlignment.end,
          children: [
            if (!isRead) // Hiển thị chấm tròn màu xanh nước biển nếu có tin nhắn chưa đọc
              Container(
                width: 8,
                height: 8,
                decoration: const BoxDecoration(
                  color: AppColors.primary,
                  shape: BoxShape.circle,
                ),
              ),
            // Text(
            //   item.messages!.length >= 1
            //       ? _showTimeOfChat(item.messages!.last.createdDate)
            //       : '', // Chỉ hiển thị giờ và ngày gửi tin nhắn cuối cùng

            //   style: AppTypography.caption,
            // ),
            // if (item.unreadCount != null && item.unreadCount! > 0)
          ],
        ),
      ),
    );
  }

  String _createMessageContent(Message message) {
    if (message.content != null && message.content!.isNotEmpty) {
      return message.content!;
    }
    final attachmentCounts = _countAttachments(message.attachments!);
    final attachmentDescriptions = attachmentCounts.entries.map((entry) {
      final type = entry.key;
      final count = entry.value;
      return '$count ${_getAttachmentTypeDescription(type)}';
    }).join(' and ');

    return 'Sent $attachmentDescriptions';
  }

  Map<String, int> _countAttachments(List<Attachment> attachments) {
    final counts = <String, int>{};

    for (var attachment in attachments) {
      counts[attachment.fileType] = (counts[attachment.fileType] ?? 0) + 1;
    }

    return counts;
  }

  String _getAttachmentTypeDescription(String type) {
    switch (type) {
      case 'Image':
        return 'image${type == 'Image' && 1 > 1 ? 's' : ''}';
      case 'Video':
        return 'video${type == 'Video' && 1 > 1 ? 's' : ''}';
      case 'Audio':
        return 'audio${type == 'Audio' && 1 > 1 ? 's' : ''}';
      case 'File':
        return 'file${type == 'File' && 1 > 1 ? 's' : ''}';
      default:
        return 'attachment${type == 'Attachment' && 1 > 1 ? 's' : ''}';
    }
  }

  String _showTimeOfChat(DateTime lastMessageTime) {
    String formattedTime = '';
    final now = DateTime.now();
    final difference = now.difference(lastMessageTime);

    if (difference.inMinutes == 0 && difference.inMinutes < 1) {
      formattedTime = 'Just now';
    } else if (difference.inMinutes < 60) {
      formattedTime = '${difference.inMinutes}m ago';
    } else if (difference.inHours < 24) {
      formattedTime = DateFormat('HH:mm').format(lastMessageTime);
    } else if (difference.inDays == 1) {
      formattedTime = 'Yesterday';
    } else if (difference.inDays < 7) {
      formattedTime = DateFormat('EEEE', 'en')
          .format(lastMessageTime); // Hiển thị thứ bằng tiếng Việt
    } else {
      formattedTime = DateFormat('dd MMMM', 'en')
          .format(lastMessageTime); // Hiển thị ngày tháng bằng tiếng Việt
    }
    return formattedTime;
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
        position: const RelativeRect.fromLTRB(100, 100, 0, 0),
        items: const [
          PopupMenuItem(
            value: 'new_chat',
            //Child include icon and text
            child: Row(
              children: [
                Icon(Icons.account_circle),
                SizedBox(width: 8),
                Text('Add New Chat'),
              ],
            ),
          ),
          PopupMenuItem(
            value: 'new_group',
            child: Row(
              children: [
                Icon(Icons.supervised_user_circle_outlined),
                SizedBox(width: 8),
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
            const SnackBar(
              content: Text('Add New Chat'),
              duration: Duration(seconds: 2),
            ),
          );
        } else if (value == 'new_group') {
          // Chuyển tới trang tạo nhóm
          Navigator.push(
            context,
            MaterialPageRoute(
              builder: (context) => const CreateGroupPage(),
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
