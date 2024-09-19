import 'package:flutter/material.dart';
import 'package:flutter_gen/gen_l10n/app_localizations.dart';
import 'package:flutter_slidable/flutter_slidable.dart';
import 'package:intl/intl.dart';
import 'package:pingmechat_ui/domain/models/chat.dart';
import 'package:pingmechat_ui/providers/chat_provider.dart';
import 'package:provider/provider.dart';

import '../../config/theme.dart';
import '../widgets/custom_icon.dart';
import 'chat_page.dart';
import 'create_group_page.dart';

class MessageTab extends StatefulWidget {
  @override
  State<MessageTab> createState() => _MessageTabState();
}

class _MessageTabState extends State<MessageTab> {
  late ChatProvider _chatProvider; // Khai báo provider
  final List<StatusItem> statusItems = [
    StatusItem(
        name: 'My status',
        imageUrl: 'assets/images/my_status.jpg',
        isMyStatus: true),
    StatusItem(
        name: 'Adil', imageUrl: 'assets/images/adil.jpg', isOnline: true),
    StatusItem(name: 'Marina', imageUrl: 'assets/images/marina.jpg'),
    StatusItem(name: 'Dean', imageUrl: 'assets/images/dean.jpg'),
    StatusItem(name: 'Max', imageUrl: 'assets/images/max.jpg'),
    StatusItem(name: 'Max', imageUrl: 'assets/images/max.jpg'),
    StatusItem(
        name: 'Adil', imageUrl: 'assets/images/adil.jpg', isOnline: true),
    StatusItem(
        name: 'Adil', imageUrl: 'assets/images/adil.jpg', isOnline: true),
  ];

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
    WidgetsBinding.instance!.addPostFrameCallback((_) {
      _chatProvider.loadChats(); // Load danh sách chat
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
      // _loadingMoreItems();
      _chatProvider.loadChats();
    }
  }

  // @override
  // Widget build(BuildContext context) {
  //   return Consumer(builder: (context, chatProvider, child){
  //     return Column(
  //     children: [
  //       _buildAppBar(),
  //       _buildStatusList(),
  //       _buildRoundedChatList(),
  //     ],
  //   )
  //   });
  // }

  @override
  Widget build(BuildContext context) {
    return Consumer<ChatProvider>(
      builder: (context, chatProvider, child) {
        return Column(
          children: [
            _buildAppBar(),
            _buildStatusList(),
            // Expanded(
            //   child: RefreshIndicator(
            //     onRefresh: _refreshChatList,
            //     child: ListView.builder(
            //       controller: _scrollController,
            //       itemCount: chatProvider.chats.length + (chatProvider.hasMoreChats ? 1 : 0),
            //       itemBuilder: (context, index) {
            //         if (index < chatProvider.chats.length) {
            //           return _buildChatItem(chatProvider.chats[index]);
            //         } else {
            //           return _buildLoadingIndicator();
            //         }
            //       },
            //     ),
            //   ),
            // ),
            _buildRoundedChatList(),
          ],
        );
      },
    );
  }

  // @override
  // Widget build(BuildContext context) {
  //   return Consumer<ChatProvider>(
  //     builder: (context, chatProvider, child) {
  //       return RefreshIndicator(
  //         onRefresh: _refreshChatList,
  //         child: ListView.builder(
  //           controller: _scrollController,
  //           itemCount:
  //               chatProvider.chats.length + (chatProvider.hasMoreChats ? 1 : 0),
  //           itemBuilder: (context, index) {
  //             if (index < chatProvider.chats.length) {
  //               return _buildChatItem(chatProvider.chats[index]);
  //             } else {
  //               return _buildLoadingIndicator();
  //             }
  //           },
  //         ),
  //       );
  //     },
  //   );
  // }

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
    return SizedBox(
      height: 100,
      child: ListView.builder(
        scrollDirection: Axis.horizontal,
        itemCount: statusItems.length,
        itemBuilder: (context, index) => _buildStatusItem(statusItems[index]),
      ),
    );
  }

  Widget _buildStatusItem(StatusItem item) {
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
                    color: item.isOnline ? Colors.green : Colors.transparent,
                    width: 2,
                  ),
                ),
                child: CircleAvatar(
                  // backgroundImage: CachedNetworkImageProvider(item.imageUrl),
                  // Use AssetImage for local images
                  backgroundImage: AssetImage(item.imageUrl),
                  radius: 30,
                ),
              ),
              if (item.isMyStatus)
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
          Text(item.name, style: AppTypography.caption),
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
          // const CircleAvatar(
          //   backgroundImage: AssetImage('assets/images/profile.jpg'),
          //   radius: 18,
          // ),
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

  // Widget _buildLoadingIndicator() {
  //   return const Center(
  //     child: Padding(
  //       padding: EdgeInsets.all(8),
  //       child: CircularProgressIndicator(
  //         // Thay thế bằng widget loading của bạn
  //         valueColor: AlwaysStoppedAnimation<Color>(AppColors.primary),
  //       ),
  //     ),
  //   );
  // }

  Future<void> _refreshChatList() async {
    // await Future.delayed(const Duration(seconds: 2));
    // _currentPage = 0;
    // _displayedChatItems.clear();
    // _loadingMoreItems();
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
            context, MaterialPageRoute(builder: (context) => ChatScreen(chatId: item.id)));
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

  // Load more items when the user scrolls to the bottom of the list
  // void _loadingMoreItems() {
  //   if (_isLoading) return;
  //   setState(() {
  //     _isLoading = true;
  //   });
  //   // Simulate a network request
  //   Future.delayed(const Duration(seconds: 2), () {
  //     final start = _currentPage * _itemsPerPage;
  //     final end = start + _itemsPerPage;
  //     final newItems = _allChatItems.sublist(
  //         start, end > _allChatItems.length ? _allChatItems.length : end);

  //     setState(() {
  //       _displayedChatItems.addAll(newItems);
  //       _currentPage++;
  //       _isLoading = false;
  //     });
  //   });
  // }

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

class StatusItem {
  final String name;
  final String imageUrl;
  final bool isOnline;
  final bool isMyStatus;

  const StatusItem({
    required this.name,
    required this.imageUrl,
    this.isOnline = false,
    this.isMyStatus = false,
  });
}
