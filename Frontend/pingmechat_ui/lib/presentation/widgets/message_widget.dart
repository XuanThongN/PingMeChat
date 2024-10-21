import 'dart:io';
import 'dart:typed_data';

import 'package:audioplayers/audioplayers.dart';
import 'package:cached_network_image/cached_network_image.dart';
  import 'package:flutter/material.dart';
import 'package:pingmechat_ui/presentation/widgets/upload_progress_indicator.dart';
import 'package:provider/provider.dart';
import 'package:intl/intl.dart';
import 'package:video_player/video_player.dart';
import 'package:video_thumbnail/video_thumbnail.dart';
import '../../config/theme.dart';
import '../../domain/models/attachment.dart';
import '../../domain/models/message.dart';
import '../../providers/auth_provider.dart';
import '../pages/attachment_detail_page.dart';
import '../widgets/custom_circle_avatar.dart';
import 'video_player_screen.dart';

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
                ? _buildLocalVideoThumbnail(attachment)
                : _buildNetworkVideoThumbnail(attachment),
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

  Widget _buildLocalVideoThumbnail(Attachment attachment) {
    return FutureBuilder<Uint8List?>(
      future: _generateVideoThumbnail(attachment.fileUrl),
      builder: (context, snapshot) {
        if (snapshot.connectionState == ConnectionState.done &&
            snapshot.data != null) {
          return Image.memory(
            snapshot.data!,
            width: 150,
            height: 150,
            fit: BoxFit.cover,
          );
        } else {
          return Container(
            width: 150,
            height: 150,
            color: Colors.grey[300],
            child: const Center(child: CircularProgressIndicator()),
          );
        }
      },
    );
  }

  Widget _buildNetworkVideoThumbnail(Attachment attachment) {
    return CachedNetworkImage(
      imageUrl: attachment.thumbnailUrl ?? '',
      width: 150,
      height: 150,
      fit: BoxFit.cover,
      placeholder: (context, url) => Container(
        width: 150,
        height: 150,
        color: Colors.grey[300],
        child: const Center(child: CircularProgressIndicator()),
      ),
      errorWidget: (context, url, error) => Container(
        width: 150,
        height: 150,
        color: Colors.grey[300],
        child: const Center(child: Icon(Icons.error, color: Colors.red)),
      ),
    );
  }

  Future<Uint8List?> _generateVideoThumbnail(String videoPath) async {
    try {
      final thumbnail = await VideoThumbnail.thumbnailData(
        video: videoPath.replaceFirst('file://', ''),
        imageFormat: ImageFormat.JPEG,
        maxWidth: 150,
        quality: 25,
      );
      return thumbnail;
    } catch (e) {
      print('Error generating thumbnail: $e');
      return null;
    }
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
    final controller = url.startsWith('file://')
        ? VideoPlayerController.file(File(url.replaceFirst('file://', '')))
        : VideoPlayerController.networkUrl(Uri.parse(url));
    try {
      await controller.initialize();
    } catch (e) {
      print('Error initializing video player: $e');
      rethrow;
    }
    return controller;
  }

  void _viewAttachmentDetail(BuildContext context, Attachment attachment) {
  if (attachment.fileType == 'Video') {
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => VideoPlayerScreen(videoUrl: attachment.fileUrl),
      ),
    );
  } else {
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => AttachmentDetailView(attachment: attachment),
      ),
    );
  }
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
