// Đây là file chứa các hằng số dùng chung trong ứng dụng
// Các hằng số này bao gồm các URL API, thông tin authen, thông tin chatHub
// Để sử dụng các hằng số này, ta chỉ cần import file này vào nơi cần sử dụng
const String baseUrl = 'https://jxhq42vd-7043.asse.devtunnels.ms';
class ApiConstants {
  // Base URL
  static const String baseApiUrl = '$baseUrl/api';

  // Auth
  static const String loginEndpoint = '$baseApiUrl/auth/login';
  static const String registerEndpoint = '$baseApiUrl/auth/register';

  // Profile
  static const String userProfileEndpoint = '$baseApiUrl/user/profile';

  // Chat
  static const String getChatListEndpoint = '$baseApiUrl/chats/get-chat-list';
  static String getChatByIdEndpoint(String chatId) {
    return '$baseApiUrl/chats/get-chat-detail/$chatId';
  }
  static const String startNewPrivateChatEndpoint = '$baseApiUrl/chats/start-new-private-chat';
  // Message
  static String getMessagesEndpoint(String chatId, int pageNumber, int pageSize) {
    return '$baseApiUrl/messages/$chatId?pageNumber=$pageNumber&pageSize=$pageSize';
  }

  // Contact
  static const String getContactListByCurrentUserEndpoint = '$baseApiUrl/contacts/get-all-by-current-user'; // Lấy danh sách contact của user hiện tại
  static const String addContactEndpoint = '$baseApiUrl/contacts/add'; // Thêm contact
  static const String removeContactEndpoint = '$baseApiUrl/contacts/remove'; // Xoá contact

  // File upload
  static const String uploadFileEndpoint = '$baseApiUrl/files/upload';
}

// ChatHub
class ChatHubConstants {
  static const String chatHubUrl = '$baseUrl/chatHub';
}
