import 'package:flutter/material.dart';
import 'package:pingmechat_ui/config/theme.dart';
import 'package:pingmechat_ui/presentation/widgets/custom_button.dart';
import 'package:pingmechat_ui/providers/auth_provider.dart';
import 'package:pingmechat_ui/providers/contact_provider.dart';
import 'package:provider/provider.dart';
import 'package:pingmechat_ui/providers/chat_provider.dart';

import '../../domain/models/account.dart';
import '../../domain/models/contact.dart';
import '../../domain/models/userchat.dart';
import '../widgets/custom_circle_avatar.dart';

class GroupMembersPage extends StatelessWidget {
  // Tạo biến truyền vào chatId
  final String chatId;
  const GroupMembersPage({Key? key, required this.chatId}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text(
          'Group Members',
          style: TextStyle(
            fontSize: 22,
            fontWeight: FontWeight.bold,
            color: Colors.white,
          ),
        ),
        backgroundColor: AppColors.primary,
        elevation: 0,
        centerTitle: true,
        actions: [
          IconButton(
            icon: Icon(Icons.person_add, color: Colors.white),
            onPressed: () {
              // Xử lý thêm thành viên mới
              _showAddMemberDialog(context);
            },
          ),
        ],
        // shape: RoundedRectangleBorder(
        //   borderRadius: BorderRadius.vertical(
        //     bottom: Radius.circular(20),
        //   ),
        // ),
      ),
      body: Consumer2<ChatProvider, AuthProvider>(
        builder: (context, chatProvider, authProvider, child) {
          final currentUser = authProvider.currentUser;
          final admin = chatProvider.chats
              .firstWhere((chat) => chat.id == chatId)
              .userChats
              .firstWhere((userChat) => userChat.isAdmin);
          final isAdmin = currentUser?.id == admin.user?.id;
          final members = chatProvider.chats
              .firstWhere((chat) => chat.id == chatId)
              .userChats;
          return ListView.builder(
            itemCount: members.length,
            itemBuilder: (context, index) {
              return Dismissible(
                key: Key(members[index]
                    .user!
                    .id), // Sử dụng id của thành viên làm key
                background: isAdmin && members[index].user?.id != admin.user?.id
                    ? Container(
                        color: Colors.red, // Màu nền khi vuốt
                        alignment: Alignment.centerRight,
                        padding: const EdgeInsets.symmetric(horizontal: 20),
                        child: const Icon(
                          Icons.remove_circle,
                          color: Colors.white,
                          size: 30,
                        ),
                      )
                    : null, // Không hiển thị background nếu không phải admin
                direction: isAdmin && members[index].user?.id != admin.user?.id
                    ? DismissDirection
                        .endToStart // Vuốt từ phải sang trái chỉ khi là admin
                    : DismissDirection
                        .none, // Không cho phép vuốt nếu không phải admin
                onDismissed: (direction) async {
                  _removeMember(context, members[index].user!.id);
                },
                child: ListTile(
                  leading: Stack(
                    children: [
                      CustomCircleAvatar(
                        backgroundImage: members[index].user?.avatarUrl != null
                            ? NetworkImage(members[index].user!.avatarUrl!)
                                as ImageProvider<Object>
                            : null,
                      ),
                      if (members[index].user?.id ==
                          admin.user?.id) // Kiểm tra nếu là admin
                        const Positioned(
                          top: -2,
                          right: -2,
                          child: Icon(
                            Icons.star,
                            color: AppColors
                                .primary, // Màu vàng cho biểu tượng ngôi sao
                            size: 20, // Kích thước biểu tượng
                          ),
                        ),
                    ],
                  ),
                  title: Text(members[index].user?.fullName ?? ''),
                  // subtitle:
                  //     Text(members[index].user != null //Giả lập online/offline
                  //         ? 'Online'
                  //         : 'Offline'),
                ),
              );
            },
          );
        },
      ),
    );
  }

  // Widget xóa thành viên
  Widget _buildDismissible(BuildContext context, String userId, bool isAdmin,
      String chatId, Account admin, List<UserChat> members, int index) {
    return Dismissible(
      key: Key(members[index].user!.id), // Sử dụng id của thành viên làm key
      background: isAdmin && members[index].user?.id != admin.id
          ? Container(
              color: Colors.red, // Màu nền khi vuốt
              alignment: Alignment.centerRight,
              padding: const EdgeInsets.symmetric(horizontal: 20),
              child: const Icon(
                Icons.remove_circle,
                color: Colors.white,
                size: 30,
              ),
            )
          : null, // Không hiển thị background nếu không phải admin
      direction: isAdmin && members[index].user?.id != admin.id
          ? DismissDirection
              .endToStart // Vuốt từ phải sang trái chỉ khi là admin
          : DismissDirection.none, // Không cho phép vuốt nếu không phải admin
      onDismissed: (direction) async {
        // Hiển thị hộp thoại xác nhận trước khi xóa
        final confirm = await showDialog<bool>(
          context: context,
          builder: (BuildContext context) {
            return AlertDialog(
              title: const Text('Xác nhận xóa'),
              content: Text(
                  'Bạn có chắc chắn muốn xóa ${members[index].user?.fullName}?'),
              actions: <Widget>[
                TextButton(
                  onPressed: () =>
                      Navigator.of(context).pop(false), // Không xóa
                  child: const Text('Hủy'),
                ),
                TextButton(
                  onPressed: () => Navigator.of(context).pop(true), // Xóa
                  child: const Text('Xóa'),
                ),
              ],
            );
          },
        );

        // Nếu người dùng xác nhận xóa
        if (confirm == true) {
          _removeMember(context, members[index].user!.id);
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(
                content: Text('${members[index].user?.fullName} đã bị xóa')),
          );
        } else {
          // Nếu không xác nhận, không làm gì cả
          // Có thể thêm logic để hoàn tác hành động vuốt nếu cần
          // Để hoàn tác hành động vuốt, bạn có thể gọi setState hoặc sử dụng một biến để theo dõi trạng thái
          // Ví dụ: setState(() { /* cập nhật trạng thái */ });
        }
      },
      child: ListTile(
        leading: Stack(
          children: [
            CustomCircleAvatar(
              backgroundImage: members[index].user?.avatarUrl != null
                  ? NetworkImage(members[index].user!.avatarUrl!)
                      as ImageProvider<Object>
                  : null,
            ),
            if (members[index].user?.id == admin.id) // Kiểm tra nếu là admin
              const Positioned(
                top: -2,
                right: -2,
                child: Icon(
                  Icons.star,
                  color: AppColors.primary, // Màu vàng cho biểu tượng ngôi sao
                  size: 20, // Kích thước biểu tượng
                ),
              ),
          ],
        ),
        title: Text(members[index].user?.fullName ?? ''),
      ),
    );
  }

  void _showAddMemberDialog(BuildContext context) {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      builder: (context) {
        return FractionallySizedBox(
          heightFactor: 0.9,
          child: AddMemberBottomSheet(chatId: chatId),
        );
      },
    );
  }

  void _removeMember(BuildContext context, String userId) async {
    // Xóa thành viên khỏi danh sách
    // Cập nhật trạng thái hoặc gọi API để xóa thành viên
    final chatProvider = Provider.of<ChatProvider>(context, listen: false);
    final result =
        await chatProvider.removeMemberFromCurrentChat(chatId, userId);
    if (result) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Member removed successfully'),
        ),
      );
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Failed to remove member'),
        ),
      );
    }
  }
}

