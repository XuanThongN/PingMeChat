import 'dart:async';
import 'dart:io';

import 'package:emoji_picker_flutter/emoji_picker_flutter.dart';
import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:image_picker/image_picker.dart';
import 'package:intl/intl.dart';
import 'package:permission_handler/permission_handler.dart';
import 'package:pingmechat_ui/config/theme.dart';
import 'package:pingmechat_ui/domain/models/account.dart';
import 'package:pingmechat_ui/presentation/pages/chat_page_extension.dart';
import 'package:pingmechat_ui/presentation/pages/chat_user_information_page.dart';
import 'package:pingmechat_ui/presentation/widgets/custom_icon.dart';
import 'package:pingmechat_ui/providers/auth_provider.dart';
import 'package:pingmechat_ui/providers/chat_provider.dart';
import 'package:pingmechat_ui/providers/contact_provider.dart';
import 'package:provider/provider.dart';

import '../../domain/models/chat.dart';
import '../../domain/models/message.dart';
import '../widgets/custom_circle_avatar.dart';
import '../widgets/message_reader_widget.dart';
import '../widgets/message_widget.dart';
import '../widgets/typing_indicator.dart';

class ChatPage extends StatefulWidget {
  final String chatId;

  const ChatPage({super.key, required this.chatId});

  @override
  _ChatPageState createState() => _ChatPageState();
}

class _ChatPageState extends State<ChatPage> {
  // Khai báo các provider cần thiết
  late ChatProvider _chatProvider;
  late AuthProvider _authProvider;

  final ScrollController _scrollController = ScrollController();
  final TextEditingController _textController = TextEditingController();
  bool _isComposing = false;
  bool _needsScrollDown =
      false; // Theo dõi việc có cần cuộn xuống cuối trang hay không

// Biến để hiển thị button cuộn xuống cuối trang
  bool _showScrollToBottomButton = false;

  // Biến để lưu ảnh/ video/ audio/ file được chọn
  List<File>? _selectedAttachments = [];

  // Biến để đếm thời gian gõ tin nhắn
  Timer? _typingTimer;
  static const _typingDuration = Duration(milliseconds: 3000);

  late FocusNode _focusNode;
  bool _isKeyboardVisible = false;
  @override
  void initState() {
    super.initState();

    _chatProvider = Provider.of<ChatProvider>(context, listen: false);
    _authProvider = Provider.of<AuthProvider>(context, listen: false);

    WidgetsBinding.instance.addPostFrameCallback((_) {
      _chatProvider.loadMessages(widget.chatId, refresh: true).then((_) {
        setState(() {
          _needsScrollDown = true;
        });
      });
    });

    _scrollController.addListener(_onScroll);

    // Xử lý khi người dùng bắt đầu gõ tin nhắn
    _textController.addListener(_handleTyping);

    // Set up focus node
    _focusNode = FocusNode();
    _focusNode.addListener(_onFocusChange);
  }

  // Xử lý khi focus thay đổi
  void _onFocusChange() {
    setState(() {
      _isKeyboardVisible = _focusNode.hasFocus;
      if (_isKeyboardVisible) {
        _needsScrollDown = true;
      }
    });
  }

  void _handleTyping() {
    final chatProvider = Provider.of<ChatProvider>(context, listen: false);
    if (_textController.text.isNotEmpty) {
      chatProvider.sendTypingStatus(widget.chatId, true);
      _resetTypingTimer();
    } else {
      chatProvider.sendTypingStatus(widget.chatId, false);
    }
  }

  void _resetTypingTimer() {
    _typingTimer?.cancel();
    _typingTimer = Timer(_typingDuration, () {
      final chatProvider = Provider.of<ChatProvider>(context, listen: false);
      chatProvider.sendTypingStatus(widget.chatId, false);
    });
  }

  // Future<void> _loadMessagesAndScroll() async {
  //   await _chatProvider.loadMessages(widget.chatId, refresh: true);
  //   WidgetsBinding.instance.addPostFrameCallback((_) {
  //     _scrollToBottom();
  //   });
  // }

