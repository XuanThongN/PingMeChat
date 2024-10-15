import http from 'k6/http';
import { check, sleep } from 'k6';
import { SharedArray } from 'k6/data';
import { Rate } from 'k6/metrics';
import ws from 'k6/ws';

// Định nghĩa các metrics tùy chỉnh
const errorRate = new Rate('errors');

// Tải dữ liệu người dùng và chat từ file JSON
const usersData = new SharedArray('users', function () {
    return JSON.parse(open('./user_chats.json'));
});

// Tạo đối tượng chứa thông tin người dùng và danh sách chat
const userChatMap = {};
usersData.forEach(user => {
    userChatMap[user.UserId] = {
        userName: user.UserName,
        chatIds: user.Chats
    };
});

// Lấy danh sách userIds
const userIds = Object.keys(userChatMap);

// Cấu hình các kịch bản kiểm thử
export const options = {
    scenarios: {
        '10_users': {
            executor: 'per-vu-iterations',
            vus: 10,
            iterations: 1,
            maxDuration: '1m',
        },
        '100_users': {
            executor: 'constant-vus',
            vus: 100,
            duration: '1m',
        },
    },
};
// Mảng các câu mẫu để tạo nội dung tin nhắn tự nhiên hơn
const messages = [
    "Hello, how are you?",
    "What's up?",
    "Did you see the game last night?",
    "I'm working on a new project.",
    "Let's catch up soon!",
    "Have you read any good books lately?",
    "I'm planning a trip next month.",
    "What's your favorite movie?",
    "Do you have any hobbies?",
    "How's the weather today?"
];
// Hàm để tạo nội dung tin nhắn ngẫu nhiên
function getRandomMessage() {
    const index = Math.floor(Math.random() * messages.length);
    return messages[index];
}
// Hàm chính để thực hiện kiểm thử
export default function () {
    const userId = userIds[__VU - 1 % userIds.length];
    const user = userChatMap[userId];

    // Đăng nhập và lấy token
    const loginRes = http.post(`${__ENV.API_URL}/api/auth/login`, JSON.stringify({
        username: user.userName,
        password: 'Xuanthongn@123',
    }), {
        headers: { 'Content-Type': 'application/json' },
    });

    if (loginRes.status !== 200) {
        console.log(`Login request failed for user ${user.userName}:`, loginRes.status, loginRes.body);
        errorRate.add(1);
        return;
    }

    check(loginRes, {
        'logged in successfully': (resp) => resp.json('result') !== undefined && resp.json('result.accessToken') !== '',
    }) || errorRate.add(1);

    const result = loginRes.json('result');
    if (result) {
        const accessToken = result.accessToken;
        const refreshToken = result.refreshToken;

        // Kiểm tra kết nối SignalR: Thương lượng
        const negotiateRes = http.post(`${__ENV.API_URL}/chatHub/negotiate`, null, {
            headers: {
                'Authorization': `Bearer ${accessToken}`,
                'RefreshToken': refreshToken,
                'Content-Type': 'application/json'
            },
        });

        check(negotiateRes, {
            'SignalR negotiation successful': (resp) => resp.status === 200,
        }) || errorRate.add(1);

        let connectionInfo;
        try {
            connectionInfo = negotiateRes.json();
        } catch (e) {
            console.log(`Failed to parse negotiate response for user ${user.userName}:`, e);
            errorRate.add(1);
            return;
        }

        // Thêm access_token và refresh_token vào URL WebSocket
        connectionInfo.url = __ENV.API_URL.replace('http', 'ws') + '/chatHub';
        const wsUrl = `${connectionInfo.url}?id=${connectionInfo.connectionId}&access_token=${accessToken}&refresh_token=${refreshToken}`;

        // Kết nối tới WebSocket (SignalR sử dụng)
        const res = ws.connect(wsUrl, null, function (socket) {
            socket.on('open', function () {
                console.log(`User ${user.userName} connected to SignalR hub`);

                // Gửi handshake tới SignalR sau khi mở kết nối WebSocket
                const handshakePayload = { protocol: 'json', version: 1 };
                socket.send(JSON.stringify(handshakePayload) + String.fromCharCode(30));
                console.log('Handshake sent');

                // Gửi tin nhắn qua SignalR
                for (let i = 0; i < 2; i++) {
                    const chatId = user.chatIds[i % user.chatIds.length];
                    const messagePayload = {
                        tempId: `temp-${userId}-${i}-${Date.now()}`,
                        chatId: chatId,
                        content: getRandomMessage(),
                        attachments: [],
                    };

                    const signalRPayload = {
                        type: 1,
                        target: 'SendMessage',
                        arguments: [messagePayload],
                    };

                    const signalRMessage = JSON.stringify(signalRPayload) + String.fromCharCode(30);
                    socket.send(signalRMessage);
                    console.log(`User ${user.userName} sending message:`, messagePayload);

                    sleep(1);
                }
            });

            socket.on('message', function (msg) {
                console.log(`User ${user.userName} received message:`, msg);
            });

            socket.on('close', function () {
                console.log(`User ${user.userName} disconnected from SignalR hub`);
            });

            socket.on('error', function (e) {
                console.log(`Error for user ${user.userName}:`, e);
                errorRate.add(1);
            });
        });

        check(res, { 'SignalR connection established': (r) => r && r.status === 101 });

        sleep(5); // Đợi 5 giây trước khi kết thúc phiên
    } else {
        console.log(`Login failed for user ${user.userName}:`, loginRes.body);
        errorRate.add(1);
    }
}