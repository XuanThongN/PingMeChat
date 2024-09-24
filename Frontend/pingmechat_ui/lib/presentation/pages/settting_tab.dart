import 'package:flutter/material.dart';
import 'package:pingmechat_ui/presentation/widgets/app_bar.dart';
import 'package:provider/provider.dart';

import '../../providers/auth_provider.dart';
import 'login_page.dart';

class SetttingTab extends StatefulWidget {
  @override
  State<SetttingTab> createState() => _SetttingTabState();
}

class _SetttingTabState extends State<SetttingTab> {
  @override
  Widget build(BuildContext context) {
    final authProvider = Provider.of<AuthProvider>(context);
    final currentUser = authProvider.currentUser;

    return Scaffold(
      appBar: CustomAppBar(),
      body: SingleChildScrollView(
        child: Padding(
          padding: const EdgeInsets.all(16.0),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              if (currentUser != null) ...[
                Center(
                  child: CircleAvatar(
                    radius: 50,
                    backgroundImage: NetworkImage(
                      'https://via.placeholder.com/150', // Replace with user's profile picture URL
                    ),
                  ),
                ),
                SizedBox(height: 20),
                Center(
                  child: Text(
                    currentUser.fullName,
                    style: TextStyle(
                      fontSize: 24,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
                SizedBox(height: 10),
                Center(
                  child: Text(
                    currentUser.email,
                    style: TextStyle(
                      fontSize: 16,
                      color: Colors.grey[600],
                    ),
                  ),
                ),
                SizedBox(height: 20),
              ],
              Divider(),
              ListTile(
                leading: Icon(Icons.person),
                title: Text('Account'),
                onTap: () {
                  // Navigate to account settings
                },
              ),
              ListTile(
                leading: Icon(Icons.notifications),
                title: Text('Notifications'),
                onTap: () {
                  // Navigate to notification settings
                },
              ),
              ListTile(
                leading: Icon(Icons.lock),
                title: Text('Privacy'),
                onTap: () {
                  // Navigate to privacy settings
                },
              ),
              ListTile(
                leading: Icon(Icons.help),
                title: Text('Help & Support'),
                onTap: () {
                  // Navigate to help & support
                },
              ),
              SizedBox(height: 20),
              Center(
                child: ElevatedButton(
                  onPressed: () async {
                    try {
                      await authProvider.logout();
                      Navigator.of(context)
                          .pushReplacementNamed(LoginPage.routeName);
                    } catch (e) {
                      ScaffoldMessenger.of(context).showSnackBar(
                        SnackBar(content: Text('Logout failed: $e')),
                      );
                    }
                  },
                  child: Text('Logout'),
                  style: ElevatedButton.styleFrom(
                    padding: EdgeInsets.symmetric(horizontal: 50, vertical: 15),
                    textStyle: TextStyle(
                      fontSize: 18,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