class AddMemberBottomSheet extends StatefulWidget {
  final String chatId;

  const AddMemberBottomSheet({Key? key, required this.chatId})
      : super(key: key);

  @override
  _AddMemberBottomSheetState createState() => _AddMemberBottomSheetState();
}

class _AddMemberBottomSheetState extends State<AddMemberBottomSheet> {
  final TextEditingController _searchController = TextEditingController();
  List<String> _selectedMembers = [];
  String _searchQuery = '';

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: EdgeInsets.only(
        bottom: MediaQuery.of(context).viewInsets.bottom,
        left: 16,
        right: 16,
        top: 30,
      ),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Container(
            width: 40,
            height: 5,
            decoration: BoxDecoration(
              color: Colors.grey[300],
              borderRadius: BorderRadius.circular(10),
            ),
          ),
          SizedBox(height: 16),
          Text(
            'Add Member',
            style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
          ),
          SizedBox(height: 16),
          TextField(
            controller: _searchController,
            decoration: InputDecoration(
              hintText: 'Search members',
              prefixIcon: Icon(Icons.search),
            ),
            onChanged: (value) {
              setState(() {
                _searchQuery = value;
              });
            },
          ),
          SizedBox(height: 16),
          Expanded(
            child: Consumer2<ContactProvider, ChatProvider>(
              builder: (context, contactProvider, chatProvider, child) {
                final currentMembers = chatProvider.chats
                    .firstWhere((chat) => chat.id == widget.chatId)
                    .userChats
                    .map((userChat) => userChat.user!.id)
                    .toList();
                final contacts = contactProvider.contacts
                    .map((contact) => _getContactUser(
                        contact, contactProvider.currentUser.id))
                    .where((contact) =>
                        contact != null &&
                        !currentMembers.contains(contact.id) &&
                        contact.fullName
                            .toLowerCase()
                            .contains(_searchQuery.toLowerCase()))
                    .toList();
                return ListView.builder(
                  itemCount: contacts.length,
                  itemBuilder: (context, index) {
                    var contact = contacts[index];
                    final isSelected = _selectedMembers.contains(contact!.id);
                    return CheckboxListTile(
                      title: Text(contact.fullName),
                      value: isSelected,
                      fillColor: WidgetStateProperty.all(Colors.grey[200]),
                      checkColor: AppColors.primary,
                      onChanged: (bool? value) {
                        setState(() {
                          if (value == true) {
                            _selectedMembers.add(contact.id);
                          } else {
                            _selectedMembers.remove(contact.id);
                          }
                        });
                      },
                    );
                  },
                );
              },
            ),
          ),
          SizedBox(height: 16),
          CustomElevatedButton(
            backgroundColor: AppColors.primary,
            foregroundColor: Colors.white,
            text: 'Submit',
            onPressed: () async {
              // Xử lý thêm thành viên
              await _addMembers(context).then((value) {
                Navigator.of(context).pop();
              });
            },
          ),
          SizedBox(height: 16),
        ],
      ),
    );
  }

  Account? _getContactUser(Contact contact, String currentUserId) {
    return contact.user!.id == currentUserId
        ? contact.contactUser
        : contact.user;
  }

  Future<void> _addMembers(BuildContext context) async {
    // nếu _selectedMembers rỗng thì không thực hiện thêm thành viên
    if (_selectedMembers.isEmpty) {
      return;
    }
    // Thêm thành viên vào nhóm
    final chatProvider = Provider.of<ChatProvider>(context, listen: false);
    final result = await chatProvider.addMembersToCurrentChat(
        widget.chatId, _selectedMembers);
    if (result) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Members added successfully'),
        ),
      );
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Failed to add members'),
        ),
      );
    }
  }
}
