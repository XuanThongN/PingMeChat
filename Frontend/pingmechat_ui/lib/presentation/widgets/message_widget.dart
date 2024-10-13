import 'dart:io';

import 'package:audioplayers/audioplayers.dart';
import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';
import 'package:pingmechat_ui/presentation/widgets/upload_progress_indicator.dart';
import 'package:provider/provider.dart';
import 'package:intl/intl.dart';
import 'package:video_player/video_player.dart';
import '../../config/theme.dart';
import '../../domain/models/attachment.dart';
import '../../domain/models/message.dart';
import '../../providers/auth_provider.dart';
import '../widgets/custom_circle_avatar.dart';

class ChatMessageWidget extends StatelessWidget {
  final Message message;
  final bool isLastMessageFromSameSender;
  final bool shouldShowAvatar;
  final bool shouldShowDateDivider;
  final DateTime? previousMessageDate;
  final bool isGroupMessage;
  final bool showTimestamp;
  const ChatMessageWidget({
    super.key,
    required this.message,
    required this.isLastMessageFromSameSender,
    required this.shouldShowAvatar,
    this.shouldShowDateDivider = false,
    this.previousMessageDate,
    this.isGroupMessage = false,
    this.showTimestamp = false,
  });

  @override
  Widget build(BuildContext context) {
    final authProvider = Provider.of<AuthProvider>(context, listen: false);
    final isCurrentUser = message.senderId == authProvider.currentUser?.id;

    return Column(
      children: [
        if (shouldShowDateDivider) _buildDateDivider(),
        Padding(
          padding: const EdgeInsets.symmetric(vertical: 4, horizontal: 8),
          child: Row(
            mainAxisAlignment:
                isCurrentUser ? MainAxisAlignment.end : MainAxisAlignment.start,
            crossAxisAlignment: CrossAxisAlignment.end,
            children: [
              if (!isCurrentUser && shouldShowAvatar)
                CustomCircleAvatar(
                  backgroundImage: message.sender?.avatarUrl != null
                      ? NetworkImage(message.sender!.avatarUrl!)
                      : null,
                  radius: 16,
                )
              else
                const SizedBox(width: 32),
              const SizedBox(width: 8),
              Flexible(
                child: Column(
                  crossAxisAlignment: isCurrentUser
                      ? CrossAxisAlignment.end
                      : CrossAxisAlignment.start,
                  children: [
                    if (!isCurrentUser && isGroupMessage && shouldShowAvatar)
                      Padding(
                        padding: const EdgeInsets.only(left: 8, bottom: 4),
                        child: Text(
                          message.sender?.fullName ?? 'Unknown',
                          style: const TextStyle(
                            fontSize: 12,
                            fontWeight: FontWeight.bold,
                            color: AppColors.primary,
                          ),
                        ),
                      ),
                    Container(
                      padding: const EdgeInsets.symmetric(
                          horizontal: 12, vertical: 8),
                      decoration: BoxDecoration(
                        color: isCurrentUser
                            ? AppColors.primary_chat
                            : AppColors.surface,
                        borderRadius: BorderRadius.circular(16),
                      ),
                      child: Column(
                        crossAxisAlignment: isCurrentUser
                            ? CrossAxisAlignment.end
                            : CrossAxisAlignment.start,
                        children: [
                          if (message.attachments != null &&
                              message.attachments!.isNotEmpty)
                            ...message.attachments!.map((attachment) =>
                                _buildAttachmentPreview(context, attachment)),
                          if (message.content != null &&
                              message.content!.isNotEmpty)
                            Text(
                              message.content!,
                              style: TextStyle(
                                color:
                                    isCurrentUser ? Colors.white : Colors.black,
                              ),
                            ),
                        ],
                      ),
                    ),
                    const SizedBox(height: 4),
                    Row(
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        if (showTimestamp)
                          Text(
                            DateFormat('HH:mm').format(message.createdDate),
                            style: TextStyle(
                              color: Colors.grey[600],
                              fontSize: 12,
                            ),
                          ),
                        const SizedBox(width: 4),
                        if (isCurrentUser &&
                            message.status != MessageStatus.sent)
                          _buildStatusIcon(
                              message.status ?? MessageStatus.sending),
                      ],
                    ),
                  ],
                ),
              ),
              const SizedBox(width: 8),
            ],
          ),
        ),
      ],
    );
  }

  Widget _buildStatusIcon(MessageStatus status) {
    IconData iconData;
    Color color;

    switch (status) {
      case MessageStatus.sending:
        iconData = Icons.access_time;
        color = Colors.grey;
        break;
      case MessageStatus.sent:
        iconData = Icons.check;
        color = Colors.grey;
        break;
      case MessageStatus.delivered:
        iconData = Icons.done_all;
        color = Colors.grey;
        break;
      case MessageStatus.read:
        iconData = Icons.done_all;
        color = Colors.blue;
        break;
      case MessageStatus.failed:
        iconData = Icons.error_outline;
        color = Colors.red;
        break;
    }

    return Icon(iconData, size: 16, color: color);
  }

  Widget _buildDateDivider() {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 16),
      child: Row(
        children: [
          Expanded(child: Divider(color: Colors.grey[300])),
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 8),
            child: Text(
              _formatDate(message.createdDate),
              style: TextStyle(
                color: Colors.grey[600],
                fontSize: 12,
                fontWeight: FontWeight.w500,
              ),
            ),
          ),
          Expanded(child: Divider(color: Colors.grey[300])),
        ],
      ),
    );
  }

  Widget _buildAttachmentPreview(BuildContext context, Attachment attachment) {
    switch (attachment.fileType) {
      case 'Image':
        return _buildImagePreview(context, attachment);
      case 'Video':
        return _buildVideoPreview(context, attachment);
      default:
        return _buildFilePreview(context, attachment);
    }
  }

  Widget _buildImagePreview(BuildContext context, Attachment attachment) {
    return GestureDetector(
      onTap: () => _viewAttachmentDetail(context, attachment),
      child: Stack(
        alignment: Alignment.center,
        children: [
          ClipRRect(
            borderRadius: BorderRadius.circular(8),
            child: attachment.fileUrl.startsWith('file://')
                ? Image.file(
                    File(attachment.fileUrl.replaceFirst('file://', '')),
                    width: 150,
                    height: 150,
                    fit: BoxFit.cover,
                  )
                : CachedNetworkImage(
                    imageUrl: attachment.fileUrl,
                    width: 150,
                    height: 150,
                    fit: BoxFit.cover,
                  ),
          ),
          UploadProgressIndicator(isUploading: attachment.isUploading),
        ],
      ),
    );
  }

  Widget _buildVideoPreview(BuildContext context, Attachment attachment) {
    return GestureDetector(
      onTap: () => _viewAttachmentDetail(context, attachment),
      child: Stack(
        alignment: Alignment.center,
        children: [
          ClipRRect(
            borderRadius: BorderRadius.circular(8),
            child: attachment.fileUrl.startsWith('file://')
                ? Image.file(
                    File(attachment.fileUrl.replaceFirst('file://', '')),
                    width: 150,
                    height: 150,
                    fit: BoxFit.cover,
                  )
                : CachedNetworkImage(
                    imageUrl: attachment.thumbnailUrl ?? '',
                    width: 150,
                    height: 150,
                    fit: BoxFit.cover,
                  ),
          ),
          const Icon(Icons.play_circle_fill, size: 40, color: Colors.white),
          if (attachment.isUploading)
            const CircularProgressIndicator(
              valueColor: AlwaysStoppedAnimation<Color>(Colors.white),
            ),
        ],
      ),
    );
  }

  Widget _buildFilePreview(BuildContext context, Attachment attachment) {
    return GestureDetector(
      onTap: () => _viewAttachmentDetail(context, attachment),
      child: Container(
        padding: EdgeInsets.all(8),
        decoration: BoxDecoration(
          color: Colors.grey[200],
          borderRadius: BorderRadius.circular(8),
        ),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(_getAttachmentIcon(attachment.fileType), size: 24),
            SizedBox(width: 8),
            Flexible(
              child: Text(
                attachment.fileName ?? '',
                style: TextStyle(fontSize: 12),
                overflow: TextOverflow.ellipsis,
              ),
            ),
            if (attachment.isUploading)
              Padding(
                padding: EdgeInsets.only(left: 8),
                child: SizedBox(
                  width: 16,
                  height: 16,
                  child: CircularProgressIndicator(
                    strokeWidth: 2,
                  ),
                ),
              ),
          ],
        ),
      ),
    );
  }

  Future<VideoPlayerController> _initializeVideoPlayer(String url) async {
    final controller = VideoPlayerController.network(url);
    await controller.initialize();
    return controller;
  }

  void _viewAttachmentDetail(BuildContext context, Attachment attachment) {
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => AttachmentDetailPage(attachment: attachment),
      ),
    );
  }

  IconData _getAttachmentIcon(String type) {
    switch (type) {
      case 'Image':
        return Icons.image;
      case 'Video':
        return Icons.videocam;
      case 'Audio':
        return Icons.audiotrack;
      case 'File':
        return Icons.insert_drive_file;
      default:
        return Icons.attachment;
    }
  }

  String _formatDate(DateTime date) {
    final now = DateTime.now();
    final yesterday = DateTime(now.year, now.month, now.day - 1);
    final messageDate = DateTime(date.year, date.month, date.day);

    if (messageDate == DateTime(now.year, now.month, now.day)) {
      return 'Today';
    } else if (messageDate == yesterday) {
      return 'Yesterday';
    } else {
      return DateFormat('MMMM d, y').format(date);
    }
  }
}

