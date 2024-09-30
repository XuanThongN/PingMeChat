import 'dart:io';

import 'package:emoji_picker_flutter/emoji_picker_flutter.dart';
import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:image_picker/image_picker.dart';
import 'package:intl/intl.dart';
import 'package:permission_handler/permission_handler.dart';
import 'package:pingmechat_ui/config/theme.dart';
import 'package:pingmechat_ui/domain/models/account.dart';
import 'package:pingmechat_ui/presentation/pages/chat_user_information_page.dart';
import 'package:pingmechat_ui/presentation/widgets/custom_icon.dart';
import 'package:pingmechat_ui/providers/auth_provider.dart';
import 'package:pingmechat_ui/providers/chat_provider.dart';
import 'package:provider/provider.dart';

import '../../domain/models/chat.dart';
import '../../domain/models/message.dart';
import '../widgets/custom_circle_avatar.dart';
import '../widgets/message_widget.dart';

class ChatScreen extends StatefulWidget {
  final String chatId;

  const ChatScreen({super.key, required this.chatId});

  @override
  _ChatScreenState createState() => _ChatScreenState();
}

class _ChatScreenState extends State<ChatScreen> {
  // Khai báo các provider cần thiết
  late ChatProvider _chatProvider;
  late AuthProvider _authProvider;

  final ScrollController _scrollController = ScrollController();
  final TextEditingController _textController = TextEditingController();
  bool _isComposing = false;

  // Biến để lưu ảnh/ video/ audio/ file được chọn
  List<File>? _selectedAttachments = [];

  @override
  void initState() {
    super.initState();

    // Lấy ra các provider cần thiết
    _chatProvider = Provider.of<ChatProvider>(context, listen: false);
    _authProvider = Provider.of<AuthProvider>(context, listen: false);

    // Load messages when the screen is first created
    WidgetsBinding.instance.addPostFrameCallback((_) {
      _chatProvider.loadMessages(widget.chatId, refresh: true);
      _scrollToBottom();
    });

    // Thêm listener để gọi hàm _onScroll khi người dùng cuộn lên trên đầu trang thì load tin nhắn mới
    _scrollController.addListener(_onScroll);
  }

  void _scrollToBottom() {
    _scrollController.animateTo(
      _scrollController.position.maxScrollExtent,
      duration: const Duration(milliseconds: 300),
      curve: Curves.easeOut,
    );
  }

  void _onScroll() {
    if (_scrollController.position.pixels ==
        _scrollController.position.minScrollExtent) {
      _chatProvider.loadMessages(widget.chatId);
    }
  }

  @override
  void dispose() {
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Consumer2<ChatProvider, AuthProvider>(
        builder: (context, chatProvider, authProvider, child) {
      final chat =
          chatProvider.chats.firstWhere((chat) => chat.id == widget.chatId);
      final currentUser = authProvider.currentUser;
      return Scaffold(
        backgroundColor: AppColors.white,
        appBar: _buildAppBar(chat, currentUser),
        body: Column(
          children: [
            Expanded(
              child: Container(
                child: _buildMessageList(currentUser!.id, chat.isGroup),
              ),
            ),
            _buildMessageInput(),
          ],
        ),
      );
    });
  }

  bool _isSameDay(DateTime date1, DateTime date2) {
    return date1.year == date2.year &&
        date1.month == date2.month &&
        date1.day == date2.day;
  }

  String _getFormattedDate(DateTime date) {
    final now = DateTime.now();
    final yesterday = now.subtract(const Duration(days: 1));

    if (_isSameDay(date, now)) {
      return 'Hôm nay';
    } else if (_isSameDay(date, yesterday)) {
      return 'Hôm qua';
    } else {
      return DateFormat('dd/MM/yyyy').format(date);
    }
  }

