import 'dart:math';

import 'package:flutter/material.dart';
import 'package:pingmechat_ui/config/theme.dart';
import 'package:pingmechat_ui/presentation/pages/home.dart';
import 'package:pingmechat_ui/presentation/pages/login_page.dart';
import 'package:pingmechat_ui/splash_screen.dart';
import 'package:provider/provider.dart';

import 'app.dart';

void main() {
  runApp(MyApp());
}

class MyApp extends StatelessWidget {
  // Giải thích: StatelessWidget là một widget không thể thay đổi trạng thái sau khi khởi tạo
  const MyApp({Key? key})
      : super(
            key:
                key); //Giải thích: super(key: key) là một tham số mặc định của constructor, bạn có thể bỏ qua nó
  @override //Giải thích: @override là một annotation, nó bắt buộc phải có khi bạn muốn ghi đè một phương thức từ lớp cha
  Widget build(BuildContext context) {
    return ChangeNotifierProvider(
      //Giải thích lý do dùng ChangeNotifierProvider ở đây là gì?  ChangeNotifierProvider là một widget cha, nó sẽ cung cấp một instance của MyAppState cho các widget con
      create: (context) => MyAppState(),
      //Giải thích: ChangeNotifierProvider là một widget cha, nó sẽ cung cấp một instance của MyAppState cho các widget con
      child: MaterialApp(
        debugShowCheckedModeBanner: false,
        title: 'Namer App',
        theme: ThemeData(
          useMaterial3: true,
          //Giải thích: useMaterial3 là một thuộc tính của ThemeData, nó sẽ sử dụng Material 3.0 cho ứng dụng
          colorScheme: ColorScheme.fromSeed(
              seedColor: Colors
                  .deepOrange), //Giải thích: ColorScheme.fromSeed(seedColor: Colors.deepOrange) tạo ra một ColorScheme từ một màu seed
        ),
        //Giải thích: ThemeData là một class chứa các thuộc tính để tạo ra một theme cho ứng dụng
        home:
            MyHomePage(), //Giải thích: home là một thuộc tính bắt buộc của MaterialApp, nó sẽ chứa widget chính của ứng dụng
      ), //Giải thích: MaterialApp là một widget cha, nó sẽ cung cấp một số thuộc tính cơ bản cho ứng dụng như theme, home, routes, v.v.
      // còn việc sử dụng child để bọc MaterialApp là để cung cấp một số thuộc tính cụ thể cho MaterialApp như theme, home, routes, v.v.
    );
  }
}

class MyHomePage extends StatefulWidget {
  @override
  State<MyHomePage> createState() => _MyHomePageState();
}

class _MyHomePageState extends State<MyHomePage> {
  //Giải thích: _MyHomePageState là một class con của MyHomePage, nó chứa trạng thái của MyHomePage
  var selectedIndex = 0;

  @override
  Widget build(BuildContext context) {
    var colorScheme = Theme.of(context).colorScheme;
    Widget page;
    switch (selectedIndex) {
      case 0:
        page = GeneratorPage();
        break;
      case 1:
        page = FavoritePage();
        break;
      default:
        throw UnimplementedError('Unknown index: $selectedIndex');
    }

    // the container for the current page, with its background color
    // and subtle switching animation
    var mainArea = ColoredBox(
      color: colorScheme.surfaceContainerHighest,
      //Giải thích: colorScheme.surface trả về màu nền của ứng dụng
      child: AnimatedSwitcher(
        //Giải thích: AnimatedSwitcher là một widget giúp tạo ra một hiệu ứng chuyển đổi mượt mà giữa các widget con
        duration: const Duration(milliseconds: 200),
        child: page,
      ),
    );

    return Scaffold(
      body: LayoutBuilder(builder: (context, constraint) {
        if (constraint.maxWidth < 450) {
          // Use a more mobile-friendly layout with BottomNavigationBar
          // on narrow screens.
          return Column(
            children: [
              Expanded(
                child: mainArea,
              ),
              SafeArea(
                child: BottomNavigationBar(
                  items: [
                    BottomNavigationBarItem(
                        icon: Icon(Icons.home), label: 'Home'),
                    BottomNavigationBarItem(
                        icon: Icon(Icons.favorite), label: 'Favcrite'),
                  ],
                  currentIndex: selectedIndex,
                  onTap: (value) {
                    setState(() {
                      selectedIndex = value;
                    });
                  },
                ),
              ),
            ],
          );
        } else {
          return Row(
            children: [
              SafeArea(
                  child: NavigationRail(
                extended: constraint.maxWidth >= 600,
                destinations: const [
                  NavigationRailDestination(
                      icon: Icon(Icons.home), label: Text('Home')),
                  NavigationRailDestination(
                      icon: Icon(Icons.favorite), label: Text('Favorite')),
                ],
                selectedIndex: selectedIndex,
                onDestinationSelected: (value) {
                  setState(() {
                    selectedIndex = value;
                  });
                },
              )),
              Expanded(child: mainArea),
            ],
          );
        }
      }),
    );
  }
}

