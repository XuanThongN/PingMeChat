import 'package:flutter/material.dart';
import 'package:flutter_slidable/flutter_slidable.dart';
import 'package:pingmechat_ui/presentation/widgets/app_bar.dart';
import '../../config/theme.dart';
import '../widgets/custom_circle_avatar.dart'; // Assuming your theme settings are configured here

class CallTab extends StatefulWidget {
  const CallTab({super.key});

  @override
  _CallTabState createState() => _CallTabState();
}

class _CallTabState extends State<CallTab> {
  final List<CallItem> _callItems = [
    CallItem(
      name: 'Team Align',
      time: 'Today, 09:30 AM',
      callTypeIcon: Icons.call_received,
      callIconColor: Colors.green,
      imageUrl: 'assets/images/team_align.jpg',
    ),
    CallItem(
      name: 'Jhon Abraham',
      time: 'Today, 07:30 AM',
      callTypeIcon: Icons.call_made,
      callIconColor: Colors.blue,
      imageUrl: 'assets/images/john_a.jpg',
    ),
    // Thêm 10 call items khác
    CallItem(
      name: 'Jane Doe',
      time: 'Yesterday, 09:30 PM',
      callTypeIcon: Icons.call_received,
      callIconColor: Colors.green,
      imageUrl: 'assets/images/jane_doe.jpg',
    ),
    CallItem(
      name: 'Jenny Doe',
      time: 'Yesterday, 07:30 PM',
      callTypeIcon: Icons.call_made,
      callIconColor: Colors.blue,
      imageUrl: 'assets/images/jenny_doe.jpg',
    ),
    CallItem(
      name: 'John Doe',
      time: 'Yesterday, 05:30 PM',
      callTypeIcon: Icons.call_received,
      callIconColor: Colors.green,
      imageUrl: 'assets/images/john_doe.jpg',
    ),
    CallItem(
      name: 'Jane Doe',
      time: 'Yesterday, 03:30 PM',
      callTypeIcon: Icons.call_made,
      callIconColor: Colors.blue,
      imageUrl: 'assets/images/jane_doe.jpg',
    ),
    CallItem(
      name: 'Jenny Doe',
      time: 'Yesterday, 01:30 PM',
      callTypeIcon: Icons.call_received,
      callIconColor: Colors.green,
      imageUrl: 'assets/images/jenny_doe.jpg',
    ),
    CallItem(
      name: 'John Doe',
      time: 'Yesterday, 11:30 AM',
      callTypeIcon: Icons.call_made,
      callIconColor: Colors.blue,
      imageUrl: 'assets/images/john_doe.jpg',
    ),
    CallItem(
      name: 'Jane Doe',
      time: 'Yesterday, 09:30 AM',
      callTypeIcon: Icons.call_received,
      callIconColor: Colors.green,
      imageUrl: 'assets/images/jane_doe.jpg',
    ),
    CallItem(
      name: 'Jenny Doe',
      time: 'Yesterday, 07:30 AM',
      callTypeIcon: Icons.call_made,
      callIconColor: Colors.blue,
      imageUrl: 'assets/images/jenny_doe.jpg',
    ),
    //Thêm vài cuộc gọi nhỡ
    CallItem(
      name: 'John Doe',
      time: 'Yesterday, 05:30 AM',
      callTypeIcon: Icons.call_missed,
      callIconColor: Colors.red,
      imageUrl: 'assets/images/john_doe.jpg',
    ),
  ];

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: const CustomAppBar(),
      body: ListView.builder(
        itemCount: _callItems.length,
        itemBuilder: (context, index) => _buildCallItem(_callItems[index]),
      ),
    );
  }

  Widget _buildCallItem(CallItem item) {
    return Slidable(
      startActionPane: ActionPane(
        motion: const DrawerMotion(),
        children: [
          SlidableAction(
            onPressed: (context) => _deleteCall(item),
            backgroundColor: Colors.red,
            foregroundColor: Colors.white,
            icon: Icons.delete,
            label: 'Delete',
          ),
        ],
      ),
      endActionPane: ActionPane(
        motion: const DrawerMotion(),
        children: [
          SlidableAction(
            onPressed: (context) => _makeCall(item),
            backgroundColor: Colors.green,
            foregroundColor: Colors.white,
            icon: Icons.call,
            label: 'Call',
          ),
        ],
      ),
      child: ListTile(
        leading: CustomCircleAvatar(
          backgroundImage: AssetImage(item.imageUrl),
          radius: 24,
        ),
        title: Text(item.name, style: Theme.of(context).textTheme.titleMedium),
        subtitle:
            Text(item.time, style: Theme.of(context).textTheme.titleSmall),
        trailing: Icon(item.callTypeIcon, color: item.callIconColor),
      ),
    );
  }

  void _deleteCall(CallItem item) {
    setState(() {
      _callItems.remove(item);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Deleted call with ${item.name}'),
          duration: const Duration(seconds: 2),
        ),
      );
    });
  }

  void _makeCall(CallItem item) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text('Calling ${item.name}'),
        duration: const Duration(seconds: 2),
      ),
    );
    // Integrate with call-making functionality here
  }
}

class CallItem {
  final String name;
  final String time;
  final IconData callTypeIcon;
  final Color callIconColor;
  final String imageUrl;

  CallItem({
    required this.name,
    required this.time,
    required this.callTypeIcon,
    required this.callIconColor,
    required this.imageUrl,
  });
}
