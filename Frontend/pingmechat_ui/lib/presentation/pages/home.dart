import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:pingmechat_ui/config/theme.dart';
import 'package:flutter_slidable/flutter_slidable.dart';

class HomePage extends StatefulWidget {
  HomePage({Key? key}) : super(key: key);

  @override
  _HomePageState createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
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
  Widget build(BuildContext context) {
    SystemChrome.setSystemUIOverlayStyle(SystemUiOverlayStyle.light);
    return Scaffold(
      backgroundColor: Color(0xFF121212),
      body: SafeArea(
        child: Column(
          children: [
            _buildAppBar(),
            _buildStatusList(),
            _buildRoundedChatList(), // This is now wrapped in Expanded
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
          _buildCircularButton(Icons.search, () {}),
          Text('Home',
              style: TextStyle(
                  color: Colors.white,
                  fontSize: 20,
                  fontWeight: FontWeight.bold)),
          CircleAvatar(
              backgroundImage: AssetImage('assets/images/profile.jpg'),
              radius: 18),
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
      ),
    );
  }

  Widget _buildStatusList() {
    return Container(
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
                  backgroundImage: AssetImage(item.imageUrl),
                  radius: 30,
                ),
              ),
              if (item.isMyStatus)
                Positioned(
                  right: 0,
                  bottom: 0,
                  child: Container(
                    padding: EdgeInsets.all(4),
                    decoration: BoxDecoration(
                      color: Colors.blue,
                      shape: BoxShape.circle,
                    ),
                    child: Icon(Icons.add, color: Colors.white, size: 14),
                  ),
                ),
            ],
          ),
          SizedBox(height: 4),
          Text(item.name, style: TextStyle(color: Colors.white, fontSize: 12)),
        ],
      ),
    );
  }

  Widget _buildRoundedChatList() {
    return Expanded(
      child: Container(
        decoration: BoxDecoration(
          color: Color(0xFF1E1E1E),
          borderRadius: BorderRadius.only(
            topLeft: Radius.circular(30),
            topRight: Radius.circular(30),
          ),
        ),
        child: ClipRRect(
          borderRadius: BorderRadius.only(
            topLeft: Radius.circular(30),
            topRight: Radius.circular(30),
          ),
          child: SlidableAutoCloseBehavior(
            child: ListView.builder(
              itemCount: chatItems.length,
              itemBuilder: (context, index) => _buildSlidableChatItem(chatItems[index]),
            ),
          )
        ),
      ),
    );
  }


  Widget _buildChatItem(ChatItem item) {
    return ListTile(
      leading:
          CircleAvatar(backgroundImage: AssetImage(item.imageUrl), radius: 25),
      title: Text(item.name,
          style: TextStyle(
              color: AppColors.secondary, fontWeight: FontWeight.bold)),
      subtitle: Text(item.message, style: TextStyle(color: AppColors.tertiary)),
      trailing: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        crossAxisAlignment: CrossAxisAlignment.end,
        children: [
          Text(item.time,
              style: TextStyle(color: Colors.grey[600], fontSize: 12)),
          if (item.unreadCount != null)
            Container(
              margin: EdgeInsets.only(top: 4),
              padding: EdgeInsets.all(6),
              decoration:
                  BoxDecoration(color: Colors.red, shape: BoxShape.circle),
              child: Text('${item.unreadCount}',
                  style: TextStyle(color: Colors.white, fontSize: 12)),
            ),
        ],
      ),
    );
  }

  Widget _buildBottomNavBar() {
    return BottomNavigationBar(
      backgroundColor: AppColors.surface,
      type: BottomNavigationBarType.fixed,
      selectedItemColor: Colors.teal,
      unselectedItemColor: Colors.grey,
      items: [
        BottomNavigationBarItem(
            icon: Icon(Icons.chat_bubble_outline), label: 'Message'),
        BottomNavigationBarItem(
            icon: Icon(Icons.phone_outlined), label: 'Calls'),
        BottomNavigationBarItem(
            icon: Icon(Icons.people_outline), label: 'Contacts'),
        BottomNavigationBarItem(
            icon: Icon(Icons.settings_outlined), label: 'Settings'),
      ],
    );
  }

  Widget _buildSlidableChatItem(ChatItem item) {
    return Slidable(
      key: ValueKey(item), // Thêm key để xác định duy nhất mỗi item
      // controller: slidableController,
      endActionPane: ActionPane(
        motion: ScrollMotion(),
        extentRatio: 0.35, // Điều chỉnh độ rộng của action pane
        children: [
          CustomSlidableAction(
            onPressed: (context) {},
            backgroundColor: Colors.transparent,
            foregroundColor: Colors.white,
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Container(
                  padding: EdgeInsets.all(10),
                  decoration: BoxDecoration(
                    color: Colors.green,
                    shape: BoxShape.circle,
                  ),
                  child: Icon(Icons.notifications, size: 20),
                ),
                SizedBox(height: 4),
                Text('Notify', style: TextStyle(fontSize: 12)),
              ],
            ),
          ),
          CustomSlidableAction(
            onPressed: (context) {},
            backgroundColor: Colors.transparent,
            foregroundColor: Colors.white,
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Container(
                  padding: EdgeInsets.all(10),
                  decoration: BoxDecoration(
                    color: Colors.red,
                    shape: BoxShape.circle,
                  ),
                  child: Icon(Icons.delete, size: 20),
                ),
                SizedBox(height: 4),
                Text('Delete', style: TextStyle(fontSize: 12)),
              ],
            ),
          ),
        ],
      ),
      child: _buildChatItem(item),
    );
  }
}

class StatusItem {
  final String name;
  final String imageUrl;
  final bool isOnline;
  final bool isMyStatus;

  StatusItem(
      {required this.name,
      required this.imageUrl,
      this.isOnline = false,
      this.isMyStatus = false});
}

class ChatItem {
  final String name;
  final String message;
  final String time;
  final int? unreadCount;
  final String imageUrl;

  ChatItem(
      {required this.name,
      required this.message,
      required this.time,
      this.unreadCount,
      required this.imageUrl});
}
