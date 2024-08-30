import 'package:flutter/material.dart';

class MediaLinksDocumentsPage extends StatefulWidget {
  @override
  _MediaLinksDocumentsPageState createState() =>
      _MediaLinksDocumentsPageState();
}

class _MediaLinksDocumentsPageState extends State<MediaLinksDocumentsPage>
    with SingleTickerProviderStateMixin {
  late TabController _tabController;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 3, vsync: this);
  }

  @override
  void dispose() {
    _tabController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('Media, Links & Documents',
            style: TextStyle(color: Colors.black)),
        backgroundColor: Colors.white,
        elevation: 0,
        iconTheme: IconThemeData(color: Colors.black),
        bottom: TabBar(
          controller: _tabController,
          labelColor: Colors.black,
          unselectedLabelColor: Colors.grey,
          indicatorColor: Colors.black,
          tabs: [
            Tab(text: 'Media'),
            Tab(text: 'Links'),
            Tab(text: 'Documents'),
          ],
        ),
      ),
      body: TabBarView(
        controller: _tabController,
        children: [
          // Media Tab
          GridView.builder(
            padding: EdgeInsets.all(10),
            gridDelegate: SliverGridDelegateWithFixedCrossAxisCount(
              crossAxisCount: 3,
              crossAxisSpacing: 10,
              mainAxisSpacing: 10,
            ),
            itemCount: 10, // Replace with actual number of media files
            itemBuilder: (context, index) {
              return Container(
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(8),
                  image: DecorationImage(
                    image: AssetImage(
                        'assets/media_$index.jpg'), // Replace with actual media path
                    fit: BoxFit.cover,
                  ),
                ),
              );
            },
          ),
          // Links Tab
          ListView(
            children: [
              ListTile(
                leading: Icon(Icons.link, color: Colors.blue),
                title: Text('160+ FREE Tab Bar Component Types'),
                subtitle: Text(
                    'https://www.figma.com/community/file/1312921033225014799/160-free-tab-bar-component-types'),
                onTap: () {
                  // Mở liên kết trong trình duyệt tại đây
                },
              ),
              ListTile(
                leading: Icon(Icons.link, color: Colors.blue),
                title: Text('150+ FREE Stepper / Wizard Component'),
                subtitle: Text(
                    'https://www.figma.com/community/file/1344038523080556624/150-free-stepper-wizard-component'),
                onTap: () {},
              ),
              // More ListTiles for each link
            ],
          ),
          // Documents Tab
          ListView(
            children: [
              ListTile(
                leading: Icon(Icons.insert_drive_file, color: Colors.orange),
                title: Text('Document1.pdf'),
                subtitle: Text('Yesterday, 10:15 AM'),
                onTap: () {},
              ),
              ListTile(
                leading: Icon(Icons.insert_drive_file, color: Colors.orange),
                title: Text('Document2.docx'),
                subtitle: Text('Last Week, 2:00 PM'),
                onTap: () {},
              ),
              // More ListTiles for each document
            ],
          ),
        ],
      ),
    );
  }
}