enum AttachmentType {
  image,
  video,
  audio,
  file,
}

class AttachmentDetailPage extends StatelessWidget {
  final Attachment attachment;

  const AttachmentDetailPage({Key? key, required this.attachment})
      : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('Attachment Detail'),
      ),
      body: Center(
        child: _getAttachmentDetailView(attachment),
      ),
    );
  }

  Widget _getAttachmentDetailView(Attachment attachment) {
    switch (attachment.fileType) {
      case 'Image':
        return CachedNetworkImage(
          imageUrl: attachment.fileUrl,
        );
      case 'Video':
        return FutureBuilder(
          future: _initializeVideoPlayer(attachment.fileUrl),
          builder: (context, snapshot) {
            if (snapshot.connectionState == ConnectionState.done) {
              return VideoPlayer(snapshot.data as VideoPlayerController);
            } else {
              return Center(child: CircularProgressIndicator());
            }
          },
        );
      case 'Audio':
        return AudioPlayerWidget(url: attachment.fileUrl);
      case 'File':
        return Text('File preview is not supported.');
      default:
        return Text('Attachment preview is not supported.');
    }
  }

  Future<VideoPlayerController> _initializeVideoPlayer(String url) async {
    final controller = VideoPlayerController.network(url);
    await controller.initialize();
    return controller;
  }
}

class AudioPlayerWidget extends StatefulWidget {
  final String url;

  const AudioPlayerWidget({Key? key, required this.url}) : super(key: key);

  @override
  _AudioPlayerWidgetState createState() => _AudioPlayerWidgetState();
}

class _AudioPlayerWidgetState extends State<AudioPlayerWidget> {
  late AudioPlayer _audioPlayer;
  bool isPlaying = false;

  @override
  void initState() {
    super.initState();
    _audioPlayer = AudioPlayer();
  }

  @override
  void dispose() {
    _audioPlayer.dispose();
    super.dispose();
  }

  void _togglePlayPause() {
    if (isPlaying) {
      _audioPlayer.pause();
    } else {
      _audioPlayer.play(UrlSource(widget.url));
    }
    setState(() {
      isPlaying = !isPlaying;
    });
  }

  @override
  Widget build(BuildContext context) {
    return IconButton(
      icon: Icon(isPlaying ? Icons.pause : Icons.play_arrow),
      onPressed: _togglePlayPause,
    );
  }
}
