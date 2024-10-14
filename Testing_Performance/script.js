import http from 'k6/http';
import { check, sleep } from 'k6';
import { SharedArray } from 'k6/data';
import { Rate } from 'k6/metrics';
import ws from 'k6/ws';  // Import thư viện WebSocket của K6

// Định nghĩa các metrics tùy chỉnh
const errorRate = new Rate('errors');

// Tải dữ liệu người dùng và chat từ file JSON
const users = new SharedArray('users', function () {
    return JSON.parse(open('./users.json')).users;
});

const chats = new SharedArray('chats', function () {
    return JSON.parse(open('./chats.json')).chats;
});

// Cấu hình kịch bản kiểm thử
export const options = {
    scenarios: {
        constant_request_rate: {
            executor: 'constant-arrival-rate',
            rate: 1000, // 1000 người dùng
            timeUnit: '1s',
            duration: '5m',
            preAllocatedVUs: 1000,
            maxVUs: 2000,
        },
    },
    thresholds: {
        'http_req_duration': ['p(95)<500'], // 95% của requests phải hoàn thành trong 500ms
        'http_req_failed': ['rate<0.01'], // Tỉ lệ lỗi phải nhỏ hơn 1%
    },
};

// Hàm chính để thực hiện kiểm thử
export default function () {
    const user = users[Math.floor(Math.random() * users.length)];
    const chat = chats[Math.floor(Math.random() * chats.length)];

    // Đăng nhập và lấy token
    const loginRes = http.post(`${__ENV.API_URL}/api/auth/login`, {
        email: user.email,
        password: user.password,
    });

    check(loginRes, {
        'logged in successfully': (resp) => resp.json('token') !== '',
    }) || errorRate.add(1);

    const token = loginRes.json('token');

    // Kiểm tra kết nối SignalR: Thương lượng
    const negotiateRes = http.get(`${__ENV.API_URL}/chat-hub/negotiate`, {
        headers: {
            'Authorization': `Bearer ${token}`,
        },
    });

    check(negotiateRes, {
        'SignalR negotiation successful': (resp) => resp.status === 200,
    }) || errorRate.add(1);

    const connectionInfo = negotiateRes.json();
    const wsUrl = `${connectionInfo.url}?id=${connectionInfo.connectionId}`;

    // Kết nối tới WebSocket (SignalR sử dụng)
    const res = ws.connect(wsUrl, null, function (socket) {
        socket.on('open', function () {
            console.log('Connected to SignalR hub');

            // Gửi tin nhắn qua SignalR
            for (let i = 0; i < 10; i++) {
                const messagePayload = {
                    type: 1, // loại message của SignalR (Invocation)
                    target: 'SendMessage',
                    arguments: [chat.id, `Test message ${i}`],
                };

                socket.send(JSON.stringify(messagePayload));

                // Nhận phản hồi từ SignalR (giả lập nhận phản hồi)
                socket.on('message', function (msg) {
                    console.log('Received message: ', msg);
                    check(msg, {
                        'message received successfully': (m) => m !== '',
                    }) || errorRate.add(1);
                });

                sleep(6); // Đợi 6 giây trước khi gửi tin nhắn tiếp theo
            }
        });

        socket.on('close', function () {
            console.log('Disconnected from SignalR hub');
        });

        socket.on('error', function (e) {
            console.log('Error: ', e);
            errorRate.add(1);
        });
    });

    check(res, { 'SignalR connection established': (r) => r && r.status === 101 });

    sleep(1); // Đợi 1 giây trước khi kết thúc phiên
}