  Widget _buildMessageList(String currentUserId, bool isGroupChat) {
    return Consumer<ChatProvider>(
      builder: (context, chatProvider, child) {
        final messages = chatProvider.getMessagesForChat(widget.chatId);
        return ListView.builder(
          controller: _scrollController,
          itemCount: messages.length,
          itemBuilder: (context, index) {
            // Hiển thị một widget loading khi đang tải tin nhắn cũ hơn
            if (index == 0) {
              chatProvider.isLoadingMessages(widget.chatId)
                  ? const Center(child: CircularProgressIndicator())
                  : const SizedBox.shrink();
            }
            final message = messages.elementAt(index);
            final showAvatar = _shouldShowAvatar(messages, index);
            final showTimestamp = _shouldShowTimestamp(messages, index);

            // Hiển thị thanh ngang để chia tin nhắn theo ngày
            final previousMessage = index > 0 ? messages[index - 1] : null;
            final showDateDivider = previousMessage == null ||
                !_isSameDay(message.createdDate, previousMessage.createdDate);
            // return _buildMessageItem(message, showAvatar, showTimestamp,
            //     currentUserId, showDateDivider);
            return ChatMessageWidget(
              message: message,
              isLastMessageFromSameSender: showAvatar,
              shouldShowAvatar: showAvatar,
              shouldShowDateDivider: showDateDivider,
              previousMessageDate: previousMessage?.createdDate,
              isGroupMessage: isGroupChat,
              showTimestamp: showTimestamp,
            );
          },
        );
      },
    );
  }

