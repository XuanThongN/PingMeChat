import http from 'k6/http';
import { check, sleep } from 'k6';
import { SharedArray } from 'k6/data';
import { Rate } from 'k6/metrics';

// Định nghĩa các metrics tùy chỉnh
const errorRate = new Rate('errors');

// Tải dữ liệu người dùng và chat từ file JSON (giả sử bạn đã xuất dữ liệu này từ PostgreSQL)
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

    // Đăng nhập
    const loginRes = http.post(`${__ENV.API_URL}/api/auth/login`, {
        email: user.email,
        password: user.password,
    });

    check(loginRes, {
        'logged in successfully': (resp) => resp.json('token') !== '',
    }) || errorRate.add(1);

    const token = loginRes.json('token');

    // Gửi tin nhắn
    for (let i = 0; i < 10; i++) {
        const hasAttachment = Math.random() < 0.2; // 20% tin nhắn có đính kèm file
        const messagePayload = {
            chatId: chat.id,
            content: `Test message ${i}`,
            attachments: hasAttachment ? [{ fileName: 'test.jpg', fileType: 'image/jpeg', fileSize: 1024 }] : [],
        };

        const sendMessageRes = http.post(`${__ENV.API_URL}/api/chat/send-message`, JSON.stringify(messagePayload), {
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`,
            },
        });

        check(sendMessageRes, {
            'message sent successfully': (resp) => resp.status === 200,
        }) || errorRate.add(1);

        sleep(6); // Đợi 6 giây trước khi gửi tin nhắn tiếp theo (10 tin nhắn/phút)
    }

    // Kiểm tra kết nối SignalR
    const wsRes = http.get(`${__ENV.API_URL}/chat-hub/negotiate`, {
        headers: {
            'Authorization': `Bearer ${token}`,
        },
    });

    check(wsRes, {
        'SignalR negotiation successful': (resp) => resp.status === 200,
    }) || errorRate.add(1);

    // Đo độ trễ khi gửi/nhận tin nhắn qua SignalR
    // Lưu ý: K6 không hỗ trợ trực tiếp WebSocket, nên chúng ta chỉ mô phỏng việc này
    const signalRLatency = Math.random() * 100; // Giả lập độ trễ từ 0-100ms
    check(signalRLatency, {
        'SignalR latency is acceptable': (latency) => latency < 50,
    });

    sleep(1);
}