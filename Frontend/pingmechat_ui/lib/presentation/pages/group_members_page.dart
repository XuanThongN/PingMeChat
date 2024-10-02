import 'package:flutter/material.dart';
import 'package:pingmechat_ui/config/theme.dart';
import 'package:pingmechat_ui/presentation/widgets/custom_button.dart';
import 'package:pingmechat_ui/providers/contact_provider.dart';
import 'package:provider/provider.dart';
import 'package:pingmechat_ui/providers/chat_provider.dart';

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
      body: Consumer<ChatProvider>(
        builder: (context, chatProvider, child) {
          final members = chatProvider.chats
              .firstWhere((chat) => chat.id == chatId)
              .userChats;
          return ListView.builder(
            itemCount: members.length,
            itemBuilder: (context, index) {
              return ListTile(
                title: Text(members[index].user?.fullName ?? ''),
                trailing: IconButton(
                  icon: Icon(Icons.remove_circle),
                  onPressed: () {
                    // Xử lý xóa thành viên
                    _removeMember(context, members[index].user!.fullName);
                  },
                ),
              );
            },
          );
        },
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

  void _removeMember(BuildContext context, String member) {
    // Xóa thành viên khỏi danh sách
    // Cập nhật trạng thái hoặc gọi API để xóa thành viên
    final chatProvider = Provider.of<ChatProvider>(context, listen: false);
    // chatProvider.removeMemberFromCurrentChat(member);
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
                // final currentMembers = chatProvider.chats
                //     .firstWhere((chat) => chat.id == widget.chatId)
                //     .userChats
                //     .map((userChat) => userChat.user!.id)
                //     .toList();
                final contacts = contactProvider.contacts
                    // .where((contact) =>
                    //     contact.user!.id != contactProvider.currentUser.id &&
                        // !currentMembers.contains(contact.user!.id) &&
                        // contact.user!.fullName
                        //     .toLowerCase()
                        //     .contains(_searchQuery.toLowerCase()))
                    .toList();
                return ListView.builder(
                  itemCount: contacts.length,
                  itemBuilder: (context, index) {
                    var contact = contacts[index];
                    final isSelected =
                        _selectedMembers.contains(contact.user!.id);
                    return CheckboxListTile(
                      title: Text(contact.user!.fullName),
                      value: isSelected,
                      fillColor: WidgetStateProperty.all(Colors.grey[200]),
                      checkColor: AppColors.primary,
                      onChanged: (bool? value) {
                        setState(() {
                          if (value == true) {
                            _selectedMembers.add(contact.user!.id);
                          } else {
                            _selectedMembers.remove(contact.user!.id);
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
            text: String.fromCharCode('Submit'.runes.first),
            onPressed: () {
              // Xử lý thêm thành viên
              _addMembers(context);
              Navigator.of(context).pop();
            },
            child: Text('Submit'),
          ),
          SizedBox(height: 16),
        ],
      ),
    );
  }

  void _addMembers(BuildContext context) {
    // nếu _selectedMembers rỗng thì không thực hiện thêm thành viên
    if (_selectedMembers.isEmpty) {
      return;
    }
    // Thêm thành viên vào nhóm
    final chatProvider = Provider.of<ChatProvider>(context, listen: false);
    chatProvider.addMembersToCurrentChat(widget.chatId, _selectedMembers);
  }
}