  Widget _buildMessageInput() {
    return Column(
      children: [
        if (_selectedAttachments!.isNotEmpty)
          Container(
            height: 100,
            child: ListView.builder(
              scrollDirection: Axis.horizontal,
              itemCount: _selectedAttachments!.length,
              itemBuilder: (context, index) {
                return Stack(
                  children: [
                    Container(
                      margin: EdgeInsets.all(8),
                      width: 100,
                      height: 100,
                      decoration: BoxDecoration(
                        borderRadius: BorderRadius.circular(8),
                        image: DecorationImage(
                          image: FileImage(_selectedAttachments![index]),
                          fit: BoxFit.cover,
                        ),
                      ),
                    ),
                    Positioned(
                      top: 0,
                      right: 0,
                      child: IconButton(
                        icon: Icon(Icons.close, color: Colors.red),
                        onPressed: () {
                          setState(() {
                            _selectedAttachments!.removeAt(index);
                            if (_selectedAttachments!.isEmpty) {
                              _isComposing = false;
                            }
                          });
                        },
                      ),
                    ),
                  ],
                );
              },
            ),
          ),
        Container(
          padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 8),
          decoration: BoxDecoration(
            color: Colors.white,
            boxShadow: [
              BoxShadow(
                color: Colors.grey.withOpacity(0.1),
                spreadRadius: 1,
                blurRadius: 3,
                offset: const Offset(0, -1),
              ),
            ],
          ),
          child: Row(
            children: [
              IconButton(
                icon: CustomSvgIcon(
                  svgPath: 'assets/icons/Clip, Attachment.svg',
                  color: AppColors.secondary,
                ),
                onPressed: _pickAction,
              ),
              Expanded(
                child: TextField(
                  controller: _textController,
                  onChanged: (text) {
                    setState(() {
                      _isComposing = text.isNotEmpty;
                    });
                  },
                  decoration: InputDecoration(
                    hintText: 'Aa',
                    hintStyle: TextStyle(color: Colors.grey[400]),
                    border: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(25),
                      borderSide: BorderSide.none,
                    ),
                    filled: true,
                    fillColor: AppColors.surface,
                    contentPadding: const EdgeInsets.symmetric(
                        horizontal: 20, vertical: 10),
                    suffixIcon: IconButton(
                      icon: CustomSvgIcon(
                        svgPath: 'assets/icons/files_in_message.svg',
                        color: AppColors.tertiary,
                        size: 24,
                      ),
                      onPressed: _pickIcon,
                    ),
                  ),
                ),
              ),
              if (!_isComposing) ...[
                IconButton(
                  icon: CustomSvgIcon(
                    svgPath: 'assets/icons/camera 01_in_message.svg',
                    color: AppColors.secondary,
                    size: 24,
                  ),
                  onPressed: () {},
                ),
                IconButton(
                  icon: CustomSvgIcon(
                    svgPath: 'assets/icons/microphone_in_message.svg',
                    color: AppColors.secondary,
                    size: 24,
                  ),
                  onPressed: _pickRecording,
                ),
              ],
              if (_isComposing)
                IconButton(
                  icon: CustomSvgIcon(
                    svgPath: 'assets/icons/Send_in_message.svg',
                    color: AppColors.primary,
                  ),
                  onPressed: () => _handleSubmitted(),
                ),
            ],
          ),
        ),
      ],
    );
  }

  PreferredSizeWidget _buildAppBar(Chat? chat, Account? currentUser) {
    final chatName = chat!.isGroup
        ? chat.name
        : chat.userChats
            .firstWhere((userChat) => userChat.user!.id != currentUser?.id)
            .user!
            .fullName;
    final chatAvatarUrl = chat.isGroup
        ? chat.avatarUrl
        : chat.userChats
            .firstWhere((userChat) => userChat.user!.id != currentUser?.id)
            .user!
            .avatarUrl;
    return AppBar(
      backgroundColor: Colors.white,
      elevation: 0,
      leading: IconButton(
        icon: CustomSvgIcon(
          svgPath: 'assets/icons/Back_app_bar.svg',
          color: AppColors.secondary,
          size: 30,
        ),
        onPressed: () {
          Navigator.of(context)
              .pushNamedAndRemoveUntil('/home', (route) => false);
        },
      ),
      title: GestureDetector(
        onTap: () {
          Navigator.push(context,
              MaterialPageRoute(builder: (context) => UserInformationPage()));
        },
        child: Row(
          children: [
            // add status icon here
            Padding(
              padding: const EdgeInsets.only(right: 10),
              child: Stack(
                children: [
                  CustomCircleAvatar(
                    radius: 20,
                    backgroundImage: chat.avatarUrl!.isNotEmpty
                        ? NetworkImage(chat.avatarUrl!)
                        : null,
                    isGroupChat: chat.isGroup,
                  ),
                  Positioned(
                    right: 0,
                    bottom: 0,
                    child: Container(
                      width: 12,
                      height: 12,
                      decoration: BoxDecoration(
                        color: AppColors.primary,
                        shape: BoxShape.circle,
                        border: Border.all(color: Colors.white, width: 2),
                      ),
                    ),
                  ),
                ],
              ),
            ),
            Flexible(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    // Nếu là cuộc trò chuyện nhóm thì hiển thị tên nhóm, nếu là cuộc trò chuyện cá nhân thì hiển thị tên của người (không phải mình) mà mình đang trò chuyện
                    chatName!,
                    style: AppTypography.chatName.copyWith(
                      fontSize: 16,
                    ),
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                  ),
                  const Text(
                    'Active now',
                    style: AppTypography.caption,
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
      actions: [
        // IconButton(
        //   icon: CustomSvgIcon(
        //     svgPath: 'assets/icons/Call_in_message.svg',
        //     color: AppColors.tertiary,
        //     size: 24,
        //   ),
        //   onPressed: () => _initiateCall(chat, false),
        // ),
        // IconButton(
        //   icon: CustomSvgIcon(
        //     svgPath: 'assets/icons/Video_in_message.svg',
        //     color: AppColors.tertiary,
        //     size: 24,
        //   ),
        //   onPressed: () => _initiateCall(chat, true),
        // ),
      ],
    );
  }


  Widget _buildMessageItem(Message message, bool showAvatar, bool showTimestamp,
      String currentUserId, bool showDateDivider) {
    final isMe = message.senderId == currentUserId;
    return Padding(
      padding: const EdgeInsets.only(bottom: 8),
      child: Column(
        children: [
          if (showDateDivider)
            Padding(
              padding: const EdgeInsets.symmetric(vertical: 8.0),
              child: Center(
                child: Text(
                  _getFormattedDate(message.createdDate),
                  style:
                      const TextStyle(color: Color.fromARGB(255, 61, 58, 58)),
                ),
              ),
            ),
          Row(
            mainAxisAlignment:
                isMe ? MainAxisAlignment.end : MainAxisAlignment.start,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              if (!isMe && showAvatar) ...[
                CustomCircleAvatar(
                  backgroundImage:
                      NetworkImage(message.sender!.avatarUrl ?? ''),
                  radius: 16,
                ),
                const SizedBox(width: 8),
              ],
              if (!isMe && !showAvatar) const SizedBox(width: 40),
              Column(
                crossAxisAlignment:
                    isMe ? CrossAxisAlignment.end : CrossAxisAlignment.start,
                children: [
                  Container(
                    constraints: BoxConstraints(
                        maxWidth: MediaQuery.of(context).size.width * 0.7),
                    padding: const EdgeInsets.symmetric(
                        horizontal: 16, vertical: 10),
                    decoration: BoxDecoration(
                      color: isMe ? AppColors.primary_chat : AppColors.surface,
                      borderRadius: BorderRadius.circular(20),
                    ),
                    child: Column(
                      children: [
                        if (message.attachments != null)
                          for (var attachment in message.attachments!)
                            if (attachment.fileType == 'image')
                              Container(
                                width: 200,
                                height: 150,
                                decoration: BoxDecoration(
                                  borderRadius: BorderRadius.circular(12),
                                  image: DecorationImage(
                                    image: NetworkImage(attachment.fileUrl),
                                    fit: BoxFit.cover,
                                  ),
                                ),
                              ),
                        if (message.attachments != null)
                          for (var attachment in message.attachments!)
                            if (attachment.fileType == 'video')
                              SizedBox(
                                width: 200,
                                height: 150,
                                // child:
                                //     VideoPlayerWidget(url: attachment.filePath),
                              ),
                        if (message.content != null)
                          Text(
                            message.content!,
                            style: AppTypography.message.copyWith(
                              color:
                                  isMe ? AppColors.white : AppColors.secondary,
                            ),
                          ),
                      ],
                    ),
                  ),
                  if (showTimestamp)
                    Padding(
                      padding: const EdgeInsets.only(top: 4),
                      child: Text(
                        DateFormat('hh:mm a').format(message.createdDate),
                        style: AppTypography.chatTime,
                      ),
                    ),
                ],
              ),
            ],
          ),
        ],
      ),
    );
  }

  // Hàm thực hiện chức năng gửi tin nhắn
  Future<void> _handleSubmitted() async {
    final text = _textController.text;
    if (text.isNotEmpty || _selectedAttachments!.isNotEmpty) {
      try {
        await _chatProvider.sendMessage(
          chatId: widget.chatId,
          message: text.isNotEmpty ? text : '',
          files: _selectedAttachments!,
        );
        _textController.clear();
        setState(() {
          _isComposing = false;
          _selectedAttachments = [];
        });
        _scrollToBottom();
      } catch (e) {
        // Handle error (e.g., show an error message)
        print('Error sending message: $e');
      }
    }
  }

// Hàm thực hiện chức năng gọi
  // void _initiateCall(Chat? chat, bool isVideo) async {
  //   if (chat == null) return;
  //   final callProvider = Provider.of<CallProvider>(context, listen: false);
  //   await callProvider.initiateCall(chat.id, isVideo);
  //   Navigator.push(
  //     context,
  //     MaterialPageRoute(
  //       builder: (context) => CallPage(
  //         chatId: chat.id,
  //         isVideo: isVideo,
  //         onEndCall: () {
  //           callProvider.endCall();
  //           Navigator.pop(context);
  //         },
  //       ),
  //     ),
  //   );
  // }

  // Hàm chọn ảnh từ thư viện
  Future<void> _pickImage() async {
    // Xin quyền truy cập vào bộ nhớ
    var status = await Permission.storage.request();

    if (status.isGranted) {
      // Nếu quyền được cấp, mở image picker
      final ImagePicker picker = ImagePicker();
      final XFile? image = await picker.pickImage(source: ImageSource.gallery);

      setState(() {
        _selectedAttachments!.add(File(image!.path));
      });
    } else if (status.isDenied) {
      // Nếu quyền bị từ chối, hiển thị thông báo
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
            content: Text(
                'Quyền truy cập bị từ chối. Vui lòng cấp quyền trong cài đặt.')),
      );
    } else if (status.isPermanentlyDenied) {
      // Nếu quyền bị từ chối vĩnh viễn, mở cài đặt ứng dụng
      openAppSettings();
    }
  }

  // Hàm chọn media từ thư viện
  // Hàm chọn media từ thư viện
  void _pickMedia() async {
    await _getPermission();
    var status = await Permission.storage.status;

    if (status.isGranted) {
      final ImagePicker picker = ImagePicker();
      final List<XFile>? mediaFiles = await picker.pickMultiImage();

      if (mediaFiles != null && mediaFiles.isNotEmpty) {
        setState(() {
          _selectedAttachments!
              .addAll(mediaFiles.map((file) => File(file.path)).toList());
          _isComposing = true;
        });
      }
    } else if (status.isDenied) {
      // Nếu quyền bị từ chối, hiển thị thông báo
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text(
              'Quyền truy cập bị từ chối. Vui lòng cấp quyền trong cài đặt.'),
        ),
      );
    } else if (status.isPermanentlyDenied) {
      // Nếu quyền bị từ chối vĩnh viễn, mở cài đặt ứng dụng
      openAppSettings();
    }
  }

  Future<void> _getPermission() async {
    var status = await Permission.storage.status;
    if (!status.isGranted) {
      await Permission.storage.request();
    }
  }

  void _pickIcon() async {
    // Hiển thị một modal bottom sheet để chọn emoji
    final selectedEmoji = await showModalBottomSheet<Emoji>(
      context: context,
      builder: (context) {
        return SizedBox(
          height: 350, // Tăng chiều cao để có thêm không gian hiển thị
          child: EmojiPicker(
            onEmojiSelected: (category, emoji) {
              Navigator.pop(context, emoji);
            },
            config: Config(
              height: 256,
              swapCategoryAndBottomBar: false,
              checkPlatformCompatibility: true,
              emojiSet: defaultEmojiSet,
              emojiTextStyle: GoogleFonts.notoColorEmoji(fontSize: 20),
            ),
          ),
        );
      },
    );

    if (selectedEmoji != null) {
      setState(() {
        _textController.text += selectedEmoji.emoji;
        _isComposing = true;
      });
    }
  }

  void _pickAction() {
    _isComposing = false;
    // Hiển thị một modal bottom sheet để chọn hành động (image, share file, share location, ...) nếu là cuộc trò chuyẹn nhóm thì có thêm chức năng tạo khảo sát
    showModalBottomSheet(
      context: context,
      builder: (context) {
        return SizedBox(
          //Chiều cao của modal bootom sheet vừa đủ để hiển thị các lựa chọn và không bị tràn ra
          // Modal phải có tiêu đề và có nút close để đóng modal
          height: 300,
          child: Column(
            children: [
              // Có một thanh ngang màu xám ở đầu modal dùng để kéo modal
              Container(
                height: 4,
                width: 40,
                margin: const EdgeInsets.symmetric(vertical: 8),
                decoration: BoxDecoration(
                  color: AppColors.tertiary,
                  borderRadius: BorderRadius.circular(2),
                ),
              ),
              Column(
                children: [
                  ListTile(
                    leading: CustomSvgIcon(
                      svgPath: 'assets/icons/media_in_message.svg',
                      color: AppColors.tertiary,
                    ),
                    title: const Text('Media'),
                    subtitle: const Text('Share photos and videos'),
                    onTap: () {
                      Navigator.pop(context);
                      _pickMedia();
                    },
                  ),
                  ListTile(
                    leading: CustomSvgIcon(
                      svgPath: 'assets/icons/doc_in_message.svg',
                      color: AppColors.tertiary,
                    ),
                    title: const Text('File', style: AppTypography.p1),
                    subtitle: const Text('Share files, documents, and more'),
                    onTap: () {
                      Navigator.pop(context);
                      // Implement file picker logic
                    },
                  ),
                  ListTile(
                    leading: CustomSvgIcon(
                      svgPath: 'assets/icons/Pin, Location.svg',
                      color: AppColors.tertiary,
                    ),
                    title: const Text('Location'),
                    subtitle: const Text('Share your location'),
                    onTap: () {
                      Navigator.pop(context);
                      // Implement location picker logic
                    },
                  ),
                ],
              ),
            ],
          ),
        );
      },
    );
  }

  // Thêm hàm pick recording khi thực hiện sẽ hiện thị thanh xử lý ghi âm từ người dùng và biến nó thành file sau đó thì thêm vào _selectedAttachments
  void _pickRecording() {
    // Hiển thị một modal bottom sheet để ghi âm
    showModalBottomSheet(
      context: context,
      builder: (context) {
        return SizedBox(
          height: 200,
          child: Column(
            children: [
              // Có một thanh ngang màu xám ở đầu modal dùng để kéo modal
              Container(
                height: 4,
                width: 40,
                margin: const EdgeInsets.symmetric(vertical: 8),
                decoration: BoxDecoration(
                  color: AppColors.tertiary,
                  borderRadius: BorderRadius.circular(2),
                ),
              ),
              Column(
                children: [
                  ListTile(
                    leading: CustomSvgIcon(
                      svgPath: 'assets/icons/microphone_in_message.svg',
                      color: AppColors.tertiary,
                    ),
                    title: const Text('Record audio'),
                    subtitle: const Text('Record a voice message'),
                    onTap: () {
                      Navigator.pop(context);
                      // Implement audio recording logic
                    },
                  ),
                ],
              ),
            ],
          ),
        );
      },
    );
  }

  // Hiển thị avatar nếu tin nhắn hiện tại không phải của người gửi trước đó
  bool _shouldShowAvatar(List<Message> messages, int index) {
    if (index == 0) return true;
    final currentMessage = messages[index];
    final previousMessage = messages[index - 1];
    return currentMessage.senderId != previousMessage.senderId;
  }

  bool _shouldShowTimestamp(List<Message> messages, int index) {
    if (index == messages.length - 1) return true;
    final currentMessage = messages[index];
    final nextMessage = messages[index + 1];
    return nextMessage.createdDate
            .difference(currentMessage.createdDate)
            .inMinutes >=
        1;
  }
}
