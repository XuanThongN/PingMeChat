class SearchResult {
  final List<User> users;
  final List<GroupChat> groupChats;

  SearchResult({
    required this.users,
    required this.groupChats,
  });

  factory SearchResult.fromJson(Map<String, dynamic> json) {
    return SearchResult(
      users: (json['users'] as List).map((user) => User.fromJson(user)).toList(),
      groupChats: (json['groupChats'] as List).map((groupChat) => GroupChat.fromJson(groupChat)).toList(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'users': users.map((user) => user.toJson()).toList(),
      'groupChats': groupChats.map((groupChat) => groupChat.toJson()).toList(),
    };
  }
}


class User {
  final String userName;
  final String fullName;
  final String email;
  final String phoneNumber;
  final String? avatarUrl;
  final bool isFriend;

  User({
    required this.userName,
    required this.fullName,
    required this.email,
    required this.phoneNumber,
    this.avatarUrl,
    required this.isFriend,
  });

  factory User.fromJson(Map<String, dynamic> json) {
    return User(
      userName: json['userName'],
      fullName: json['fullName'],
      email: json['email'],
      phoneNumber: json['phoneNumber'],
      avatarUrl: json['avatarUrl'] ?? '',
      isFriend: json['isFriend'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'userName': userName,
      'fullName': fullName,
      'email': email,
      'phoneNumber': phoneNumber,
      'avatarUrl': avatarUrl,
      'isFriend': isFriend,
    };
  }
}

class GroupChat {
  final String name;
  final bool isGroup;
  final String? avatarUrl;
  final List<UserChat> userChats;
  final List<dynamic>? messages;
  final String id;
  final String? createdDate;
  final String? createdDateString;
  final String? createdBy;
  final String? updatedDate;
  final String? updatedDateString;
  final String? updatedBy;

  GroupChat({
    required this.name,
    required this.isGroup,
    this.avatarUrl,
    required this.userChats,
     this.messages,
    required this.id,
     this.createdDate,
    this.createdDateString,
     this.createdBy,
     this.updatedDate,
    this.updatedDateString,
     this.updatedBy,
  });

  factory GroupChat.fromJson(Map<String, dynamic> json) {
    return GroupChat(
      name: json['name'],
      isGroup: json['isGroup'],
      avatarUrl: json['avatarUrl'],
      userChats: (json['userChats'] as List).map((userChat) => UserChat.fromJson(userChat)).toList(),
      messages: [],
      id: json['id'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'name': name,
      'isGroup': isGroup,
      'avatarUrl': avatarUrl,
      'userChats': userChats,
      'messages': messages,
      'id': id,
      'createdDate': createdDate,
      'createdDateString': createdDateString,
      'createdBy': createdBy,
      'updatedDate': updatedDate,
      'updatedDateString': updatedDateString,
      'updatedBy': updatedBy,
    };
  }
}

class UserChat {
  final String id;
  final String fullName;
  final String? avatarUrl;

  UserChat({
    required this.id,
    required this.fullName,
    this.avatarUrl,
  });

  factory UserChat.fromJson(Map<String, dynamic> json) {
    return UserChat(
      id: json['id'] ?? '',
      fullName: json['fullName'] ?? '',
      avatarUrl: json['avatarUrl'] ?? '',
    );
  }
}




