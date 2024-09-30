import 'dart:async';
import 'package:signalr_core/signalr_core.dart';
import '../../providers/auth_provider.dart';
import '../../core/constants/constant.dart';

class SignalRConnection {
  HubConnection? _hubConnection;
  final AuthProvider authProvider;
  final Completer<void> _connectionCompleter = Completer<void>();
  bool _isConnected = false;

  SignalRConnection(this.authProvider) {
    authProvider.addListener(_handleAuthChange);
  }

  Future<void> initialize() async {
    if (!authProvider.isAuth) {
      print('User not authenticated. Skipping connection initialization.');
      return;
    }
    await _initializeConnection();
  }

  Future<void> _initializeConnection() async {
    try {
      _hubConnection = await _buildHubConnection();
      _setupConnectionHandlers();
      await _hubConnection!.start();
      print('Connected to SignalR hub');
      _isConnected = true;
      if (!_connectionCompleter.isCompleted) {
        _connectionCompleter.complete();
      }
    } catch (e) {
      print('Error initializing connection: $e');
      await _handleConnectionError(e);
    }
  }

  Future<void> connect() async {
    if (_isConnected) return;
    if (!authProvider.isAuth) {
      throw Exception('User is not authenticated');
    }
    await _initializeConnection();
  }

  Future<HubConnection> _buildHubConnection() async {
    final connection = HubConnectionBuilder()
        .withUrl(
          ChatHubConstants.chatHubUrl,
          HttpConnectionOptions(
            accessTokenFactory: () async => authProvider.accessToken!,
            transport: HttpTransportType.webSockets,
            customHeaders: await authProvider.getCustomHeaders(),
          ),
        )
        .withAutomaticReconnect()
        .build();

    final originalUrl = connection.baseUrl;
    final uriBuilder = Uri.parse(originalUrl).replace(queryParameters: {
      'access_token': authProvider.accessToken!,
      'refresh_token': authProvider.refreshToken!,
    });
    connection.baseUrl = uriBuilder.toString();

    return connection;
  }

  void _setupConnectionHandlers() {
    _hubConnection?.onclose((error) async {
      print('Connection closed: $error');
      await reconnect();
    });
  }

  Future<void> _handleConnectionError(dynamic e) async {
    print('Error connecting to SignalR hub: $e');
    _isConnected = false;
    if (e.toString().contains('401')) {
      print('Authentication failed: Invalid access token');
    } else {
      print('Error connecting to SignalR hub: $e');
      await reconnect();
    }
  }

  Future<void> reconnect() async {
    const int maxAttempts = 5;
    int attempt = 0;
    while (attempt < maxAttempts) {
      attempt++;
      print('Reconnecting attempt $attempt...');
      try {
        await _hubConnection?.start();
        print('Reconnected to SignalR hub');
        _isConnected = true;
        return;
      } catch (e) {
        print('Reconnection attempt $attempt failed: $e');
        await Future.delayed(const Duration(seconds: 2));
      }
    }
    print('Failed to reconnect after $maxAttempts attempts');
  }

  void _handleAuthChange() {
    if (!authProvider.isAuth && _isConnected) {
      stop();
    } else if (authProvider.isAuth && !_isConnected) {
      initialize();
    }
  }

  Future<void> stop() async {
    await _hubConnection?.stop();
    _isConnected = false;
    print('Disconnected from SignalR hub');
  }

  HubConnection? get connection => _hubConnection;

  void on(String methodName, Function(List<dynamic>?) handler) {
    _hubConnection?.on(methodName, handler);
  }

  Future<dynamic> invoke(String methodName, {List<dynamic>? args}) async {
    if (_hubConnection == null) {
      throw Exception('HubConnection is not initialized');
    }
    return await _hubConnection!.invoke(methodName, args: args);
  }
}
