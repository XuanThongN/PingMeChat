// Đây là file chứa các hằng số dùng chung trong ứng dụng
// Các hằng số này bao gồm các URL API, thông tin authen, thông tin chatHub
// Để sử dụng các hằng số này, ta chỉ cần import file này vào nơi cần sử dụng
const String baseUrl = 'http://api.pingmechat.xuanthongn.me';

class ApiConstants {
  // Base URL
  static const String baseApiUrl = '$baseUrl/api';

  // Auth
  static const String loginEndpoint = '$baseApiUrl/auth/login';
  static const String registerEndpoint = '$baseApiUrl/auth/register';
  static const String updateFCMTokenEndpoint =
      '$baseApiUrl/users/update-fcm-token';
  static const String reSendVerificationCodeEndpoint =
      '$baseApiUrl/auth/resend-verification-code';
  static const String verifyCodeEndpoint = '$baseApiUrl/auth/verify-code';
  static const String verifyResetCodeEndpoint =
      '$baseApiUrl/auth/verify-reset-code';
  static const String forgotPasswordEndpoint =
      '$baseApiUrl/auth/forgot-password';
  static const String resetPasswordEndpoint = '$baseApiUrl/auth/reset-password';

  // Profile
  static const String userProfileEndpoint = '$baseApiUrl/users/profile';
  static const String updateUserInfoEndpoint =
      '$baseApiUrl/users/update-profile';
  static const String updateUserAvatarEndpoint =
      '$baseApiUrl/users/update-avatar';

  // Chat
  static const String getChatListEndpoint = '$baseApiUrl/chats/get-chat-list';
  static String getChatByIdEndpoint(String chatId) {
    return '$baseApiUrl/chats/get-chat-detail/$chatId';
  }

  static const String startNewPrivateChatEndpoint =
      '$baseApiUrl/chats/start-new-private-chat';
  // Message
  static String getMessagesEndpoint(
      String chatId, int pageNumber, int pageSize) {
    return '$baseApiUrl/messages/$chatId?pageNumber=$pageNumber&pageSize=$pageSize';
  }

  // Contact
  static const String getContactListByCurrentUserEndpoint =
      '$baseApiUrl/contacts/get-all-by-current-user'; // Lấy danh sách contact của user hiện tại
  static const String addContactEndpoint =
      '$baseApiUrl/contacts/add'; // Thêm contact
  static const String removeContactEndpoint =
      '$baseApiUrl/contacts/remove'; // Xoá contact
  static const String acceptFriendRequestEndpoint =
      '$baseApiUrl/contacts/accept-friend-request'; // Chấp nhận lời mời kết bạn
  static const String cancelFriendRequestEndpoint =
      '$baseApiUrl/contacts/cancel-friend-request'; // Hủy lời mời kết bạn
  static const String sendFriendRequestEndpoint =
      '$baseApiUrl/contacts/send-friend-request'; // Gửi lời mời kết bạn
  static const String getRecommendedFriendsEndpoint =
      '$baseApiUrl/contacts/recommend-friends'; // Lấy danh sách bạn bè gợi ý
  static const String getFriendStatusEndpoint =
      '$baseApiUrl/contacts/get-all-friends-status'; // Lấy trạng thái bạn bè

  // File upload
  static const String uploadFileEndpoint =
      '$baseApiUrl/attachments/upload-multiple-files';
  static const String uploadChunkEndpoint =
      '$baseApiUrl/attachments/upload-chunk';
  static const String completeUploadEndpoint =
      '$baseApiUrl/attachments/complete-upload';

  // Search
  static const String searchEndpoint = '$baseApiUrl/search';

  // Member chat
  static String getAddMembersToChatEndpoint(String chatId) {
    return '$baseApiUrl/chats/add-users-to-chat/$chatId';
  }

  static String getRemoveMemberFromChatEndpoint(String chatId) {
    return '$baseApiUrl/chats/remove-user-from-chat/$chatId';
  }
}

// ChatHub
class ChatHubConstants {
  static const String chatHubUrl = '$baseUrl/chatHub';
}

// Ảnh tĩnh
class ImageConstants {
  static const String defaultAvatarPath = 'assets/images/default_avatar.png';
  static const String defaultGroupAvatarPath =
      'assets/images/default_group_avatar.jpg';
}

// Status của user
class ContactStatus {
  static const String PENDING = 'Pending'; // Đang chờ
  static const String ACCEPTED = 'Accepted'; // Đã chấp nhận
  static const String BLOCKED = 'Blocked'; // Bị chặn
  static const String STRANGER = 'Stranger'; // Người lạ
  static const String REQUESTED = 'Requested'; // Đã gửi yêu cầu
  static const String CANCELLED = 'Cancelled'; // Đã hủy yêu cầu
}
