import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_svg/svg.dart';
import 'package:pingmechat_ui/config/theme.dart';
import 'package:flutter_slidable/flutter_slidable.dart';
import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter_gen/gen_l10n/app_localizations.dart';
import 'package:pingmechat_ui/presentation/pages/call_tab.dart';
import 'package:pingmechat_ui/presentation/pages/chat_page.dart';
import 'package:pingmechat_ui/presentation/pages/contact_tab.dart';
import 'package:pingmechat_ui/presentation/pages/message_tab.dart';
import 'package:pingmechat_ui/presentation/pages/settting_tab.dart';

import '../widgets/custom_icon.dart';
import 'create_group_page.dart';

class HomePage extends StatefulWidget {
  const HomePage({Key? key}) : super(key: key);

  @override
  _HomePageState createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  int _selectedIndex = 0;
  static final List<Widget> _tabs = <Widget>[
    MessageTab(),
    CallTab(),
    ContactTab(),
    SetttingTab(),
  ];

  void _onItemTapped(int index) {
    setState(() {
      _selectedIndex = index;
    });
  }

  @override
  Widget build(BuildContext context) {
    SystemChrome.setSystemUIOverlayStyle(SystemUiOverlayStyle.light);
    return Scaffold(
      backgroundColor: AppColors.background,
      body: SafeArea(
        child: Column(
          children: [
            Expanded(child: _tabs[_selectedIndex]),
            _buildBottomNavBar(),
          ],
        ),
      ),
    );
  }

  Widget _buildBottomNavBar() {
    return BottomNavigationBar(
      backgroundColor: AppColors.surface,
      type: BottomNavigationBarType.fixed,
      selectedItemColor: AppColors.primary,
      unselectedItemColor: AppColors.tertiary,
      currentIndex: _selectedIndex,
      onTap: _onItemTapped,
      items: [
        BottomNavigationBarItem(
          icon: CustomSvgIcon(svgPath: 'assets/icons/Message.svg'),
          label: AppLocalizations.of(context)!.message,
        ),
        BottomNavigationBarItem(
          icon: CustomSvgIcon(svgPath: 'assets/icons/Call.svg'),
          label: AppLocalizations.of(context)!.calls,
        ),
        BottomNavigationBarItem(
          icon: CustomSvgIcon(svgPath: 'assets/icons/user.svg'),
          label: AppLocalizations.of(context)!.contacts,
        ),
        BottomNavigationBarItem(
          icon: CustomSvgIcon(svgPath: 'assets/icons/settings.svg'),
          label: AppLocalizations.of(context)!.settings,
        ),
      ],
    );
  }
}
