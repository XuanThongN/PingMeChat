// Đây là file chứa các hằng số dùng chung trong ứng dụng
// Các hằng số này bao gồm các URL API, thông tin authen, thông tin chatHub
// Để sử dụng các hằng số này, ta chỉ cần import file này vào nơi cần sử dụng
const String baseUrl = 'https://jxhq42vd-7043.asse.devtunnels.ms';


class ApiConstants {
  static const String baseApiUrl = '$baseUrl/api';
  static const String loginEndpoint = '/auth/login';
  static const String registerEndpoint = '/auth/register';
  static const String userProfileEndpoint = '/user/profile';
  static const String sendMessageEndpoint = '/message/send';
  static const String fetchMessagesEndpoint = '/message/fetch';
}

class ChatHubConstants {
  static const String chatHubUrl = '$baseUrl/chatHub';
}
