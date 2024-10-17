# Sử dụng image chính thức của K6
FROM loadimpact/k6:latest

# Sao chép file script.js và user_chats.json vào container
COPY script.js /script.js
COPY user_chats.json /user_chats.json

# Đặt thư mục làm việc
WORKDIR /

# Chạy K6 với script.js
ENTRYPOINT ["k6", "run", "/script.js"]