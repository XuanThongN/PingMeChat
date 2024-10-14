class Account {
  final String id;
  final String fullName;
  final String? userName;
  final String email;
  final String? phoneNumber;
  final String? avatarUrl;
  bool isOnline;

  Account({
    required this.id,
    required this.fullName,
     this.userName,
    required this.email,
    this.phoneNumber,
    this.avatarUrl,
    this.isOnline = false,
  });

  factory Account.fromJson(Map<String, dynamic> json) {
    return Account(
      id: json['id'],
      fullName: json['fullName'],
      userName: json['userName'] ?? '',
      email: json['email'],
      phoneNumber: json['phoneNumber'],
      avatarUrl: json['avatarUrl'],
      isOnline: json['isOnline'] ?? false,
      // phoneNumber: json['phoneNumber'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'fullName': fullName,
      'email': email,
      'phoneNumber': phoneNumber,
    };
  }
}