class GeneratorPage extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    var appState = context.watch<MyAppState>();
    var pair = appState.current;

    IconData icon;
    if (appState.favorites.contains(pair)) {
      icon = Icons.favorite;
    } else {
      icon = Icons.favorite_border;
    }
    return Center(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Expanded(
            flex: 2,
            // Giải thích: flex là một thuộc tính của Expanded, nó xác định tỉ lệ chiếm không gian của widget con
            child: HistoryListView(),
          ),
          SizedBox(height: 20),
          BigCard(pair: pair),
          //Giải thích: BigCard là một widget tự định nghĩa, bạn có thể tìm hiểu ở dưới
          SizedBox(height: 10),
          Row(
            mainAxisSize: MainAxisSize.min,
            children: [
              ElevatedButton.icon(
                onPressed: () {
                  appState.toggleFavorite();
                },
                icon: Icon(icon),
                label: Text('Like'),
              ),
              SizedBox(width: 10),
              //Giải thích SizedBox: Widget này giúp tạo ra một khoảng cách giữa các widget con
              ElevatedButton.icon(
                onPressed: () {
                  appState.getNext();
                },
                icon: Icon(Icons.arrow_forward),
                label: Text('Next'),
              ),
            ],
          ),
          Spacer(
            flex: 2,
          )
          //Giải thích: Spacer là một widget giúp tạo ra một khoảng cách giữa các widget con
        ],
      ),
    );
  }
}

class FavoritePage extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    var theme = Theme.of(context);
    var appState = context.watch<MyAppState>();
    if (appState.favorites.isEmpty) {
      return Center(
        child: Text('No favorite yet'),
      );
    }
    return Column(
      children: [
        Padding(
          padding: const EdgeInsets.all(20),
          child: Text('You have ${appState.favorites.length} favorites:'),
        ),
        Expanded(
            child: GridView(
          gridDelegate: SliverGridDelegateWithMaxCrossAxisExtent(
            //Giải thích: SliverGridDelegateWithMaxCrossAxisExtent là một delegate cho GridView, nó sẽ tạo ra một grid với chiều rộng tối đa cho mỗi item
            maxCrossAxisExtent: 400,
            //Giải thích: maxCrossAxisExtent là một thuộc tính của SliverGridDelegateWithMaxCrossAxisExtent, nó sẽ xác định chiều rộng tối đa của mỗi item
            childAspectRatio: 400 /
                80, //Giải thích: childAspectRatio là một thuộc tính của SliverGridDelegateWithMaxCrossAxisExtent, nó sẽ xác định tỉ lệ giữa chiều rộng và chiều cao của mỗi item
          ),
          children: [
            // Padding(
            //   padding: const EdgeInsets.all(20),
            //   child: Text('You have ${appState.favorites.length} favorites:'),
            // ),
            ...appState.favorites.map((e) => ListTile(
                  leading: IconButton(
                    icon: Icon(
                      Icons.delete_outline,
                      semanticLabel: 'Delete',
                      color: theme.colorScheme.primary,
                      textDirection: TextDirection.rtl,
                    ),
                    onPressed: () {
                      appState.removeFavorite(e);
                    },
                  ),
                  title: Text(
                    e.toString(),
                  ),
                )),
          ],
        ))
      ],
    );
  }
}

class MyAppState extends ChangeNotifier {
  var current = Random().nextInt(100);
  var history = <int>[];
  GlobalKey?
      historyListKey; //Giải thích: GlobalKey là một key đặc biệt, nó giúp truy cập trực tiếp đến một widget con từ một widget cha
  void getNext() {
    history.insert(0,
        current); //Giải thích: insert(0, current) chèn current vào vị trí đầu tiên của history
    var animatedList = historyListKey?.currentState
        as AnimatedListState?; //Giải thích: historyListKey.currentState trả về trạng thái của widget con được xác định bởi historyListKey
    animatedList?.insertItem(
        0); //Giải thích: insertItem(0) chèn một item vào vị trí đầu tiên của danh sách
    current = Random().nextInt(100);
    notifyListeners();
  }

  var favorites = <int>[];

  void toggleFavorite([int? pair]) {
    pair = pair ?? current;
    //Giải thích: pair là một tham số tùy chọn, bạn có thể bỏ qua nó
    if (favorites.contains(current)) {
      favorites.remove(current);
    } else {
      favorites.add(current);
    }
    notifyListeners();
  }

  void removeFavorite(int pair) {
    favorites.remove(pair);
    notifyListeners();
  }
}

class HistoryListView extends StatefulWidget {
  const HistoryListView({Key? key}) : super(key: key);

  @override
  State<HistoryListView> createState() => _HistoryListViewState();
}

