import 'package:flutter/material.dart';
import 'package:intl/intl.dart';

import '../../config/theme.dart';
import '../../domain/models/message.dart';
import '../widgets/custom_icon.dart';

class ChatPageHelper {
  // void _pickAction(BuildContext context) {
  //   _isComposing = false;
  //   // Hiển thị một modal bottom sheet để chọn hành động (image, share file, share location, ...) nếu là cuộc trò chuyẹn nhóm thì có thêm chức năng tạo khảo sát
  //   showModalBottomSheet(
  //     context: context,
  //     builder: (context) {
  //       return SizedBox(
  //         //Chiều cao của modal bootom sheet vừa đủ để hiển thị các lựa chọn và không bị tràn ra
  //         // Modal phải có tiêu đề và có nút close để đóng modal
  //         height: 300,
  //         child: Column(
  //           children: [
  //             // Có một thanh ngang màu xám ở đầu modal dùng để kéo modal
  //             Container(
  //               height: 4,
  //               width: 40,
  //               margin: const EdgeInsets.symmetric(vertical: 8),
  //               decoration: BoxDecoration(
  //                 color: AppColors.tertiary,
  //                 borderRadius: BorderRadius.circular(2),
  //               ),
  //             ),
  //             Column(
  //               children: [
  //                 ListTile(
  //                   leading: CustomSvgIcon(
  //                     svgPath: 'assets/icons/media_in_message.svg',
  //                     color: AppColors.tertiary,
  //                   ),
  //                   title: const Text('Media'),
  //                   subtitle: const Text('Share photos and videos'),
  //                   onTap: () {
  //                     Navigator.pop(context);
  //                     _pickMedia();
  //                   },
  //                 ),
  //                 ListTile(
  //                   leading: CustomSvgIcon(
  //                     svgPath: 'assets/icons/doc_in_message.svg',
  //                     color: AppColors.tertiary,
  //                   ),
  //                   title: const Text('File', style: AppTypography.p1),
  //                   subtitle: const Text('Share files, documents, and more'),
  //                   onTap: () {
  //                     Navigator.pop(context);
  //                     // Implement file picker logic
  //                   },
  //                 ),
  //                 ListTile(
  //                   leading: CustomSvgIcon(
  //                     svgPath: 'assets/icons/Pin, Location.svg',
  //                     color: AppColors.tertiary,
  //                   ),
  //                   title: const Text('Location'),
  //                   subtitle: const Text('Share your location'),
  //                   onTap: () {
  //                     Navigator.pop(context);
  //                     // Implement location picker logic
  //                   },
  //                 ),
  //               ],
  //             ),
  //           ],
  //         ),
  //       );
  //     },
  //   );
  // }

  // Thêm hàm pick recording khi thực hiện sẽ hiện thị thanh xử lý ghi âm từ người dùng và biến nó thành file sau đó thì thêm vào _selectedAttachments
  static void pickRecording(BuildContext context) {
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
  static bool shouldShowAvatar(List<Message> messages, int index) {
    if (index == 0) return true;
    final currentMessage = messages[index];
    final previousMessage = messages[index - 1];
    return currentMessage.senderId != previousMessage.senderId;
  }

  // Hiển thị timestamp nếu tin nhắn hiện tại và tin nhắn tiếp theo được gửi cách nhau ít nhất 1 phút
  static bool shouldShowTimestamp(List<Message> messages, int index) {
    if (index == messages.length - 1) return true;
    final currentMessage = messages[index];
    final nextMessage = messages[index + 1];
    return nextMessage.createdDate
            .difference(currentMessage.createdDate)
            .inMinutes >=
        1;
  }

  // Kiem tra xem hai ngay co phai cung mot ngay khong
  static bool isSameDay(DateTime date1, DateTime date2) {
    return date1.year == date2.year &&
        date1.month == date2.month &&
        date1.day == date2.day;
  }

  // Kiểm tra xem hai ngày có phải cùng một ngày không
  static String getFormattedDate(DateTime date) {
    final now = DateTime.now();
    final yesterday = now.subtract(const Duration(days: 1));

    if (isSameDay(date, now)) {
      return 'Hôm nay';
    } else if (isSameDay(date, yesterday)) {
      return 'Hôm qua';
    } else {
      return DateFormat('dd/MM/yyyy').format(date);
    }
  }
}
