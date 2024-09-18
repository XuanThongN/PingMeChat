class Account {
  final String fullName;
  final String email;
  final String phoneNumber;

  Account({
    required this.fullName,
    required this.email,
    required this.phoneNumber,
  });

  factory Account.fromJson(Map<String, dynamic> json) {
    return Account(
      fullName: json['fullName'],
      email: json['email'],
      phoneNumber: json['phoneNumber'],
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