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
        ramp_up: {
            executor: 'ramping-vus',
            startVUs: 0,
            stages: [
                { duration: '2m', target: 500 },  // Tăng dần lên 500 user trong 2 phút
                { duration: '5m', target: 500 },  // Giữ ở 500 user trong 5 phút
                { duration: '2m', target: 1000 }, // Tăng lên 1000 user trong 2 phút tiếp theo
                { duration: '5m', target: 1000 }, // Giữ ở 1000 user trong 5 phút
                { duration: '2m', target: 0 },    // Giảm xuống 0 user trong 2 phút cuối
            ],
            gracefulRampDown: '30s',
        },
    },
};

// Mảng các câu mẫu để tạo nội dung tin nhắn tự nhiên hơn
const messages = [
    "Ê, dạo này sao rồi?",
    "Có gì hay ho không?",
    "Hôm qua xem bóng đá có vui không?",
    "Mình đang lấn sân sang một dự án mới nè.",
    "Đi uống cà phê sớm đi nhé!",
    "Đọc sách gì hay không? Chia sẻ đi!",
    "Tháng sau mình đi chơi đây, có ai đi chung không?",
    "Phim nào đang hot hả bạn?",
    "Bạn có sở thích gì quái quái không?",
    "Hôm nay thời tiết khá ổn!",
    "Ăn gì chưa? Đói quá!",
    "Cuối tuần này có kế hoạch gì vui không?",
    "Có tin gì vui không, hóng quá!",
    "Chỉ mình chỗ ăn ngon đi!",
    "Xem phim gì gần đây chưa? Có hay không?",
    "Năm tới có dự định gì đặc biệt không?",
    "Ê, làm ván PUBG không?",
    "Chán quá, chat chơi tí!",
    "Mày ơi, tao vừa thấy clip hài vcl",
    "Đi nhậu không? Tao đãi",
    "Ê, có biết tin gì hot không?",
    "Mua đồ online rồi bị lừa, tức quá!",
    "Hôm nay tao đẹp trai vcl, khen đi!",
    "Mày có thấy crush tao xinh không?",
    "Ê, tao vừa trúng số nè!",
    "Đi phượt cuối tuần không?",
    "Tao vừa bị sếp chửi, buồn vl",
    "Mày ơi, tao vừa thất tình",
    "Có gì vui không? Chán quá!",
    "Ê, mày biết bài hát nào hay không?",
    "Tao vừa mua điện thoại mới nè",
    "Đi uống bia không? Tao đãi",
    "Mày ơi, tao vừa bị đuổi việc",
    "Ê, có muốn làm ăn với tao không?",
    "Tao vừa thấy người yêu cũ, awkward vl",
    "Mày có tin vào tình yêu online không?",
    "Ê, tao vừa học được món mới, qua ăn không?",
    "Chơi game gì hay không? Chỉ tao với",
    "Tao vừa bị lừa tiền, tức quá!",
    "Mày ơi, tao sắp cưới rồi!"
];

// Hàm để tạo nội dung tin nhắn ngẫu nhiên
function getRandomMessage() {
    const index = Math.floor(Math.random() * messages.length);
    return messages[index];
}

// Hàm chính để thực hiện kiểm thử
export default function () {
    const userId = userIds[__VU % userIds.length];
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