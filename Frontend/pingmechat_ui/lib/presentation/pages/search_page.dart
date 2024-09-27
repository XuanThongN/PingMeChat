import 'package:flutter/material.dart';

class SearchResultsScreen extends StatefulWidget {
    static const routeName = '/search';
  @override
  _SearchResultsScreenState createState() => _SearchResultsScreenState();
}

class _SearchResultsScreenState extends State<SearchResultsScreen> {
  final TextEditingController _searchController = TextEditingController();
  List<Person> _people = [
    Person('Adil Adnan', 'Be your own hero 💪', Color(0xFFFFD54F)),
    Person('Bristy Haque', 'Keep working ✍️', Colors.white),
    Person('John Borino', 'Make yourself proud 🤩', Color(0xFFBDBDBD)),
  ];
  List<Group> _groups = [
    Group('Team Align-Practise', 4, [Color(0xFFFFA726), Color(0xFF42A5F5), Color(0xFF66BB6A)]),
    Group('Team Align', 8, [Color(0xFF42A5F5), Color(0xFFBDBDBD), Color(0xFFFFEE58)]),
  ];

  List<Person> _filteredPeople = [];
  List<Group> _filteredGroups = [];

  @override
  void initState() {
    super.initState();
    _filteredPeople = _people;
    _filteredGroups = _groups;
    _searchController.addListener(_performSearch);
  }

  void _performSearch() {
    final query = _searchController.text.toLowerCase();
    setState(() {
      _filteredPeople = _people.where((person) => 
        person.name.toLowerCase().contains(query) || 
        person.status.toLowerCase().contains(query)
      ).toList();
      _filteredGroups = _groups.where((group) => 
        group.name.toLowerCase().contains(query)
      ).toList();
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      body: SafeArea(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            SearchBar(controller: _searchController),
            Expanded(
              child: ListView(
                children: [
                  if (_filteredPeople.isNotEmpty) ...[
                    SectionTitle(title: 'People'),
                    ..._filteredPeople.map((person) => PersonTile(person: person)),
                  ],
                  if (_filteredGroups.isNotEmpty) ...[
                    SectionTitle(title: 'Group Chat'),
                    ..._filteredGroups.map((group) => GroupTile(group: group)),
                  ],
                  if (_filteredPeople.isEmpty && _filteredGroups.isEmpty)
                    Center(child: Text('No results found')),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }
}

class SearchBar extends StatelessWidget {
  final TextEditingController controller;

  SearchBar({required this.controller});

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: EdgeInsets.all(16),
      padding: EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      decoration: BoxDecoration(
        color: Color(0xFFF5F5F5),
        borderRadius: BorderRadius.circular(10),
      ),
      child: Row(
        children: [
          Icon(Icons.search, color: Colors.grey),
          SizedBox(width: 8),
          Expanded(
            child: TextField(
              controller: controller,
              decoration: InputDecoration(
                hintText: 'People',
                border: InputBorder.none,
                hintStyle: TextStyle(color: Colors.black, fontSize: 16),
              ),
              style: TextStyle(color: Colors.black, fontSize: 16),
            ),
          ),
          GestureDetector(
            onTap: () => controller.clear(),
            child: Icon(Icons.close, color: Colors.grey),
          ),
        ],
      ),
    );
  }
}

class SectionTitle extends StatelessWidget {
  final String title;

  SectionTitle({required this.title});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      child: Text(
        title,
        style: TextStyle(fontSize: 22, fontWeight: FontWeight.bold),
      ),
    );
  }
}

class PersonTile extends StatelessWidget {
  final Person person;

  PersonTile({required this.person});

  @override
  Widget build(BuildContext context) {
    return ListTile(
      contentPadding: EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      leading: CircleAvatar(
        backgroundColor: person.avatarColor,
        radius: 28,
        backgroundImage: NetworkImage('https://example.com/placeholder.jpg'),
      ),
      title: Text(person.name, style: TextStyle(fontWeight: FontWeight.bold, fontSize: 18)),
      subtitle: Text(person.status, style: TextStyle(fontSize: 14)),
      onTap: () {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Selected ${person.name}')),
        );
      },
    );
  }
}

class GroupTile extends StatelessWidget {
  final Group group;

  GroupTile({required this.group});

  @override
  Widget build(BuildContext context) {
    return ListTile(
      contentPadding: EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      leading: SizedBox(
        width: 56,
        height: 56,
        child: Stack(
          children: [
            Positioned(
              left: 0,
              top: 0,
              child: CircleAvatar(backgroundColor: group.avatarColors[0], radius: 20),
            ),
            Positioned(
              right: 0,
              top: 0,
              child: CircleAvatar(backgroundColor: group.avatarColors[1], radius: 20),
            ),
            Positioned(
              left: 8,
              bottom: 0,
              child: CircleAvatar(backgroundColor: group.avatarColors[2], radius: 20),
            ),
          ],
        ),
      ),
      title: Text(group.name, style: TextStyle(fontWeight: FontWeight.bold, fontSize: 18)),
      subtitle: Text('${group.participants} participants', style: TextStyle(fontSize: 14, color: Colors.grey[600])),
      trailing: Container(
        width: 10,
        height: 10,
        decoration: BoxDecoration(
          color: Colors.green,
          shape: BoxShape.circle,
        ),
      ),
      onTap: () {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Selected ${group.name}')),
        );
      },
    );
  }
}

class Person {
  final String name;
  final String status;
  final Color avatarColor;

  Person(this.name, this.status, this.avatarColor);
}

class Group {
  final String name;
  final int participants;
  final List<Color> avatarColors;

  Group(this.name, this.participants, this.avatarColors);
}