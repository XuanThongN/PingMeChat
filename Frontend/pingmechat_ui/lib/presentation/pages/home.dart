import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_svg/svg.dart';
import 'package:pingmechat_ui/config/theme.dart';
import 'package:flutter_slidable/flutter_slidable.dart';
import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter_gen/gen_l10n/app_localizations.dart';
import 'package:pingmechat_ui/presentation/pages/chat_page.dart';

import '../widgets/custom_icon.dart';
import 'create_group_page.dart';

class HomePage extends StatefulWidget {
  const HomePage({Key? key}) : super(key: key);

  @override
  _HomePageState createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  final List<ChatItem> _allChatItems = [
    ChatItem(
        name: 'Alex Linderson',
        message: 'How are you today?',
        time: '2 min ago',
        unreadCount: 3,
        imageUrl: 'assets/images/alex.jpg',
        isActive: true),
    ChatItem(
        name: 'Team Align',
        message: 'Dont miss to attend the meeting.',
        time: '2 min ago',
        unreadCount: 4,
        imageUrl: 'assets/images/team_align.jpg',
        isActive: true),
    ChatItem(
        name: 'John Ahraham',
        message: 'Hey! Can you join the meeting?',
        time: '2 min ago',
        imageUrl: 'assets/images/john_a.jpg'),
    ChatItem(
        name: 'Sabila Sayma',
        message: 'How are you today?',
        time: '2 min ago',
        imageUrl: 'assets/images/sabila.jpg'),
    ChatItem(
        name: 'John Borino',
        message: 'Have a good day 🌸',
        time: '2 min ago',
        imageUrl: 'assets/images/john_b.jpg'),
    ChatItem(
        name: 'John Borino',
        message: 'Have a good day 🌸',
        time: '2 min ago',
        imageUrl: 'assets/images/john_b.jpg'),
    ChatItem(
        name: 'John Borino',
        message: 'Have a good day 🌸',
        time: '2 min ago',
        imageUrl: 'assets/images/john_b.jpg'),
    ChatItem(
        name: 'Jane Doe',
        message: 'Hello!',
        time: '2 min ago',
        imageUrl: 'assets/images/jane.jpg'),
    ChatItem(
        name: 'John Doe',
        message: 'Hi there!',
        time: '2 min ago',
        imageUrl: 'assets/images/john.jpg'),
    ChatItem(
        name: 'Emma Watson',
        message: 'What are you up to?',
        time: '2 min ago',
        imageUrl: 'assets/images/emma.jpg'),
    ChatItem(
        name: 'Daniel Smith',
        message: 'Lets grab lunch!',
        time: '2 min ago',
        imageUrl: 'assets/images/daniel.jpg'),
    ChatItem(
        name: 'Sophia Johnson',
        message: 'Can you help me with this?',
        time: '2 min ago',
        imageUrl: 'assets/images/sophia.jpg'),
    ChatItem(
        name: 'Oliver Brown',
        message: 'I have a question for you.',
        time: '2 min ago',
        imageUrl: 'assets/images/oliver.jpg'),
    ChatItem(
        name: 'Ava Wilson',
        message: 'Are you free tomorrow?',
        time: '2 min ago',
        imageUrl: 'assets/images/ava.jpg'),
    ChatItem(
        name: 'William Davis',
        message: 'Lets go for a walk!',
        time: '2 min ago',
        imageUrl: 'assets/images/william.jpg'),
    ChatItem(
        name: 'Mia Anderson',
        message: 'I need your help!',
        time: '2 min ago',
        imageUrl: 'assets/images/mia.jpg'),
    ChatItem(
        name: 'James Martinez',
        message: 'How was your weekend?',
        time: '2 min ago',
        imageUrl: 'assets/images/james.jpg'),
    ChatItem(
        name: 'Charlotte Taylor',
        message: 'Lets catch up soon!',
        time: '2 min ago',
        imageUrl: 'assets/images/charlotte.jpg'),
    ChatItem(
        name: 'Benjamin Harris',
        message: 'Do you have any plans?',
        time: '2 min ago',
        imageUrl: 'assets/images/benjamin.jpg'),
    ChatItem(
        name: 'Harper Clark',
        message: 'I miss you!',
        time: '2 min ago',
        imageUrl: 'assets/images/harper.jpg'),
    ChatItem(
        name: 'Elijah Lewis',
        message: 'Can we talk?',
        time: '2 min ago',
        imageUrl: 'assets/images/elijah.jpg'),
    ChatItem(
        name: 'Amelia Turner',
        message: 'How are you feeling?',
        time: '2 min ago',
        imageUrl: 'assets/images/amelia.jpg'),
    ChatItem(
        name: 'Logan Walker',
        message: 'Lets hang out!',
        time: '2 min ago',
        imageUrl: 'assets/images/logan.jpg'),
    ChatItem(
        name: 'Sofia Hill',
        message: 'I have a surprise for you.',
        time: '2 min ago',
        imageUrl: 'assets/images/sofia.jpg'),
    ChatItem(
        name: 'Jackson Green',
        message: 'Can you call me?',
        time: '2 min ago',
        imageUrl: 'assets/images/jackson.jpg'),
    ChatItem(
        name: 'Lily Adams',
        message: 'I need your advice.',
        time: '2 min ago',
        imageUrl: 'assets/images/lily.jpg'),
    ChatItem(
        name: 'Sebastian Wright',
        message: 'Lets go on an adventure!',
        time: '2 min ago',
        imageUrl: 'assets/images/sebastian.jpg'),
    ChatItem(
        name: 'Zoe Parker',
        message: 'I have something to tell you.',
        time: '2 min ago',
        imageUrl: 'assets/images/zoe.jpg'),
    ChatItem(
        name: 'Michael Smith',
        message: 'Good morning!',
        time: '2 min ago',
        imageUrl: 'assets/images/michael.jpg'),
    ChatItem(
        name: 'Emily Johnson',
        message: 'How was your day?',
        time: '2 min ago',
        imageUrl: 'assets/images/emily.jpg'),
    ChatItem(
        name: 'David Brown',
        message: 'See you later!',
        time: '2 min ago',
        imageUrl: 'assets/images/david.jpg'),
    ChatItem(
        name: 'Sarah Wilson',
        message: 'Have a great weekend!',
        time: '2 min ago',
        imageUrl: 'assets/images/sarah.jpg'),
  ];
  List<ChatItem> _displayedChatItems = [];
  int _currentPage = 0;
  final int _itemsPerPage = 20;
  bool _isLoading = false;
  bool _isAddOptionsVisible = false;
  final ScrollController _scrollController = ScrollController();
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

