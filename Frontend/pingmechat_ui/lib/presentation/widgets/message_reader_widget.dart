 import 'package:flutter/material.dart';

import '../../domain/models/chat.dart';
import '../../domain/models/message.dart';
import '../../domain/models/userchat.dart';
import 'custom_circle_avatar.dart';

Widget buildMessageReadersWidget(Message message, Chat? chat, String? currentUserId) {
    if (chat == null || message.messageReaders == null || message.messageReaders!.isEmpty) {
      return SizedBox.shrink();
    }

    final otherReaders = message.messageReaders!
        .where((reader) => reader.readerId != currentUserId)
        .toList();

    if (otherReaders.isEmpty) {
      return SizedBox.shrink();
    }

    return Padding(
      padding: const EdgeInsets.only(left: 16, right: 16, bottom: 8),
      child: Row(
        children: [
          Text(
            'Seen by ',
            style: TextStyle(
              color: Colors.grey[600],
              fontSize: 12,
            ),
          ),
          Expanded(
            child: SizedBox(
              height: 24,
              child: ListView.builder(
                scrollDirection: Axis.horizontal,
                itemCount: otherReaders.length,
                itemBuilder: (context, index) {
                  final reader = otherReaders[index];
                  final userChat = chat.userChats.firstWhere(
                    (uc) => uc.userId == reader.readerId,
                    orElse: () => UserChat(userId: reader.readerId, chatId: chat.id, isAdmin: false, id: ''),
                  );
                  return Padding(
                    padding: const EdgeInsets.only(right: 4),
                    child: CustomCircleAvatar(
                      backgroundImage: userChat.user?.avatarUrl != null
                          ? NetworkImage(userChat.user!.avatarUrl!)
                          : null,
                      radius: 12,
                    ),
                  );
                },
              ),
            ),
          ),
        ],
      ),
    );
  }