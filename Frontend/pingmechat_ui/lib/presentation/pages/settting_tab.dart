import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../domain/models/account.dart';
import '../../providers/auth_provider.dart';
import '../widgets/custom_circle_avatar.dart';
import '../../config/theme.dart';
import 'package:image_picker/image_picker.dart';

class SettingTab extends StatelessWidget {
  const SettingTab({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    final authProvider = Provider.of<AuthProvider>(context);
    final currentUser = authProvider.currentUser;

    return Scaffold(
      backgroundColor: AppColors.background,
      body: SafeArea(
        child: Column(
          children: [
            _buildAppBar(),
            Expanded(
              child: Container(
                margin: const EdgeInsets.only(top: 20),
                decoration: const BoxDecoration(
                  color: Colors.white,
                  borderRadius: BorderRadius.only(
                    topLeft: Radius.circular(30),
                    topRight: Radius.circular(30),
                  ),
                ),
                child: ListView(
                  padding: const EdgeInsets.all(20),
                  children: [
                    _buildUserInfo(currentUser),
                    const SizedBox(height: 20),
                    _buildSettingItem(Icons.key, 'Account', 'Privacy, security, change number', context),
                    // _buildSettingItem(Icons.chat_bubble, 'Chat', 'Chat history, theme, wallpapers', context),
                    // _buildSettingItem(Icons.notifications, 'Notifications', 'Messages, group and others', context),
                    // _buildSettingItem(Icons.help, 'Help', 'Help center, contact us, privacy policy', context  ),
                    // _buildSettingItem(Icons.storage, 'Storage and data', 'Network usage, storage usage', context),
                    // _buildSettingItem(Icons.person_add, 'Invite a friend', '', context),
                    const SizedBox(height: 20),
                    _buildLogoutButton(context, authProvider),
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildAppBar() {
    return Padding(
      padding: const EdgeInsets.all(16),
      child: Row(
        children: [
          IconButton(
            icon: const Icon(Icons.arrow_back, color: Colors.white),
            onPressed: () {
              // TODO: Handle back navigation
            },
          ),
          const Expanded(
            child: Text(
              'Settings',
              style: TextStyle(color: Colors.white, fontSize: 24, fontWeight: FontWeight.bold),
              textAlign: TextAlign.center,
            ),
          ),
          const SizedBox(width: 48), // To balance the back button
        ],
      ),
    );
  }

  Widget _buildUserInfo(Account? user) {
    return Row(
      children: [
        CustomCircleAvatar(
          radius: 30,
          backgroundImage: user?.avatarUrl != null ? NetworkImage(user!.avatarUrl!) : null,
        ),
        const SizedBox(width: 16),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                user?.fullName ?? 'Unknown User',
                style: const TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
              ),
              Text(
                user?.email ?? user!.phoneNumber!,
                style: TextStyle(color: Colors.grey[600]),
              ),
            ],
          ),
        ),
        IconButton(
          icon: const Icon(Icons.qr_code, color: AppColors.primary),
          onPressed: () {
            // TODO: Handle QR code action
          },
        ),
      ],
    );
  }

  Widget _buildSettingItem(IconData icon, String title, String subtitle, BuildContext context) {
    return ListTile(
      leading: CircleAvatar(
        backgroundColor: Colors.grey[200],
        child: Icon(icon, color: AppColors.primary),
      ),
      title: Text(title, style: const TextStyle(fontWeight: FontWeight.bold)),
      subtitle: Text(subtitle, style: TextStyle(color: Colors.grey[600])),
      onTap: () {
        if (title == 'Account') {
          _showAccountDetails(context);
        }
        // Xử lý các mục cài đặt khác ở đây
      },
    );
  }

  void _showAccountDetails(BuildContext context) {
    final authProvider = Provider.of<AuthProvider>(context, listen: false);
    final currentUser = authProvider.currentUser;

    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
      ),
      builder: (BuildContext context) {
        return StatefulBuilder(
          builder: (BuildContext context, StateSetter setState) {
            return Container(
              padding: const EdgeInsets.all(20),
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  Stack(
                    alignment: Alignment.center,
                    children: [
                      CustomCircleAvatar(
                        radius: 50,
                        backgroundImage: currentUser?.avatarUrl != null
                            ? NetworkImage(currentUser!.avatarUrl!)
                            : null,
                      ),
                      Positioned(
                        right: 0,
                        bottom: 0,
                        child: CircleAvatar(
                          backgroundColor: AppColors.primary,
                          radius: 18,
                          child: IconButton(
                            icon: const Icon(Icons.camera_alt, size: 18, color: Colors.white),
                            onPressed: () async {
                              final ImagePicker _picker = ImagePicker();
                              final XFile? image = await _picker.pickImage(source: ImageSource.gallery);
                              if (image != null) {
                                // TODO: Implement image upload and update user avatar
                                // authProvider.updateUserAvatar(image.path);
                                setState(() {}); // Refresh the bottom sheet to show new avatar
                              }
                            },
                          ),
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 20),
                  TextField(
                    decoration: InputDecoration(labelText: 'Full Name'),
                    controller: TextEditingController(text: currentUser?.fullName),
                  ),
                  const SizedBox(height: 10),
                  TextField(
                    decoration: InputDecoration(labelText: 'Status'),
                    controller: TextEditingController(text: currentUser?.phoneNumber ?? currentUser?.email),
                  ),
                  const SizedBox(height: 20),
                  ElevatedButton(
                    style: ElevatedButton.styleFrom(
                      backgroundColor: AppColors.primary,
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(8),
                      ),
                      padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 24),
                    ),
                    onPressed: () {
                      // TODO: Implement update user information
                      // authProvider.updateUserInfo(newName, newStatus);
                      Navigator.pop(context);
                    },
                    child: const Text('Update', style: TextStyle(fontSize: 16)),
                  ),
                ],
              ),
            );
          },
        );
      },
    );
  }

  Widget _buildLogoutButton(BuildContext context, AuthProvider authProvider) {
    return ElevatedButton(
      style: ElevatedButton.styleFrom(
        backgroundColor: Colors.red,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(8),
        ),
        padding: const EdgeInsets.symmetric(vertical: 12),
      ),
      onPressed: () {
        showDialog(
          context: context,
          builder: (BuildContext context) {
            return AlertDialog(
              title: const Text('Logout'),
              content: const Text('Are you sure you want to logout?'),
              actions: <Widget>[
                TextButton(
                  child: const Text('Cancel'),
                  onPressed: () {
                    Navigator.of(context).pop();
                  },
                ),
                TextButton(
                  child: const Text('Logout'),
                  onPressed: () {
                    authProvider.logout(context);
                    Navigator.of(context).pop();
                    // TODO: Navigate to login screen or clear navigation stack
                  },
                ),
              ],
            );
          },
        );
      },
      child: const Text(
        'Logout',
        style: TextStyle(fontSize: 16, color: Colors.white),
      ),
    );
  }
}