  final List<ChatItem> chatItems = [
    ChatItem(
        name: 'Alex Linderson',
        message: 'How are you today?',
        time: '2 min ago',
        unreadCount: 3,
        imageUrl: 'assets/images/alex.jpg'),
    ChatItem(
        name: 'Team Align',
        message: 'Dont miss to attend the meeting.',
        time: '2 min ago',
        unreadCount: 4,
        imageUrl: 'assets/images/team_align.jpg'),
    ChatItem(
        name: 'John Ahraham',
        message: 'Hey! Can you join the meeting?',
        time: '2 min ago',
        imageUrl: 'assets/images/john_a.jpg'),
    ChatItem(
        name: 'Sabila Sayma',
        message: 'How are you today?',
        time: '2 min ago',
        imageUrl: 'assets/images/sabila.jpg'),
    ChatItem(
        name: 'John Borino',
        message: 'Have a good day 🌸',
        time: '2 min ago',
        imageUrl: 'assets/images/john_b.jpg'),
    ChatItem(
        name: 'John Borino',
        message: 'Have a good day 🌸',
        time: '2 min ago',
        imageUrl: 'assets/images/john_b.jpg'),
    ChatItem(
        name: 'John Borino',
        message: 'Have a good day 🌸',
        time: '2 min ago',
        imageUrl: 'assets/images/john_b.jpg'),
    ChatItem(
        name: 'Jane Doe',
        message: 'Hello!',
        time: '2 min ago',
        imageUrl: 'assets/images/jane.jpg'),
    ChatItem(
        name: 'John Doe',
        message: 'Hi there!',
        time: '2 min ago',
        imageUrl: 'assets/images/john.jpg'),
    ChatItem(
        name: 'Emma Watson',
        message: 'What are you up to?',
        time: '2 min ago',
        imageUrl: 'assets/images/emma.jpg'),
    ChatItem(
        name: 'Daniel Smith',
        message: 'Lets grab lunch!',
        time: '2 min ago',
        imageUrl: 'assets/images/daniel.jpg'),
    ChatItem(
        name: 'Sophia Johnson',
        message: 'Can you help me with this?',
        time: '2 min ago',
        imageUrl: 'assets/images/sophia.jpg'),
    ChatItem(
        name: 'Oliver Brown',
        message: 'I have a question for you.',
        time: '2 min ago',
        imageUrl: 'assets/images/oliver.jpg'),
    ChatItem(
        name: 'Ava Wilson',
        message: 'Are you free tomorrow?',
        time: '2 min ago',
        imageUrl: 'assets/images/ava.jpg'),
    ChatItem(
        name: 'William Davis',
        message: 'Lets go for a walk!',
        time: '2 min ago',
        imageUrl: 'assets/images/william.jpg'),
    ChatItem(
        name: 'Mia Anderson',
        message: 'I need your help!',
        time: '2 min ago',
        imageUrl: 'assets/images/mia.jpg'),
    ChatItem(
        name: 'James Martinez',
        message: 'How was your weekend?',
        time: '2 min ago',
        imageUrl: 'assets/images/james.jpg'),
    ChatItem(
        name: 'Charlotte Taylor',
        message: 'Lets catch up soon!',
        time: '2 min ago',
        imageUrl: 'assets/images/charlotte.jpg'),
    ChatItem(
        name: 'Benjamin Harris',
        message: 'Do you have any plans?',
        time: '2 min ago',
        imageUrl: 'assets/images/benjamin.jpg'),
    ChatItem(
        name: 'Harper Clark',
        message: 'I miss you!',
        time: '2 min ago',
        imageUrl: 'assets/images/harper.jpg'),
    ChatItem(
        name: 'Elijah Lewis',
        message: 'Can we talk?',
        time: '2 min ago',
        imageUrl: 'assets/images/elijah.jpg'),
    ChatItem(
        name: 'Amelia Turner',
        message: 'How are you feeling?',
        time: '2 min ago',
        imageUrl: 'assets/images/amelia.jpg'),
    ChatItem(
        name: 'Logan Walker',
        message: 'Lets hang out!',
        time: '2 min ago',
        imageUrl: 'assets/images/logan.jpg'),
    ChatItem(
        name: 'Sofia Hill',
        message: 'I have a surprise for you.',
        time: '2 min ago',
        imageUrl: 'assets/images/sofia.jpg'),
    ChatItem(
        name: 'Jackson Green',
        message: 'Can you call me?',
        time: '2 min ago',
        imageUrl: 'assets/images/jackson.jpg'),
    ChatItem(
        name: 'Lily Adams',
        message: 'I need your advice.',
        time: '2 min ago',
        imageUrl: 'assets/images/lily.jpg'),
    ChatItem(
        name: 'Sebastian Wright',
        message: 'Lets go on an adventure!',
        time: '2 min ago',
        imageUrl: 'assets/images/sebastian.jpg'),
    ChatItem(
        name: 'Zoe Parker',
        message: 'I have something to tell you.',
        time: '2 min ago',
        imageUrl: 'assets/images/zoe.jpg'),
    ChatItem(
        name: 'Michael Smith',
        message: 'Good morning!',
        time: '2 min ago',
        imageUrl: 'assets/images/michael.jpg'),
    ChatItem(
        name: 'Emily Johnson',
        message: 'How was your day?',
        time: '2 min ago',
        imageUrl: 'assets/images/emily.jpg'),
    ChatItem(
        name: 'David Brown',
        message: 'See you later!',
        time: '2 min ago',
        imageUrl: 'assets/images/david.jpg'),
    ChatItem(
        name: 'Sarah Wilson',
        message: 'Have a great weekend!',
        time: '2 min ago',
        imageUrl: 'assets/images/sarah.jpg'),
  ];

