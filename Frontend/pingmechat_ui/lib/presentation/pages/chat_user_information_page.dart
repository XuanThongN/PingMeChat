import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:pingmechat_ui/presentation/widgets/custom_circle_avatar.dart';
import 'package:pingmechat_ui/providers/chat_provider.dart';
import 'package:provider/provider.dart';

import '../../domain/models/chat.dart';
import '../../providers/auth_provider.dart';
import 'chat_media_links_document_page.dart';
import 'group_members_page.dart';

class UserInformationPage extends StatefulWidget {
  final String chatId;
  const UserInformationPage({super.key, required this.chatId});

  @override
  _UserInformationPageState createState() => _UserInformationPageState();
}

class _UserInformationPageState extends State<UserInformationPage> {
  @override
  Widget build(BuildContext context) {
    // provider để lấy thông tin chi tiết của chat
    final chatProvider = Provider.of<ChatProvider>(context);
    final authProvider = Provider.of<AuthProvider>(context);
    final chat = chatProvider.chats.firstWhere((c) => c.id == widget.chatId);
    if (chat.id.isEmpty) {
      return const Scaffold(
        body: Center(
          child: Text('Chat not found'),
        ),
      );
    } // Chat được chọn
    final otherUser = chat.userChats // Người dùng còn lại trong chat
        .firstWhere((uc) => uc.userId != authProvider.currentUser!.id)
        .user;
    final chatAvatar = chat.isGroup
        ? chat.avatarUrl!
        : otherUser!.avatarUrl; // Ảnh đại diện của chat
    final chatName =
        chat.isGroup ? chat.name! : otherUser!.fullName; // Tên chat
    final additionalInfo = chat.isGroup // Thông tin bổ sung
        ? '${chat.userChats.length} members'
        : otherUser!.phoneNumber ?? otherUser!.email;
    final attachmentsNumber = chat.messages! // Số lượng tệp đính kèm trong chat
        .where((m) => m.attachments != null)
        .expand((m) => m.attachments!)
        .length;
    return Scaffold(
      backgroundColor: Colors.white,
      appBar: AppBar(
        elevation: 0,
        backgroundColor: Colors.white,
        leading: IconButton(
          icon: const Icon(Icons.arrow_back, color: Colors.black),
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
                  CustomCircleAvatar(
                      radius: 50,
                      isGroupChat: chat.isGroup,
                      backgroundImage: chatAvatar != null
                          ? CachedNetworkImageProvider(chatAvatar)
                          : null),
                  Text(
                    chatName,
                    style: const TextStyle(
                        fontSize: 20, fontWeight: FontWeight.bold),
                  ),
                  Text(
                    additionalInfo,
                    style: const TextStyle(color: Colors.grey),
                  ),
                  !chat.isGroup
                      ? IconButton(
                          icon: const Icon(Icons.copy, color: Colors.grey),
                          onPressed: () {
                            Clipboard.setData(
                                ClipboardData(text: additionalInfo));
                            ScaffoldMessenger.of(context).showSnackBar(
                              SnackBar(content: Text('Copied to clipboard')),
                            );
                          },
                        )
                      : const SizedBox(),
                ],
              ),
            ),
            const Divider(),
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 20.0),
              child: Column(
                children: [
                  SettingsOption(
                    icon: Icons.link,
                    title: 'Media, Links & Documents',
                    trailing: attachmentsNumber > 0
                        ? attachmentsNumber.toString()
                        : null,
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
                  chat.isGroup
                      ? SettingsOption(
                          icon: Icons.group,
                          title: 'Members',
                          onTap: () {
                            Navigator.push(
                              context,
                              MaterialPageRoute(
                                builder: (context) =>
                                    GroupMembersPage(chatId: chat.id),
                              ),
                            );
                          },
                        )
                      : const SizedBox(),
                  chat.isGroup // Nếu là nhóm thì hiển thị nút Leave Group
                      ? SettingsOption(
                          icon: Icons.exit_to_app,
                          title: 'Leave Group',
                          onTap: () {},
                        )
                      : ListTile(
                          leading: const Icon(Icons.block, color: Colors.red),
                          title: const Text('Block',
                              style: TextStyle(color: Colors.red)),
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

  const SettingsOption(
      {super.key,
      required this.icon,
      required this.title,
      this.trailing,
      required this.onTap});

  @override
  Widget build(BuildContext context) {
    return ListTile(
      leading: Icon(icon, color: Colors.black),
      title: Text(title),
      trailing: trailing != null
          ? Text(trailing!, style: const TextStyle(color: Colors.grey))
          : const Icon(Icons.arrow_forward_ios, size: 16),
      onTap: onTap,
    );
  }
}

class SettingsSwitchOption extends StatelessWidget {
  final IconData icon;
  final String title;
  final bool value;
  final ValueChanged<bool> onChanged;

  const SettingsSwitchOption(
      {super.key,
      required this.icon,
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

  const SettingsColorOption(
      {super.key,
      required this.icon,
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
