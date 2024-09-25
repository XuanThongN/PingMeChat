import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart';

class CallHistoryPage extends StatelessWidget {
  const CallHistoryPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        backgroundColor: Colors.black,
        elevation: 0,
        title: const Text('Calls', style: TextStyle(color: Colors.white)),
        actions: [
          IconButton(
            icon: const Icon(Icons.search, color: Colors.white),
            onPressed: () {},
          ),
          IconButton(
            icon: const Icon(Icons.add_call, color: Colors.white),
            onPressed: () {},
          ),
        ],
      ),
      body: ListView(
        children: [
          const ListTile(
            title: Text('Recent',
                style:
                    TextStyle(color: Colors.grey, fontWeight: FontWeight.bold)),
          ),
          ..._buildCallItems(),
        ],
      ),
      bottomNavigationBar: _buildBottomNavBar(context),
    );
  }

  List<Widget> _buildCallItems() {
    List<Map<String, dynamic>> callData = [
      {
        'name': 'Team Align',
        'time': 'Today, 09:30 AM',
        'icon': Icons.call_received,
        'iconColor': Colors.green,
        'avatarUrl': 'https://via.placeholder.com/150',
      },
      {
        'name': 'Jhon Abraham',
        'time': 'Today, 07:30 AM',
        'icon': Icons.call_made,
        'iconColor': Colors.blue,
        'avatarUrl': 'https://via.placeholder.com/150',
      },
      // Add more entries as needed
    ];

    return callData.map((call) {
      return ListTile(
        leading: CircleAvatar(
          backgroundImage: NetworkImage(call['avatarUrl']),
        ),
        title: Text(call['name']),
        subtitle: Text(call['time']),
        trailing: Icon(call['icon'], color: call['iconColor']),
      );
    }).toList();
  }

  Widget _buildBottomNavBar(BuildContext context) {
    return BottomNavigationBar(
      backgroundColor: Colors.black,
      type: BottomNavigationBarType.fixed,
      selectedItemColor: Colors.white,
      unselectedItemColor: Colors.grey,
      items: const [
        BottomNavigationBarItem(
          icon: Icon(Icons.message),
          label: 'Message',
        ),
        BottomNavigationBarItem(
          icon: Icon(Icons.call),
          label: 'Calls',
        ),
        BottomNavigationBarItem(
          icon: Icon(Icons.contacts),
          label: 'Contacts',
        ),
        BottomNavigationBarItem(
          icon: Icon(Icons.settings),
          label: 'Settings',
        ),
      ],
    );
  }
}
