import 'package:flutter/material.dart';

class SetttingTab extends StatefulWidget {
  @override
  State<SetttingTab> createState() => _SetttingTabState();
}

class _SetttingTabState extends State<SetttingTab> {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('Setting'),
      ),
      body: Center(
        child: Text('Setting Tab'),
      ),
    );
  }
}
