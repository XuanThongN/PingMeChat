import 'package:flutter/material.dart';
import 'package:pingmechat_ui/presentation/widgets/app_bar.dart';
import 'package:provider/provider.dart';

import '../../providers/auth_provider.dart';
import 'home.dart';
import 'login_page.dart';

class SetttingTab extends StatefulWidget {
  const SetttingTab({super.key});

  @override
  State<SetttingTab> createState() => _SetttingTabState();
}

class _SetttingTabState extends State<SetttingTab> {
  @override
  Widget build(BuildContext context) {
    final authProvider = Provider.of<AuthProvider>(context);
    final currentUser = authProvider.currentUser;

    return Scaffold(
      appBar: CustomAppBar(
        onBackButtonPressed: () {
         // Trở về trang home theo routeName
         Navigator.of(context).pushNamed(HomePage.routeName);
        },
      ),
      body: SingleChildScrollView(
        child: Padding(
          padding: const EdgeInsets.all(16.0),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              if (currentUser != null) ...[
                const Center(
                  child: CircleAvatar(
                    radius: 50,
                    backgroundImage: NetworkImage(
                      'https://via.placeholder.com/150', // Replace with user's profile picture URL
                    ),
                  ),
                ),
                const SizedBox(height: 20),
                Center(
                  child: Text(
                    currentUser.fullName,
                    style: const TextStyle(
                      fontSize: 24,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
                const SizedBox(height: 10),
                Center(
                  child: Text(
                    currentUser.email,
                    style: TextStyle(
                      fontSize: 16,
                      color: Colors.grey[600],
                    ),
                  ),
                ),
                const SizedBox(height: 20),
              ],
              const Divider(),
              ListTile(
                leading: const Icon(Icons.person),
                title: const Text('Account'),
                onTap: () {
                  // Navigate to account settings
                },
              ),
              ListTile(
                leading: const Icon(Icons.notifications),
                title: const Text('Notifications'),
                onTap: () {
                  // Navigate to notification settings
                },
              ),
              ListTile(
                leading: const Icon(Icons.lock),
                title: const Text('Privacy'),
                onTap: () {
                  // Navigate to privacy settings
                },
              ),
              ListTile(
                leading: const Icon(Icons.help),
                title: const Text('Help & Support'),
                onTap: () {
                  // Navigate to help & support
                },
              ),
              const SizedBox(height: 20),
              Center(
                child: ElevatedButton(
                  onPressed: () async {
                    try {
                      await authProvider.logout(context);
                      Navigator.of(context)
                          .pushReplacementNamed(LoginPage.routeName);
                    } catch (e) {
                      ScaffoldMessenger.of(context).showSnackBar(
                        SnackBar(content: Text('Logout failed: $e')),
                      );
                    }
                  },
                  style: ElevatedButton.styleFrom(
                    padding: const EdgeInsets.symmetric(horizontal: 50, vertical: 15),
                    textStyle: const TextStyle(
                      fontSize: 18,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  child: const Text('Logout'),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