class _HistoryListViewState extends State<HistoryListView> {
  final _key =
      GlobalKey(); //Giải thích: GlobalKey là một key đặc biệt, nó giúp truy cập trực tiếp đến một widget con từ một widget cha
  static const Gradient _maskingGradient = LinearGradient(
    //Giải thích: LinearGradient là một lớp để tạo ra một gradient từ hai hoặc nhiều màu
    colors: [Colors.transparent, Colors.black],
    stops: [0.0, 0.5],
    //Giải thích: stops là một danh sách các giá trị từ 0.0 đến 1.0, nó xác định vị trí của mỗi màu trong gradient
    // Giảii thích Alignment.topCenter: Alignment.topCenter là một hằng số, nó xác định vị trí topCenter của widget
    begin: Alignment.topCenter,
    //Giải thích: begin là một thuộc tính của LinearGradient, nó xác định vị trí bắt đầu của gradient trong widget
    end: Alignment.bottomCenter,
  );

  @override
  Widget build(BuildContext context) {
    final appState = context.watch<MyAppState>();
    appState.historyListKey = _key;
    return ShaderMask(
      shaderCallback: (bounds) => _maskingGradient.createShader(bounds),
      //Giải thích: ShaderMask là một widget giúp tạo ra một mask cho widget con
      //Giải thích: shaderCallback là một thuộc tính của ShaderMask, nó sẽ trả về một Shader
      //Giải thích: _maskingGradient.createShader(bounds) tạo ra một Shader từ _maskingGradient
      blendMode: BlendMode.dstIn,
      //Giải thích: blendMode là một thuộc tính của ShaderMask, nó xác định cách thức sử dụng shader và màu của widget con
      child: AnimatedList(
        key: _key,
        //Giải thích: key là một thuộc tính bắt buộc của AnimatedList, nó giúp xác định một widget con trong một danh sách các widget con
        reverse: true,
        //Giải thích: reverse là một thuộc tính của AnimatedList, nó xác định xem danh sách có bắt đầu từ cuối không
        padding: EdgeInsets.only(top: 100),
        initialItemCount: appState.history.length,
        //Giải thích: initialItemCount là một thuộc tính bắt buộc của AnimatedList, nó xác định số lượng item ban đầu của danh sách
        itemBuilder: (context, index, animation) {
          //Giải thích: itemBuilder là một hàm callback, nó sẽ trả về một widget tại vị trí index
          final pair = appState.history[index];
          return SizeTransition(
            sizeFactor: animation,
            child: Center(
              child: TextButton.icon(
                onPressed: () {
                  appState.toggleFavorite(pair);
                },
                icon: appState.favorites.contains(pair)
                    ? Icon(Icons.favorite, size: 12)
                    : SizedBox(),
                label: Text(pair.toString(), semanticsLabel: 'Pair $pair'),
              ),
            ),
          );
          //Giải thích: sizeFactor là một thuộc tính của SizeTransition, nó xác định kích thước của widget con
        },
      ),
    );
  }
}

class BigCard extends StatelessWidget {
  const BigCard({Key? key, required this.pair})
      : super(
            key:
                key); //Giải thích: super.key là một tham số mặc định của constructor, bạn có thể bỏ qua nó
  final int
      pair; //Giải thích: final là từ khóa để khai báo một biến không thể thay đổi giá trị sau khi khởi tạo
  @override
  Widget build(BuildContext context) {
    //Giải thích: Hàm build sẽ trả về một widget
    final theme = Theme.of(
        context); //Giải thích: Theme.of(context) trả về theme hiện tại của ứng dụng
    final style = theme.textTheme.displayMedium!.copyWith(
      color: theme.colorScheme
          .onPrimary, //Giải thích: theme.colorScheme.onPrimary trả về màu chữ mặc định trên màu nền chính của ứng dụng
    );

    return Card(
      color: theme.colorScheme.primary,
      //Giải thích: theme.colorScheme.primary trả về màu nền chính của ứng dụng
      // child là một tham số bắt buộc của Card, nó sẽ chứa nội dung của Card
      // Padding giúp tạo ra một khoảng cách giữa nội dung và viền của Card
      // Text là một widget hiển thị một đoạn văn bản
      // children là một tham số bắt buộc của Row, nó sẽ chứa các widget con của Row
      // mainAxisSize: MainAxisSize.min giúp Row co lại với kích thước nhỏ nhất có thể
      child: Padding(
        //Giải thích: Padding là một widget giúp tạo ra một khoảng cách giữa nội dung và viền của widget cha
        padding: const EdgeInsets.all(20),
        //Giải thích: EdgeInsets.all(20) tạo ra một khoảng cách 20px ở cả bốn phía của widget
        child: AnimatedSize(
          duration: const Duration(milliseconds: 200),
          child: MergeSemantics(
            child: Wrap(
              children: [
                Text(
                  'Pair: ',
                  style: style,
                ),
                Text(
                  pair.toString(),
                  style: style.copyWith(
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
