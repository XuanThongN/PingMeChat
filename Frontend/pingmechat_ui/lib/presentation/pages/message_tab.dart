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
  // final List<ChatItem> _allChatItems = [
  //   ChatItem(
  //       name: 'Alex Linderson',
  //       message: 'How are you today?',
  //       time: '2 min ago',
  //       unreadCount: 3,
  //       imageUrl: 'assets/images/alex.jpg',
  //       isActive: true),
  //   ChatItem(
  //       name: 'Team Align',
  //       message: 'Dont miss to attend the meeting.',
  //       time: '2 min ago',
  //       unreadCount: 4,
  //       imageUrl: 'assets/images/team_align.jpg',
  //       isActive: true),
  //   ChatItem(
  //       name: 'John Ahraham',
  //       message: 'Hey! Can you join the meeting?',
  //       time: '2 min ago',
  //       imageUrl: 'assets/images/john_a.jpg'),
  //   ChatItem(
  //       name: 'Sabila Sayma',
  //       message: 'How are you today?',
  //       time: '2 min ago',
  //       imageUrl: 'assets/images/sabila.jpg'),
  //   ChatItem(
  //       name: 'John Borino',
  //       message: 'Have a good day 🌸',
  //       time: '2 min ago',
  //       imageUrl: 'assets/images/john_b.jpg'),
  //   ChatItem(
  //       name: 'John Borino',
  //       message: 'Have a good day 🌸',
  //       time: '2 min ago',
  //       imageUrl: 'assets/images/john_b.jpg'),
  //   ChatItem(
  //       name: 'John Borino',
  //       message: 'Have a good day 🌸',
  //       time: '2 min ago',
  //       imageUrl: 'assets/images/john_b.jpg'),
  //   ChatItem(
  //       name: 'Jane Doe',
  //       message: 'Hello!',
  //       time: '2 min ago',
  //       imageUrl: 'assets/images/jane.jpg'),
  //   ChatItem(
  //       name: 'John Doe',
  //       message: 'Hi there!',
  //       time: '2 min ago',
  //       imageUrl: 'assets/images/john.jpg'),
  //   ChatItem(
  //       name: 'Emma Watson',
  //       message: 'What are you up to?',
  //       time: '2 min ago',
  //       imageUrl: 'assets/images/emma.jpg'),
  //   ChatItem(
  //       name: 'Daniel Smith',
  //       message: 'Lets grab lunch!',
  //       time: '2 min ago',
  //       imageUrl: 'assets/images/daniel.jpg'),
  //   ChatItem(
  //       name: 'Sophia Johnson',
  //       message: 'Can you help me with this?',
  //       time: '2 min ago',
  //       imageUrl: 'assets/images/sophia.jpg'),
  //   ChatItem(
  //       name: 'Oliver Brown',
  //       message: 'I have a question for you.',
  //       time: '2 min ago',
  //       imageUrl: 'assets/images/oliver.jpg'),
  //   ChatItem(
  //       name: 'Ava Wilson',
  //       message: 'Are you free tomorrow?',
  //       time: '2 min ago',
  //       imageUrl: 'assets/images/ava.jpg'),
  //   ChatItem(
  //       name: 'William Davis',
  //       message: 'Lets go for a walk!',
  //       time: '2 min ago',
  //       imageUrl: 'assets/images/william.jpg'),
  //   ChatItem(
  //       name: 'Mia Anderson',
  //       message: 'I need your help!',
  //       time: '2 min ago',
  //       imageUrl: 'assets/images/mia.jpg'),
  //   ChatItem(
  //       name: 'James Martinez',
  //       message: 'How was your weekend?',
  //       time: '2 min ago',
  //       imageUrl: 'assets/images/james.jpg'),
  //   ChatItem(
  //       name: 'Charlotte Taylor',
  //       message: 'Lets catch up soon!',
  //       time: '2 min ago',
  //       imageUrl: 'assets/images/charlotte.jpg'),
  //   ChatItem(
  //       name: 'Benjamin Harris',
  //       message: 'Do you have any plans?',
  //       time: '2 min ago',
  //       imageUrl: 'assets/images/benjamin.jpg'),
  //   ChatItem(
  //       name: 'Harper Clark',
  //       message: 'I miss you!',
  //       time: '2 min ago',
  //       imageUrl: 'assets/images/harper.jpg'),
  //   ChatItem(
  //       name: 'Elijah Lewis',
  //       message: 'Can we talk?',
  //       time: '2 min ago',
  //       imageUrl: 'assets/images/elijah.jpg'),
  //   ChatItem(
  //       name: 'Amelia Turner',
  //       message: 'How are you feeling?',
  //       time: '2 min ago',
  //       imageUrl: 'assets/images/amelia.jpg'),
  //   ChatItem(
  //       name: 'Logan Walker',
  //       message: 'Lets hang out!',
  //       time: '2 min ago',
  //       imageUrl: 'assets/images/logan.jpg'),
  //   ChatItem(
  //       name: 'Sofia Hill',
  //       message: 'I have a surprise for you.',
  //       time: '2 min ago',
  //       imageUrl: 'assets/images/sofia.jpg'),
  //   ChatItem(
  //       name: 'Jackson Green',
  //       message: 'Can you call me?',
  //       time: '2 min ago',
  //       imageUrl: 'assets/images/jackson.jpg'),
  //   ChatItem(
  //       name: 'Lily Adams',
  //       message: 'I need your advice.',
  //       time: '2 min ago',
  //       imageUrl: 'assets/images/lily.jpg'),
  //   ChatItem(
  //       name: 'Sebastian Wright',
  //       message: 'Lets go on an adventure!',
  //       time: '2 min ago',
  //       imageUrl: 'assets/images/sebastian.jpg'),
  //   ChatItem(
  //       name: 'Zoe Parker',
  //       message: 'I have something to tell you.',
  //       time: '2 min ago',
  //       imageUrl: 'assets/images/zoe.jpg'),
  //   ChatItem(
  //       name: 'Michael Smith',
  //       message: 'Good morning!',
  //       time: '2 min ago',
  //       imageUrl: 'assets/images/michael.jpg'),
  //   ChatItem(
  //       name: 'Emily Johnson',
  //       message: 'How was your day?',
  //       time: '2 min ago',
  //       imageUrl: 'assets/images/emily.jpg'),
  //   ChatItem(
  //       name: 'David Brown',
  //       message: 'See you later!',
  //       time: '2 min ago',
  //       imageUrl: 'assets/images/david.jpg'),
  //   ChatItem(
  //       name: 'Sarah Wilson',
  //       message: 'Have a great weekend!',
  //       time: '2 min ago',
  //       imageUrl: 'assets/images/sarah.jpg'),
  // ];
  // List<ChatItem> _displayedChatItems = [];
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

  int _currentPage = 0;
  final int _itemsPerPage = 20; // Số lượng item mỗi lần load
  bool _isLoading = false; // Biến kiểm tra xem có đang load dữ liệu không
  bool _isAddOptionsVisible =
      false; // Biến kiểm tra xem có hiển thị menu thêm mới không
  final ScrollController _scrollController = ScrollController();

  @override
  void initState() {
    super.initState();
    _scrollController.addListener(_onScroll);
    // _loadingMoreItems();
    WidgetsBinding.instance!.addPostFrameCallback((_) {
      _chatProvider = Provider.of<ChatProvider>(context,
          listen: false); // Lấy provider từ context
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
        return RefreshIndicator(
          onRefresh: _refreshChatList,
          child: ListView.builder(
            controller: _scrollController,
            itemCount:
                chatProvider.chats.length + (chatProvider.hasMoreChats ? 1 : 0),
            itemBuilder: (context, index) {
              if (index < chatProvider.chats.length) {
                return _buildChatItem(chatProvider.chats[index]);
              } else {
                return _buildLoadingIndicator();
              }
            },
          ),
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
                // itemCount: chatItems.length,
                // itemBuilder: (context, index) =>
                //     _buildSlidableChatItem(chatItems[index]),
                controller: _scrollController,
                // itemCount: _displayedChatItems.length + 1,
                // itemBuilder: (context, index) {
                //   if (index < _displayedChatItems.length) {
                //     return _buildSlidableChatItem(_displayedChatItems[index]);
                //   } else if (_isLoading) {
                //     return _buildLoadingIndicator();
                //   } else {
                //     return const SizedBox.shrink();
                //   }
                // },
                itemCount: _chatProvider.chats.length,
                itemBuilder: (context, index) =>
                    _buildSlidableChatItem(_chatProvider.chats[index]),
                itemExtent: 60, // Assuming each item has a fixed height
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
            context, MaterialPageRoute(builder: (context) => ChatScreen()));
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
          item.isGroup ? item.name! : (item.userChats.first.user?.fullName ?? ''),
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
                ? DateFormat('hh:mm a, dd/MM').format(item.messages!.last.createdDate) 
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
