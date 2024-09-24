import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:intl/intl.dart';
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
    Key? key,
    required this.message,
    required this.isLastMessageFromSameSender,
    required this.shouldShowAvatar,
    this.shouldShowDateDivider = false,
    this.previousMessageDate,
    this.isGroupMessage = false,
    this.showTimestamp = false,
  }) : super(key: key);

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
            mainAxisAlignment: isCurrentUser ? MainAxisAlignment.end : MainAxisAlignment.start,
            crossAxisAlignment: CrossAxisAlignment.start,
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
                  crossAxisAlignment: isCurrentUser ? CrossAxisAlignment.end : CrossAxisAlignment.start,
                  children: [
                    if (!isCurrentUser && isGroupMessage && shouldShowAvatar)
                      Padding(
                        padding: const EdgeInsets.only(left: 8, bottom: 4),
                        child: Text(
                          message.sender?.fullName ?? 'Unknown',
                          style: TextStyle(
                            fontSize: 12,
                            fontWeight: FontWeight.bold,
                            color: AppColors.primary,
                          ),
                        ),
                      ),
                    Container(
                      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
                      decoration: BoxDecoration(
                        color: isCurrentUser ? AppColors.primary_chat : AppColors.surface,
                        borderRadius: BorderRadius.circular(16),
                      ),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          if (message.attachments != null && message.attachments!.isNotEmpty)
                            ...message.attachments!.map((attachment) => _buildAttachmentPreview(attachment)),
                          if (message.content != null && message.content!.isNotEmpty)
                            Text(
                              message.content!,
                              style: TextStyle(
                                color: isCurrentUser ? Colors.white : Colors.black,
                              ),
                            ),
                        ],
                      ),
                    ),
                    if (showTimestamp)
                      Text(
                        DateFormat('HH:mm').format(message.createdDate),
                        style: TextStyle(
                          color: Colors.grey[600],
                          fontSize: 12,
                        ),
                      ),
                  ],
                ),
              ),
              const SizedBox(width: 8),
              // if (isCurrentUser && shouldShowAvatar)
              //   CustomCircleAvatar(
              //     backgroundImage: authProvider.currentUser?.avatarUrl != null
              //         ? NetworkImage(authProvider.currentUser!.avatarUrl!)
              //         : null,
              //     radius: 16,
              //   )
              // else
              //   const SizedBox(width: 32),
            ],
          ),
        ),
      ],
    );
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

  Widget _buildAttachmentPreview(Attachment attachment) {
    // This is a placeholder. You should implement proper preview for each attachment type.
    return Container(
      width: 200,
      height: 150,
      margin: const EdgeInsets.only(bottom: 8),
      decoration: BoxDecoration(
        color: Colors.grey[200],
        borderRadius: BorderRadius.circular(8),
      ),
      child: Center(
        child: Icon(
          _getAttachmentIcon(attachment.fileType as AttachmentType),
          size: 48,
          color: Colors.grey[400],
        ),
      ),
    );
  }

  IconData _getAttachmentIcon(AttachmentType type) {
    switch (type) {
      case AttachmentType.image:
        return Icons.image;
      case AttachmentType.video:
        return Icons.videocam;
      case AttachmentType.audio:
        return Icons.audiotrack;
      case AttachmentType.file:
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