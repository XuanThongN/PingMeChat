class Account {
  final String id;
  final String fullName;
  final String email;
  final String? phoneNumber;

  Account({
    required this.id,
    required this.fullName,
    required this.email,
    this.phoneNumber,
  });

  factory Account.fromJson(Map<String, dynamic> json) {
    return Account(
      id: json['id'],
      fullName: json['fullName'],
      email: json['email'],
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
