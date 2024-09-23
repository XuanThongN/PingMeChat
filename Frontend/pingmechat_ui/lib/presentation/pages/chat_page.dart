import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:pingmechat_ui/config/theme.dart';
import 'package:pingmechat_ui/domain/models/account.dart';
import 'package:pingmechat_ui/presentation/pages/call_group_page.dart';
import 'package:pingmechat_ui/presentation/pages/chat_user_information_page.dart';
import 'package:pingmechat_ui/presentation/pages/video_call_page.dart';
import 'package:pingmechat_ui/presentation/widgets/custom_icon.dart';
import 'package:pingmechat_ui/providers/auth_provider.dart';
import 'package:pingmechat_ui/providers/chat_provider.dart';
import 'package:provider/provider.dart';
import 'package:video_player/video_player.dart';

import '../../domain/models/chat.dart';
import '../../domain/models/message.dart';
import '../widgets/custom_circle_avatar.dart';

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
  String? _selectedImage;
  String? _selectedVideo;

  // late AudioPlayer _audioPlayer =
  //     AudioPlayer(); // Dùng để làm gì trong đây? // Để phát audio từ URL
  // final PlayerController _playerController = PlayerController();

  @override
  void initState() {
    super.initState();

    // Lấy ra các provider cần thiết
    _chatProvider = Provider.of<ChatProvider>(context, listen: false);
    _authProvider = Provider.of<AuthProvider>(context, listen: false);
    // WidgetsBinding.instance.addPostFrameCallback((_) => _scrollToBottom());
    // _audioPlayer = AudioPlayer();
    // _audioPlayer.setReleaseMode(ReleaseMode.stop);

    // Load messages when the screen is first created
    WidgetsBinding.instance.addPostFrameCallback((_) {
      _chatProvider.loadMessages(widget.chatId, refresh: true);
    });
  }

  void _scrollToBottom() {
    _scrollController.animateTo(
      _scrollController.position.maxScrollExtent,
      duration: Duration(milliseconds: 300),
      curve: Curves.easeOut,
    );
  }

  @override
  void dispose() {
    // _audioPlayer.dispose();
    // _playerController.dispose();
    super.dispose();
  }

  void _playPauseAudio(String url) async {
    // try {
    //   if (_audioPlayer.state == audioplayers.PlayerState.playing) {
    //     await _audioPlayer.pause();
    //   } else {
    //     // await _audioPlayer.play(UrlSource(url));
    //   }
    // } catch (e) {
    //   print('Error playing audio: $e');
    // }
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
              child: _buildMessageList(currentUser!.id),
            ),
            _buildMessageInput(),
          ],
        ),
      );
    });
  }

  Widget _buildMessageList(String currentUserId) {
    return Consumer<ChatProvider>(
      builder: (context, chatProvider, child) {
        final messages = chatProvider.getMessagesForChat(widget.chatId);
        return ListView.builder(
          controller: _scrollController,
          itemCount: messages.length,
          itemBuilder: (context, index) {
            final message = messages.elementAt(index);
            final showAvatar = _shouldShowAvatar(messages, index);
            final showTimestamp = _shouldShowTimestamp(messages, index);
            return _buildMessageItem(
              message,
              showAvatar,
              showTimestamp,
              currentUserId
            );
          },
        );
      },
    );
  }

  Widget _buildMessageComposer() {
    return Container(
      padding: EdgeInsets.symmetric(horizontal: 8.0),
      decoration: BoxDecoration(color: Theme.of(context).cardColor),
      child: Row(
        children: <Widget>[
          Expanded(
            child: TextField(
              controller: _textController,
              onChanged: (text) {
                setState(() {
                  _isComposing = text.isNotEmpty;
                });
              },
              onSubmitted: _isComposing ? _handleSubmitted : (String a) {},
              decoration: InputDecoration.collapsed(hintText: "Send a message"),
            ),
          ),
          IconButton(
            icon: Icon(Icons.send),
            onPressed: _isComposing
                ? () => _handleSubmitted(_textController.text)
                : null,
          ),
        ],
      ),
    );
  }

  Widget _buildMessageInput() {
    return Container(
      padding: EdgeInsets.symmetric(horizontal: 8, vertical: 8),
      decoration: BoxDecoration(
        color: Colors.white,
        boxShadow: [
          BoxShadow(
            color: Colors.grey.withOpacity(0.1),
            spreadRadius: 1,
            blurRadius: 3,
            offset: Offset(0, -1),
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
                // Dùng để làm gì trong đây?  // Để tạo màu nền cho TextField
                contentPadding:
                    EdgeInsets.symmetric(horizontal: 20, vertical: 10),
                suffixIcon: IconButton(
                  icon: CustomSvgIcon(
                    svgPath: 'assets/icons/files_in_message.svg',
                    color: AppColors.tertiary,
                    size: 24,
                  ),
                  onPressed: _pickSticker,
                ),
              ),
            ),
          ),
          if (!_isComposing) ...[
            // Dùng để làm gì trong đây? // Hiển thị các icon khi không có nội dung trong TextField
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
              onPressed: _pickVideo,
            ),
          ],
          if (_isComposing)
            IconButton(
              icon: CustomSvgIcon(
                svgPath: 'assets/icons/Send_in_message.svg',
                color: AppColors.primary,
              ),
              onPressed: () => _handleSubmitted(_textController.text),
            ),
        ],
      ),
    );
  }

  PreferredSizeWidget _buildAppBar(Chat? chat, Account? currentUser) {
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
          Navigator.pop(context);
        },
      ),
      title: GestureDetector(
        onTap: () {
          Navigator.push(context,
              MaterialPageRoute(builder: (context) => UserInformationPage()));
        },
        // child: Row(
        //   children: [
        //     // add status icon here
        //     Stack(
        //       children: [
        //         CustomCircleAvatar(
        //           backgroundImage:
        //               chat!.isGroup
        //               ? NetworkImage(chat.avatarUrl!)
        //               : NetworkImage(
        //                   chat.userChats
        //                       .firstWhere(
        //                           (userChat) => userChat.user!.id != currentUser?.id)
        //                       .user!
        //                       .avatarUrl!,
        //                 ),
        //           radius: 20,
        //         ),
        //         Positioned(
        //           right: 0,
        //           bottom: 0,
        //           child: Container(
        //             width: 12,
        //             height: 12,
        //             decoration: BoxDecoration(
        //               color: AppColors.primary,
        //               shape: BoxShape.circle,
        //               border: Border.all(color: Colors.white, width: 2),
        //             ),
        //           ),
        //         ),
        //       ],
        //     ),
        //     SizedBox(width: 12),
        //     Column(
        //       crossAxisAlignment: CrossAxisAlignment.start,
        //       children: [
        //         Text(
        //           // Nếu là cuộc trò chuyện nhóm thì hiển thị tên nhóm, nếu là cuộc trò chuyện cá nhân thì hiển thị tên của người (không phải mình) mà mình đang trò chuyện
        //           chat!.isGroup
        //               ? chat.name!
        //               : chat.userChats!
        //                   .firstWhere(
        //                       (userChat) => userChat.user!.id != currentUser?.id)
        //                   .user!
        //                   .fullName,
        //           style: AppTypography.chatName.copyWith(
        //             fontSize: 16,
        //           ),
        //         ),
        //         Text(
        //           'Active now',
        //           style: AppTypography.caption,
        //         ),
        //       ],
        //     ),
        //   ],
        // ),
      ),
      actions: [
        IconButton(
          icon: CustomSvgIcon(
            svgPath: 'assets/icons/Call_in_message.svg',
            color: AppColors.tertiary,
            size: 24,
          ),
          onPressed: () {
            Navigator.push(context,
                MaterialPageRoute(builder: (context) => GroupCallPage()));
            // MaterialPageRoute(builder: (context) => IncomingCallPage()));
            // context, MaterialPageRoute(builder: (context) => CallScreen()));
          },
        ),
        IconButton(
          icon: CustomSvgIcon(
            svgPath: 'assets/icons/Video_in_message.svg',
            color: AppColors.tertiary,
            size: 24,
          ),
          onPressed: () {
            Navigator.push(context,
                MaterialPageRoute(builder: (context) => VideoCallPage()));
          },
        ),
      ],
    );
  }

  Widget _buildMessageItem(
      Message message, bool showAvatar, bool showTimestamp, String currentUserId) {
    final isMe = message.senderId == currentUserId;
    return Padding(
      padding: EdgeInsets.only(bottom: 8),
      child: Row(
        mainAxisAlignment:
            isMe ? MainAxisAlignment.end : MainAxisAlignment.start,
        crossAxisAlignment: CrossAxisAlignment.end,
        children: [
          if (!isMe && showAvatar) ...[
            CustomCircleAvatar(
              backgroundImage:
                  NetworkImage('https://i.sstatic.net/B7tGA.gif?s=256'),
              radius: 16,
            ),
            SizedBox(width: 8),
          ],
          if (!isMe && !showAvatar) SizedBox(width: 40),
          Column(
            crossAxisAlignment:
                isMe ? CrossAxisAlignment.end : CrossAxisAlignment.start,
            children: [
              Container(
                constraints: BoxConstraints(
                    maxWidth: MediaQuery.of(context).size.width * 0.7),
                padding: EdgeInsets.symmetric(horizontal: 16, vertical: 10),
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
                                image: NetworkImage(attachment.filePath),
                                fit: BoxFit.cover,
                              ),
                            ),
                          ),
                    if (message.attachments != null)
                      for (var attachment in message.attachments!)
                        if (attachment.fileType == 'video')
                          Container(
                            width: 200,
                            height: 150,
                            child: VideoPlayerWidget(url: attachment.filePath),
                          ),
                    if (message.content != null)
                      Text(
                        message.content!,
                        style: AppTypography.message.copyWith(
                          color: isMe ? AppColors.white : AppColors.secondary,
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
    );
  }

  // Widget _buildAudioMessage(String duration) {
  //   return Row(
  //     mainAxisSize: MainAxisSize.min,
  //     children: [
  //       Icon(Icons.play_arrow, color: AppColors.white, size: 20),
  //       SizedBox(width: 8),
  //       Container(
  //         width: 100,
  //         height: 4,
  //         decoration: BoxDecoration(
  //           color: AppColors.white.withOpacity(0.5),
  //           borderRadius: BorderRadius.circular(2),
  //         ),
  //       ),
  //       SizedBox(width: 8),
  //       Text(
  //         duration,
  //         style: TextStyle(color: AppColors.white, fontSize: 12),
  //       ),
  //     ],
  //   );
  // }

  Widget _buildAudioMessage(String duration, String url) {
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        IconButton(
          icon: Icon(Icons.play_arrow, color: AppColors.white, size: 20),
          onPressed: () => _playPauseAudio(url),
        ),
        SizedBox(width: 8),
        Container(
          width: 100,
          height: 30,
          // child: AudioFileWaveforms(
          //   playerWaveStyle: const PlayerWaveStyle(
          //     fixedWaveColor: AppColors.white,
          //     liveWaveColor: AppColors.primary,
          //     spacing: 6,
          //   ),
          //   size: Size(MediaQuery.of(context).size.width, 100.0),
          //   playerController: _playerController,
          //   waveformType: WaveformType.long,
          //   enableSeekGesture: true,
          //   waveformData: [0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0],
          // ),
        ),
        SizedBox(width: 8),
        Text(
          duration,
          style: TextStyle(color: AppColors.white, fontSize: 12),
        ),
      ],
    );
  }

  // Widget _buildMessageInput() {
  //   return Container(
  //     padding: EdgeInsets.symmetric(horizontal: 8, vertical: 8),
  //     decoration: BoxDecoration(
  //       color: Colors.white,
  //       boxShadow: [
  //         BoxShadow(
  //           color: Colors.grey.withOpacity(0.1),
  //           spreadRadius: 1,
  //           blurRadius: 3,
  //           offset: Offset(0, -1),
  //         ),
  //       ],
  //     ),
  //     child: Row(
  //       children: [
  //         IconButton(
  //           icon: CustomSvgIcon(
  //             svgPath: 'assets/icons/Clip, Attachment.svg',
  //             color: AppColors.secondary,
  //           ),
  //           onPressed: _pickAction,
  //         ),
  //         Expanded(
  //           child: TextField(
  //             controller: _textController,
  //             onChanged: (text) {
  //               setState(() {
  //                 _isComposing = text.isNotEmpty;
  //               });
  //             },
  //             decoration: InputDecoration(
  //               hintText: 'Aa',
  //               hintStyle: TextStyle(color: Colors.grey[400]),
  //               border: OutlineInputBorder(
  //                 borderRadius: BorderRadius.circular(25),
  //                 borderSide: BorderSide.none,
  //               ),
  //               filled: true,
  //               fillColor: AppColors.surface,
  //               // Dùng để làm gì trong đây?  // Để tạo màu nền cho TextField
  //               contentPadding:
  //                   EdgeInsets.symmetric(horizontal: 20, vertical: 10),
  //               suffixIcon: IconButton(
  //                 icon: CustomSvgIcon(
  //                   svgPath: 'assets/icons/files_in_message.svg',
  //                   color: AppColors.tertiary,
  //                   size: 24,
  //                 ),
  //                 onPressed: _pickSticker,
  //               ),
  //             ),
  //           ),
  //         ),
  //         if (!_isComposing) ...[
  //           // Dùng để làm gì trong đây? // Hiển thị các icon khi không có nội dung trong TextField
  //           IconButton(
  //             icon: CustomSvgIcon(
  //               svgPath: 'assets/icons/camera 01_in_message.svg',
  //               color: AppColors.secondary,
  //               size: 24,
  //             ),
  //             onPressed: () {},
  //           ),
  //           IconButton(
  //             icon: CustomSvgIcon(
  //               svgPath: 'assets/icons/microphone_in_message.svg',
  //               color: AppColors.secondary,
  //               size: 24,
  //             ),
  //             onPressed: _pickVideo,
  //           ),
  //         ],
  //         if (_isComposing)
  //           IconButton(
  //             icon: CustomSvgIcon(
  //               svgPath: 'assets/icons/Send_in_message.svg',
  //               color: AppColors.primary,
  //             ),
  //             onPressed: _handleSubmitted,
  //           ),
  //       ],
  //     ),
  //   );
  // }

  // Widget _buildMessageInput() {
  //   return Container(
  //     padding: EdgeInsets.symmetric(horizontal: 8, vertical: 8),
  //     decoration: BoxDecoration(
  //       color: Colors.white,
  //       boxShadow: [
  //         BoxShadow(
  //           color: Colors.grey.withOpacity(0.1),
  //           spreadRadius: 1,
  //           blurRadius: 3,
  //           offset: Offset(0, -1),
  //         ),
  //       ],
  //     ),
  //     child: Row(
  //       children: [
  //         IconButton(
  //           icon: Icon(Icons.image),
  //           onPressed: _pickImage,
  //         ),
  //         IconButton(
  //           icon: Icon(Icons.videocam),
  //           onPressed: _pickVideo,
  //         ),
  //         IconButton(
  //           icon: Icon(Icons.emoji_emotions),
  //           onPressed: _pickSticker,
  //         ),
  //         Expanded(
  //           child: TextField(
  //             controller: _textController,
  //             onChanged: (text) {
  //               setState(() {
  //                 _isComposing = text.isNotEmpty;
  //               });
  //             },
  //             decoration: InputDecoration(
  //               hintText: 'Aa',
  //               hintStyle: TextStyle(color: Colors.grey[400]),
  //               border: OutlineInputBorder(
  //                 borderRadius: BorderRadius.circular(25),
  //                 borderSide: BorderSide.none,
  //               ),
  //               filled: true,
  //               fillColor: AppColors.surface,
  //               contentPadding:
  //                   EdgeInsets.symmetric(horizontal: 20, vertical: 10),
  //               suffixIcon: IconButton(
  //                 icon: Icon(Icons.send),
  //                 onPressed: _handleSubmitted,
  //               ),
  //             ),
  //           ),
  //         ),
  //       ],
  //     ),
  //   );
  // }

  void _handleSubmitted(String text) {
    // final text = _textController.text;
    if (text.isNotEmpty || _selectedImage != null || _selectedVideo != null) {
      setState(() {
        // messages.add(Message(
        //   chatId: 'chatId',
        //   // Add a valid chat ID
        //   senderId: 'Nazrul',
        //   // Use a valid sender ID
        //   createdDate: DateTime.now(),
        //   chat: Chat(
        //       id: 'chatId',
        //       name: 'Chat Name',
        //       isGroup: false,
        //       userChats: [],
        //       messages: []),
        //   // Add a valid Chat object
        //   content: text,
        //   attachments: [
        //     if (_selectedImage != null)
        //       Attachment(
        //           fileName: 'image',
        //           filePath: _selectedImage!,
        //           fileType: 'image',
        //           fileSize: 100,
        //           messageId: 'messageId'),
        //     if (_selectedVideo != null)
        //       Attachment(
        //           fileName: 'video',
        //           filePath: _selectedVideo!,
        //           fileType: 'video',
        //           fileSize: 100,
        //           messageId: 'messageId'),
        //   ],
        //   sender: Account(
        //     id: 'Nazrul',
        //     fullName: 'Nazrul',
        //     email: 'Nazrul',
        //     phoneNumber: 'Nazrul',
        //   ),
        // ));

        // Gọi hàm gửi tin nhắn tới server ở đây
        _chatProvider.sendMessage(widget.chatId, text);

        _textController.clear();
        _isComposing = false;
        _selectedImage = null;
        _selectedVideo = null;
      });
      _scrollToBottom();
    }
  }

  void _pickImage() async {
    // Implement image picker logic
  }

  void _pickVideo() async {
    // Implement video picker logic
  }

  void _pickSticker() async {
    // Hiển thị một modal bottom sheet để chọn sticker
    final selectedSticker = await showModalBottomSheet<String>(
      context: context,
      builder: (context) {
        return Container(
          height: 200,
          child: ListView(
            children: [
              // Hiển thị các sticker tại đây sử dụng telegram_stickers_importer để import sticker từ Telegram ở dưới đây
            ],
          ),
        );
      },
    );
    if (selectedSticker != null) {
      setState(() {
        _textController.text = selectedSticker;
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
        return Container(
          //Chiều cao của modal bootom sheet vừa đủ để hiển thị các lựa chọn và không bị tràn ra
          // Modal phải có tiêu đề và có nút close để đóng modal
          height: 300,
          child: Column(
            children: [
              // Có một thanh ngang màu xám ở đầu modal dùng để kéo modal
              Container(
                height: 4,
                width: 40,
                margin: EdgeInsets.symmetric(vertical: 8),
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
                    title: Text('Media'),
                    subtitle: Text('Share photos and videos'),
                    onTap: () {
                      Navigator.pop(context);
                      _pickImage();
                    },
                  ),
                  ListTile(
                    leading: CustomSvgIcon(
                      svgPath: 'assets/icons/doc_in_message.svg',
                      color: AppColors.tertiary,
                    ),
                    title: Text('File', style: AppTypography.p1),
                    subtitle: Text('Share files, documents, and more'),
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
                    title: Text('Location'),
                    subtitle: Text('Share your location'),
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

  // void _handleSubmitted() {
  //   final text = _textController.text;
  //   if (text.isNotEmpty) {
  //     setState(() {
  //       messages.add(Message(
  //         sender: 'Nazrul',
  //         content: text,
  //         timestamp: DateTime.now(),
  //       ));
  //       _textController.clear();
  //       _isComposing = false;
  //     });
  //     _scrollToBottom();
  //   }
  // }

  // bool _shouldShowAvatar(int index) {
  //   if (index == 0) return true;
  //   final currentMessage = messages[index];
  //   final previousMessage = messages[index - 1];
  //   return currentMessage.senderId != previousMessage.senderId ||
  //       currentMessage.sentAt.difference(previousMessage.sentAt).inMinutes >= 1;
  // }

  // Hiển thị avatar nếu tin nhắn hiện tại không phải của người gửi trước đó
  bool _shouldShowAvatar(List<Message> messages, int index) {
    if (index == 0) return true;
    final currentMessage = messages[index];
    final previousMessage = messages[index - 1];
    return currentMessage.senderId != previousMessage.senderId;
  }

  // bool _shouldShowTimestamp(int index) {
  //   if (index == messages.length - 1) return true;
  //   final currentMessage = messages[index];
  //   final nextMessage = messages[index + 1];
  //   return currentMessage.senderId != nextMessage.senderId ||
  //       nextMessage.sentAt.difference(currentMessage.sentAt).inMinutes >= 1;
  // }
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

class VideoPlayerWidget extends StatefulWidget {
  final String url;

  VideoPlayerWidget({required this.url});

  @override
  _VideoPlayerWidgetState createState() => _VideoPlayerWidgetState();
}

class _VideoPlayerWidgetState extends State<VideoPlayerWidget> {
  late VideoPlayerController _controller;

  @override
  void initState() {
    super.initState();
    _controller = VideoPlayerController.networkUrl(Uri.parse(widget.url))
      ..initialize().then((_) {
        setState(() {});
      });
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    // Hiển thị hình ảnh tĩnh thay vì video và hiển thị một nút để phát video khi người dùng nhấn vào
    // return Stack(
    //   children: [
    //     _controller.value.isInitialized
    //         ? AspectRatio(
    //             aspectRatio: _controller.value.aspectRatio, // 16:9
    //             child: VideoPlayer(_controller),
    //           )
    //         : Center(child: CircularProgressIndicator()),
    //     Center(
    //       child: IconButton(
    //         icon: Icon(
    //           _controller.value.isPlaying ? Icons.pause : Icons.play_arrow,
    //           color: AppColors.white,
    //           size: 40,
    //         ),
    //         onPressed: () {
    //           setState(() {
    //             if (_controller.value.isPlaying) {
    //               _controller.pause();
    //             } else {
    //               _controller.play();
    //             }
    //           });
    //         },
    //       ),
    //     ),
    //   ],
    // );
    // Video hiển thị chiều cao chưa đúng khi dùng stack để chứa video player và nút play/pause, hãy sửa dưới đây
    return _controller.value.isInitialized
        ? AspectRatio(
            aspectRatio: _controller.value.aspectRatio, // 16:9
            child: Stack(
              children: [
                VideoPlayer(_controller),
                Center(
                  child: IconButton(
                    icon: Icon(
                      _controller.value.isPlaying
                          ? Icons.pause
                          : Icons.play_arrow,
                      color: AppColors.white,
                      size: 40,
                    ),
                    onPressed: () {
                      setState(() {
                        if (_controller.value.isPlaying) {
                          _controller.pause();
                        } else {
                          _controller.play();
                        }
                      });
                    },
                  ),
                ),
              ],
            ),
          )
        : Center(child: CircularProgressIndicator());
  }
}