  // void _scrollToBottom() {
  //   if (_scrollController.hasClients) {
  //     _scrollController.animateTo(
  //       _scrollController.position.maxScrollExtent,
  //       duration: const Duration(milliseconds: 200),
  //       curve: Curves.easeOut,
  //     );
  //     setState(() {
  //       _showScrollToBottomButton = false;
  //     });
  //   }
  // }
  void _scrollToBottom() {
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (_scrollController.hasClients) {
        Future.delayed(Duration(milliseconds: 100), () {
          _scrollController.animateTo(
            _scrollController.position.maxScrollExtent,
            duration: const Duration(milliseconds: 300),
            curve: Curves.easeOut,
          );
        });
        setState(() {
          _showScrollToBottomButton = false;
        });
      }
    });
  }

  void _onScroll() {
    if (_scrollController.position.pixels ==
        _scrollController.position.minScrollExtent) {
      _chatProvider.loadMessages(widget.chatId);
      setState(() {
        _showScrollToBottomButton = true;
      });
    } else if (_scrollController.position.pixels ==
        _scrollController.position.maxScrollExtent) {
      setState(() {
        _showScrollToBottomButton = false;
      });
    }
  }

  @override
  void dispose() {
    _focusNode.removeListener(_onFocusChange);
    _focusNode.dispose();
    _scrollController.dispose();
    _textController.dispose();
    _typingTimer?.cancel();
    _textController.removeListener(_handleTyping);
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    if (_needsScrollDown) {
      WidgetsBinding.instance.addPostFrameCallback((_) {
        _scrollToBottom();
        _needsScrollDown = false;
      });
    }
    return Consumer2<ChatProvider, AuthProvider>(
        builder: (context, chatProvider, authProvider, child) {
      final chat =
          chatProvider.chats.firstWhere((chat) => chat.id == widget.chatId);
      final currentUser = authProvider.currentUser;
      return Scaffold(
        backgroundColor: AppColors.white,
        appBar: _buildAppBar(chat, currentUser),
        body: Stack(
          children: [
            Column(
              children: [
                Expanded(
                  child: _buildMessageList(
                      authProvider.currentUser!.id, chat.isGroup),
                ),
                if (chatProvider.isUserTyping(widget.chatId))
                  _buildTypingIndicator(chat),
                _buildMessageInput(),
              ],
            ),
            if (_showScrollToBottomButton)
              Positioned(
                right: 16,
                bottom:
                    80, // Adjust this value to position the button above the message composer
                child: FloatingActionButton(
                  mini: true,
                  backgroundColor: AppColors.primary.withOpacity(0.8),
                  child: const Icon(Icons.arrow_downward, color: Colors.white),
                  onPressed: _scrollToBottom,
                ),
              ),
          ],
        ),
      );
    });
  }

  Widget _buildTypingIndicator(Chat chat) {
    return Consumer<ChatProvider>(
      builder: (context, chatProvider, child) {
        final typingUserId = chatProvider.getTypingUser(widget.chatId);
        if (typingUserId == null) return const SizedBox.shrink();

        final typingUser = chat.userChats
            .firstWhere((userChat) => userChat.user!.id == typingUserId)
            .user;

        return Container(
          padding: const EdgeInsets.symmetric(vertical: 8, horizontal: 16),
          child: Row(
            children: [
              CustomCircleAvatar(
                radius: 10,
                backgroundImage: typingUser?.avatarUrl != null
                    ? NetworkImage(typingUser!.avatarUrl!)
                    : null,
              ),
              const SizedBox(width: 8),
              const TypingIndicator(
                bubbleColor: AppColors.primary,
                flashingCircleDarkColor: AppColors.primary,
                flashingCircleBrightColor: Colors.white,
              ),
            ],
          ),
        );
      },
    );
  }

  Widget _buildMessageList(String currentUserId, bool isGroupChat) {
    return Consumer<ChatProvider>(
      builder: (context, chatProvider, child) {
        final messages = chatProvider.getMessagesForChat(widget.chatId);
        return ListView.builder(
          controller: _scrollController,
          itemCount: messages.length +
              1, // Thêm 1 vào itemCount để có chỗ cho loading indicator
          itemBuilder: (context, index) {
            // Hiển thị loading indicator ở đầu danh sách
            if (index == 0) {
              return chatProvider.isLoadingMessages(widget.chatId)
                  ? const Center(
                      child: Padding(
                      padding: EdgeInsets.all(8.0),
                      child: SizedBox(
                        width: 20,
                        height: 20,
                        child: CircularProgressIndicator(
                          color: AppColors.primary,
                          strokeWidth: 2,
                          backgroundColor: AppColors.surface,
                        ),
                      ),
                    ))
                  : const SizedBox.shrink();
            }

            // Điều chỉnh index để lấy tin nhắn đúng
            final messageIndex = index - 1;
            final message = messages.elementAt(messageIndex);
            final chat = chatProvider.chats.firstWhere((chat) => chat.id == widget.chatId);
            final showAvatar =
                ChatPageHelper.shouldShowAvatar(messages, messageIndex);
            final showTimestamp =
                ChatPageHelper.shouldShowTimestamp(messages, messageIndex);
            final isLastMessage = messageIndex == 0;
            // Hiển thị thanh ngang để chia tin nhắn theo ngày
            final previousMessage =
                messageIndex > 0 ? messages[messageIndex - 1] : null;
            final showDateDivider = previousMessage == null ||
                !ChatPageHelper.isSameDay(
                    message.createdDate, previousMessage.createdDate);

            // return ChatMessageWidget(
            //   message: message,
            //   isLastMessageFromSameSender: showAvatar,
            //   shouldShowAvatar: showAvatar,
            //   shouldShowDateDivider: showDateDivider,
            //   previousMessageDate: previousMessage?.createdDate,
            //   isGroupMessage: isGroupChat,
            //   showTimestamp: showTimestamp,
            // );

            return Column(
              children: [
                ChatMessageWidget(
                  message: message,
                  isLastMessageFromSameSender: showAvatar,
                  shouldShowAvatar: showAvatar,
                  shouldShowDateDivider: showDateDivider,
                  previousMessageDate: previousMessage?.createdDate,
                  isGroupMessage: isGroupChat,
                  showTimestamp: showTimestamp,
                ),
                if (isLastMessage)
                  buildMessageReadersWidget(message, chat, currentUserId),
              ],
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
                  size: 24,
                  svgPath: 'assets/icons/media_in_message.svg',
                  color: AppColors.secondary,
                ),
                onPressed: _pickImage,
              ),
              Expanded(
                child: TextField(
                  focusNode: _focusNode,
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
      leadingWidth: 40,
      leading: IconButton(
        icon: const Icon(Icons.arrow_back, color: AppColors.secondary),
        onPressed: () => Navigator.of(context).pop(),
      ),
      title: GestureDetector(
        onTap: () {
          Navigator.push(
              context,
              MaterialPageRoute(
                  builder: (context) => UserInformationPage(chatId: chat.id)));
        },
        child: Row(
          children: [
            Hero(
              tag: 'profileImage${chat.id}',
              child: Stack(
                children: [
                  CustomCircleAvatar(
                    radius: 20,
                    backgroundImage:
                        chatAvatarUrl != null && chatAvatarUrl.isNotEmpty
                            ? NetworkImage(chatAvatarUrl)
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
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    chatName ?? '',
                    style: AppTypography.subH3.copyWith(
                      fontWeight: FontWeight.bold,
                      color: AppColors.secondary,
                    ),
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                  ),
                  Text(
                    'Active now',
                    style: AppTypography.caption.copyWith(
                      color: AppColors.secondary,
                    ),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
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
                  ChatPageHelper.getFormattedDate(message.createdDate),
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
        final attachments = _selectedAttachments;

        // Xóa attachments và reset trạng thái composing
        setState(() {
          _isComposing = false;
          _selectedAttachments = [];
          _needsScrollDown = true; // Đánh dấu cần scroll xuống cuối
        });
        _textController.clear();

        // Gửi tin nhắn
        await _chatProvider.sendMessage(
          chatId: widget.chatId,
          message: text.isNotEmpty ? text : '',
          files: attachments!,
        );
      } catch (e) {
        // Handle error (e.g., show an error message)
        print('Error sending message: $e');
      }
    }
  }

  Future<void> _pickImage() async {
    // Xin quyền truy cập vào bộ nhớ
    final picker = ImagePicker();
    final pickedFile = await picker.pickImage(source: ImageSource.gallery);

    if (pickedFile != null) {
      setState(() {
        _selectedAttachments!.add(File(pickedFile.path));
        _isComposing = true;
      });
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
}
