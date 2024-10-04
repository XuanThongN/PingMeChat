import 'dart:io';

import 'package:flutter/material.dart';
import 'package:pingmechat_ui/presentation/pages/login_page.dart';
import 'package:pingmechat_ui/presentation/widgets/custom_button.dart';
import 'package:provider/provider.dart';
import '../../domain/models/account.dart';
import '../../providers/auth_provider.dart';
import '../widgets/custom_circle_avatar.dart';
import '../../config/theme.dart';
import 'package:image_picker/image_picker.dart';

import '../widgets/custom_text_field.dart';

class SettingTab extends StatelessWidget {
  const SettingTab({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      body: SafeArea(
        child: Consumer<AuthProvider>(builder: (context, authProvider, child) {
          final Account? currentUser = authProvider.currentUser;
          if (currentUser == null) {
            // Điều hướng đến trang đăng nhập nếu không có thông tin user
            WidgetsBinding.instance.addPostFrameCallback((_) {
              Navigator.pushReplacementNamed(context, LoginPage.routeName);
            });
            return SizedBox.shrink(); // Return an empty widget if navigating
          }
          return Column(
            children: [
              _buildAppBar(context),
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
                      _buildSettingItem(
                          Icons.key,
                          'Account',
                          'Privacy, security, change number',
                          context,
                          currentUser),
                      const SizedBox(height: 20),
                      _buildLogoutButton(context, authProvider),
                    ],
                  ),
                ),
              ),
            ],
          );
        }),
      ),
    );
  }

  Widget _buildAppBar(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(16),
      child: Row(
        children: [
          IconButton(
            icon: const Icon(Icons.arrow_back, color: Colors.white),
            onPressed: () {
              // Navigate back to the previous screen
              Navigator.pop(context);
            },
          ),
          const Expanded(
            child: Text(
              'Settings',
              style: TextStyle(
                  color: Colors.white,
                  fontSize: 24,
                  fontWeight: FontWeight.bold),
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
          backgroundImage:
              user?.avatarUrl != null ? NetworkImage(user!.avatarUrl!) : null,
        ),
        const SizedBox(width: 16),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                user?.fullName ?? 'Unknown User',
                style:
                    const TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
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

  Widget _buildSettingItem(IconData icon, String title, String subtitle,
      BuildContext context, Account? currentUser) {
    return ListTile(
      leading: CircleAvatar(
        backgroundColor: Colors.grey[200],
        child: Icon(icon, color: AppColors.primary),
      ),
      title: Text(title, style: const TextStyle(fontWeight: FontWeight.bold)),
      subtitle: Text(subtitle, style: TextStyle(color: Colors.grey[600])),
      onTap: () {
        if (title == 'Account') {
          _showProfileBottomSheet(context);
        }
        // Xử lý các mục cài đặt khác ở đây
      },
    );
  }

  Widget _buildLogoutButton(BuildContext context, AuthProvider authProvider) {
    return ElevatedButton(
      style: ElevatedButton.styleFrom(
        backgroundColor: Colors.red,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(12),
        ),
        padding: const EdgeInsets.symmetric(vertical: 16),
        elevation: 3,
      ),
      onPressed: () {
        showDialog(
          context: context,
          builder: (BuildContext context) {
            return AlertDialog(
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(16),
              ),
              title: const Text('Logout',
                  style: TextStyle(fontWeight: FontWeight.bold)),
              content: const Text('Are you sure you want to logout?',
                  style: TextStyle(fontSize: 16)),
              actions: <Widget>[
                TextButton(
                  child: const Text('Cancel',
                      style: TextStyle(color: Colors.grey)),
                  onPressed: () => Navigator.of(context).pop(),
                ),
                ElevatedButton(
                  style: ElevatedButton.styleFrom(
                    backgroundColor: Colors.red,
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(8),
                    ),
                  ),
                  child: const Text('Logout',
                      style: TextStyle(color: Colors.white)),
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
        style: TextStyle(
            fontSize: 18, fontWeight: FontWeight.bold, color: Colors.white),
      ),
    );
  }

  void _showProfileBottomSheet(BuildContext context) {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      builder: (context) => ProfileBottomSheet(),
    );
  }
}

class ProfileBottomSheet extends StatefulWidget {
  @override
  _ProfileBottomSheetState createState() => _ProfileBottomSheetState();
}

class _ProfileBottomSheetState extends State<ProfileBottomSheet> {
  final _formKey = GlobalKey<FormState>();
  final _nameController = TextEditingController();
  final _phoneController = TextEditingController();
  File? _avatarFile;
  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    _loadUserData();
  }

  void _loadUserData() {
    final user = Provider.of<AuthProvider>(context, listen: false).currentUser;
    if (user != null) {
      _nameController.text = user.fullName;
      _phoneController.text = user.phoneNumber ?? '';
    }
  }

  @override
  Widget build(BuildContext context) {
    final user = Provider.of<AuthProvider>(context).currentUser;

    return Padding(
      padding: EdgeInsets.only(
        bottom: MediaQuery.of(context).viewInsets.bottom,
      ),
      child: Container(
        padding: EdgeInsets.all(16),
        child: Form(
          key: _formKey,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.center,
            children: [
              _buildAvatarSection(user?.avatarUrl),
              SizedBox(height: 24),
              CustomTextField(
                label: 'Full Name',
                controller: _nameController,
                validator: (value) =>
                    value!.isEmpty ? 'Please enter your name' : null,
              ),
              SizedBox(height: 16),
              CustomTextField(
                label: 'Phone Number',
                controller: _phoneController,
                validator: (value) =>
                    value!.isEmpty ? 'Please enter your phone number' : null,
                keyboardType: TextInputType.phone,
              ),
              SizedBox(height: 24),
              CustomElevatedButton(
                text: 'Update Profile',
                onPressed: _isLoading ? () {} : _handleUpdateProfile,
                backgroundColor: AppColors.primary,
                foregroundColor: Colors.white,
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildAvatarSection(String? avatarUrl) {
    return GestureDetector(
      onTap: _pickImage,
      child: CircleAvatar(
        radius: 50,
        backgroundImage: _avatarFile != null
            ? FileImage(_avatarFile!)
            : (avatarUrl != null ? NetworkImage(avatarUrl) : null)
                as ImageProvider?,
        child: _avatarFile == null && avatarUrl == null
            ? Icon(Icons.camera_alt, size: 50, color: Colors.white)
            : null,
      ),
    );
  }

  Future<void> _pickImage() async {
    final picker = ImagePicker();
    final pickedFile = await picker.pickImage(source: ImageSource.gallery);

    if (pickedFile != null) {
      setState(() {
        _avatarFile = File(pickedFile.path);
      });
      // Upload avatar to server
      final authProvider = Provider.of<AuthProvider>(context, listen: false);
      final result = await authProvider.updateAvatar(_avatarFile!);
      if (result) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Avatar updated successfully')),
        );
      } else {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Failed to update avatar')),
        );
      }
    }
  }

  Future<void> _handleUpdateProfile() async {
    if (_formKey.currentState!.validate()) {
      setState(() => _isLoading = true);
      final authProvider = Provider.of<AuthProvider>(context, listen: false);

      try {
        // Update user info
        final userInfoUpdated = await authProvider.updateUserInfo({
          'fullName': _nameController.text.trim(),
          'phoneNumber': _phoneController.text.trim(),
        });

        if (userInfoUpdated) {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(content: Text('Profile updated successfully')),
          );
          Navigator.of(context).pop(); // Close the bottom sheet
        } else {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(content: Text('Failed to update profile')),
          );
          throw Exception('Failed to update profile');
        }
      } catch (error) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('An error occurred: $error')),
        );
      } finally {
        setState(() => _isLoading = false);
      }
    }
  }
}
