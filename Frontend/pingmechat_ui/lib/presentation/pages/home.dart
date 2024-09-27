import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:pingmechat_ui/config/theme.dart';
import 'package:flutter_gen/gen_l10n/app_localizations.dart';
import 'package:pingmechat_ui/presentation/pages/call_tab.dart';
import 'package:pingmechat_ui/presentation/pages/contact_tab.dart';
import 'package:pingmechat_ui/presentation/pages/message_tab.dart';
import 'package:pingmechat_ui/presentation/pages/settting_tab.dart';

import '../widgets/custom_icon.dart';

class HomePage extends StatefulWidget {
  static const routeName = '/home';

  const HomePage({Key? key}) : super(key: key);

  @override
  _HomePageState createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  int _selectedIndex = 0;
  int _contactsBadgeCount = 1;
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
        child: Column( // Giải thích: 
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
          icon: CustomSvgIcon(svgPath: 'assets/icons/Message.svg', color: AppColors.tertiary),
          activeIcon: CustomSvgIcon(svgPath: 'assets/icons/Message.svg', color: AppColors.primary),
          label: AppLocalizations.of(context)!.message,
        ),
        BottomNavigationBarItem(
          icon: CustomSvgIcon(svgPath: 'assets/icons/Call.svg'),
          activeIcon: CustomSvgIcon(svgPath: 'assets/icons/Call.svg', color: AppColors.primary),
          label: AppLocalizations.of(context)!.calls,
        ),
        BottomNavigationBarItem(
            icon: Badge(
              label: Text('$_contactsBadgeCount', style: TextStyle(color: Colors.white)),
              child: CustomSvgIcon(svgPath: 'assets/icons/user.svg'),
            ),
            activeIcon: Badge(
              label: Text('$_contactsBadgeCount', style: TextStyle(color: Colors.white)),
              child: CustomSvgIcon(svgPath: 'assets/icons/user.svg', color: AppColors.primary),
            ),
            label: AppLocalizations.of(context)!.contacts,
          ),
        BottomNavigationBarItem(
          icon: CustomSvgIcon(svgPath: 'assets/icons/settings.svg'),
          activeIcon: CustomSvgIcon(svgPath: 'assets/icons/settings.svg', color: AppColors.primary),
          label: AppLocalizations.of(context)!.settings,
        ),
      ],
    );
  }
}
