import 'package:flutter/material.dart';

import 'chat_media_links_document_page.dart';

class UserInformationPage extends StatefulWidget {
  @override
  _UserInformationPageState createState() => _UserInformationPageState();
}

class _UserInformationPageState extends State<UserInformationPage> {
  int _selectedIndex = 0;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      appBar: AppBar(
        elevation: 0,
        backgroundColor: Colors.white,
        leading: IconButton(
          icon: Icon(Icons.arrow_back, color: Colors.black),
          onPressed: () {
            Navigator.pop(context);
          },
        ),
      ),
      body: SingleChildScrollView(
        child: Column(
          children: [
            Center(
              child: Column(
                children: [
                  CircleAvatar(
                    radius: 50,
                    backgroundImage: //Image from internet
                        NetworkImage(
                            'https://plus.unsplash.com/premium_photo-1670148434900-5f0af77ba500?fm=jpg&q=60&w=3000&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MXx8c3BsYXNofGVufDB8fDB8fHww'),
                  ),
                  SizedBox(height: 10),
                  Text(
                    'David Wayne',
                    style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
                  ),
                  Text(
                    '(+44) 20 1234 5689',
                    style: TextStyle(color: Colors.grey),
                  ),
                  IconButton(
                    icon: Icon(Icons.copy, color: Colors.grey),
                    onPressed: () {},
                  ),
                ],
              ),
            ),
            Divider(),
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 20.0),
              child: Column(
                children: [
                  SettingsOption(
                    icon: Icons.link,
                    title: 'Media, Links & Documents',
                    trailing: '152',
                    onTap: () {
                      //Chuyển tới trang Media, Links & Documents
                      Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (context) => MediaLinksDocumentsPage(),
                        ),
                      );
                    },
                  ),
                  SettingsSwitchOption(
                    icon: Icons.notifications_off,
                    title: 'Mute Notification',
                    value: false,
                    onChanged: (bool value) {},
                  ),
                  SettingsOption(
                    icon: Icons.notification_important,
                    title: 'Custom Notification',
                    onTap: () {},
                  ),
                  SettingsSwitchOption(
                    icon: Icons.lock,
                    title: 'Protected Chat',
                    value: false,
                    onChanged: (bool value) {},
                  ),
                  SettingsSwitchOption(
                    icon: Icons.visibility_off,
                    title: 'Hide Chat',
                    value: false,
                    onChanged: (bool value) {},
                  ),
                  SettingsSwitchOption(
                    icon: Icons.history,
                    title: 'Hide Chat History',
                    value: false,
                    onChanged: (bool value) {},
                  ),
                  SettingsOption(
                    icon: Icons.group,
                    title: 'Add To Group',
                    onTap: () {},
                  ),
                  SettingsColorOption(
                    icon: Icons.color_lens,
                    title: 'Custom Color Chat',
                    color: Colors.blue,
                    onTap: () {},
                  ),
                  SettingsOption(
                    icon: Icons.image,
                    title: 'Custom Background Chat',
                    onTap: () {},
                  ),
                  Divider(),
                  ListTile(
                    leading: Icon(Icons.report, color: Colors.red),
                    title: Text('Report', style: TextStyle(color: Colors.red)),
                    onTap: () {},
                  ),
                  ListTile(
                    leading: Icon(Icons.block, color: Colors.red),
                    title: Text('Block', style: TextStyle(color: Colors.red)),
                    onTap: () {},
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class SettingsOption extends StatelessWidget {
  final IconData icon;
  final String title;
  final String? trailing;
  final VoidCallback onTap;

  SettingsOption(
      {required this.icon,
      required this.title,
      this.trailing,
      required this.onTap});

  @override
  Widget build(BuildContext context) {
    return ListTile(
      leading: Icon(icon, color: Colors.black),
      title: Text(title),
      trailing: trailing != null
          ? Text(trailing!, style: TextStyle(color: Colors.grey))
          : Icon(Icons.arrow_forward_ios, size: 16),
      onTap: onTap,
    );
  }
}

class SettingsSwitchOption extends StatelessWidget {
  final IconData icon;
  final String title;
  final bool value;
  final ValueChanged<bool> onChanged;

  SettingsSwitchOption(
      {required this.icon,
      required this.title,
      required this.value,
      required this.onChanged});

  @override
  Widget build(BuildContext context) {
    return ListTile(
      leading: Icon(icon, color: Colors.black),
      title: Text(title),
      trailing: Switch(
        value: value,
        onChanged: onChanged,
      ),
    );
  }
}

class SettingsColorOption extends StatelessWidget {
  final IconData icon;
  final String title;
  final Color color;
  final VoidCallback onTap;

  SettingsColorOption(
      {required this.icon,
      required this.title,
      required this.color,
      required this.onTap});

  @override
  Widget build(BuildContext context) {
    return ListTile(
      leading: Icon(icon, color: Colors.black),
      title: Text(title),
      trailing: GestureDetector(
        onTap: onTap,
        child: Container(
          width: 20,
          height: 20,
          decoration: BoxDecoration(
            color: color,
            shape: BoxShape.circle,
          ),
        ),
      ),
      onTap: onTap,
    );
  }
}