  @override
  void initState() {
    super.initState();
    _loadingMoreItems(); // Load the first page of items
    _scrollController.addListener(_onScroll);
  }

  void _onScroll() {
    if (_scrollController.position.pixels ==
        _scrollController.position.maxScrollExtent) {
      _loadingMoreItems();
    }
  }

  @override
  void dispose() {
    _scrollController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    SystemChrome.setSystemUIOverlayStyle(SystemUiOverlayStyle.light);
    return Scaffold(
      backgroundColor: AppColors.background,
      body: SafeArea(
        child: Column(
          children: [
            _buildAppBar(),
            _buildStatusList(),
            _buildRoundedChatList(),
            _buildBottomNavBar(),
          ],
        ),
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
                itemCount: _displayedChatItems.length + 1,
                itemBuilder: (context, index) {
                  if (index < _displayedChatItems.length) {
                    return _buildSlidableChatItem(_displayedChatItems[index]);
                  } else if (_isLoading) {
                    return _buildLoadingIndicator();
                  } else {
                    return const SizedBox.shrink();
                  }
                },
                itemExtent: 60, // Assuming each item has a fixed height
              ),
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildLoadingIndicator() {
    return Container(
      padding: const EdgeInsets.all(16),
      alignment: Alignment.center,
      child: const CircularProgressIndicator(
        valueColor: AlwaysStoppedAnimation<Color>(AppColors.primary),
      ),
    );
  }

  Future<void> _refreshChatList() async {
    // Simulate a network request
    await Future.delayed(const Duration(seconds: 2));
    // In a real app, you would fetch new data here
    setState(() {
      _currentPage = 0;
      _displayedChatItems.clear();
      // Update the chat list with new data
    });
    _loadingMoreItems();
  }

  Widget _buildBottomNavBar() {
    return BottomNavigationBar(
      backgroundColor: AppColors.surface,
      type: BottomNavigationBarType.fixed,
      selectedItemColor: AppColors.primary,
      unselectedItemColor: AppColors.tertiary,
      items: [
        BottomNavigationBarItem(
          icon: CustomSvgIcon(svgPath: 'assets/icons/Message.svg'),
          label: AppLocalizations.of(context)!.message,
        ),
        BottomNavigationBarItem(
          icon: CustomSvgIcon(svgPath: 'assets/icons/Call.svg'),
          label: AppLocalizations.of(context)!.calls,
        ),
        BottomNavigationBarItem(
          icon: CustomSvgIcon(svgPath: 'assets/icons/user.svg'),
          label: AppLocalizations.of(context)!.contacts,
        ),
        BottomNavigationBarItem(
          icon: CustomSvgIcon(svgPath: 'assets/icons/settings.svg'),
          label: AppLocalizations.of(context)!.settings,
        ),
      ],
    );
  }

  Widget _buildSlidableChatItem(ChatItem item) {
    return Slidable(
      key: ValueKey(item),
      endActionPane: ActionPane(
        motion: const ScrollMotion(),
        extentRatio: 0.45, // Giảm kích thước của khu vực action
        children: [
          CustomSlidableAction(
            onPressed: (context) => _handleNotification(item),
            backgroundColor: AppColors.surface,
            child: CustomActionIcon(
              color: AppColors.secondary,
              svgPath:
                  'assets/icons/notification.svg', // Đường dẫn đến file SVG của bạn
            ),
          ),
          CustomSlidableAction(
            onPressed: (context) => _handleDelete(item),
            backgroundColor: AppColors.surface,
            child: CustomActionIcon(
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

  Widget _buildChatItem(ChatItem item) {
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
            backgroundImage: AssetImage(item.imageUrl),
          ),
          if (item.isActive)
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
          item.name,
          style: AppTypography.chatName,
        ),
        subtitle: Text(
          item.message,
          style: AppTypography.chatMessage,
          maxLines: 1,
          overflow: TextOverflow.ellipsis,
        ),
        trailing: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          crossAxisAlignment: CrossAxisAlignment.end,
          children: [
            Text(
              item.time,
              style: AppTypography.caption,
            ),
            if (item.unreadCount != null && item.unreadCount! > 0)
              Container(
                padding: EdgeInsets.all(6),
                decoration: const BoxDecoration(
                  color: AppColors.red,
                  shape: BoxShape.circle,
                ),
                child: Text(
                  item.unreadCount.toString(),
                  style: AppTypography.badge,
                ),
              ),
          ],
        ),
      ),
    );
  }

  void _handleNotification(ChatItem item) {
    // Xử lý khi nhấn vào nút thông báo, thông báo bằng snackbar
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text('Notification sent to ${item.name}'),
        duration: const Duration(seconds: 2),
      ),
    );
    // Thêm logic xử lý thông báo ở đây
  }

  void _handleDelete(ChatItem item) {
    // Xử lý khi nhấn vào nút xóa
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text('Delete ${item.name}'),
        duration: const Duration(seconds: 2),
      ),
    );
  }

  void _loadingMoreItems() {
    if (_isLoading) return;
    setState(() {
      _isLoading = true;
    });
    // Simulate a network request
    Future.delayed(const Duration(seconds: 2), () {
      final start = _currentPage * _itemsPerPage;
      final end = start + _itemsPerPage;
      final newItems = _allChatItems.sublist(
          start, end > _allChatItems.length ? _allChatItems.length : end);

      setState(() {
        _displayedChatItems.addAll(newItems);
        _currentPage++;
        _isLoading = false;
      });
    });
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

class ChatItem {
  final String name;
  final String message;
  final String time;
  final int? unreadCount;
  final String imageUrl;
  final bool isActive;

  const ChatItem({
    required this.name,
    required this.message,
    required this.time,
    this.unreadCount,
    required this.imageUrl,
    this.isActive = false,
  });
}
