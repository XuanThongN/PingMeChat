# -*- coding: utf-8 -*-
"""PingMeChat-Friend_Recommendation.ipynb

Automatically generated by Colab.

Original file is located at
    https://colab.research.google.com/drive/1V_RGKuCvthyr8mFKn8mR4_1Ou5f-Lxvd
"""

# pip install faker

import csv
import random
from datetime import datetime, timedelta
from faker import Faker
import uuid
import json

# Định nghĩa danh sách các sở thích
interests = ["Thể thao", "Âm nhạc", "Đọc sách", "Du lịch", "Phim ảnh", "Ẩm thực", "Công nghệ", "Thời trang", "Nghệ thuật", "Game"]

# Khởi tạo Faker với locale tiếng Việt
fake = Faker('vi_VN')
# Đầu số hợp lệ của các nhà mạng Việt Nam
valid_prefixes = ['03', '07', '08', '09']

# Hàm tạo số điện thoại hợp lệ
def generate_vn_phone_number():
    prefix = random.choice(valid_prefixes)  # Chọn đầu số ngẫu nhiên
    suffix = ''.join([str(random.randint(0, 9)) for _ in range(8)])  # Tạo phần còn lại của số điện thoại (8 chữ số)
    return prefix + suffix

# Tạo danh sách users
users = []
for i in range(1000):
    user = {
        'Id': str(uuid.uuid4()),
        'Username': fake.user_name()+str(i),
        'FullName': fake.name(),
        'Email': fake.email(),
        'PhoneNumber': generate_vn_phone_number(),
        'DateOfBirth': fake.date_of_birth(minimum_age=18, maximum_age=70).strftime('%Y-%m-%d'),
        'Gender': random.choice([True, False]), # Changed to boolean values
        'Interests': json.dumps(random.sample(interests, random.randint(1, 5))), # Convert to JSON string
        'CreatedDate': fake.date_between(start_date='-2y', end_date='today').strftime('%Y-%m-%d')
    }
    users.append(user)

from google.colab import drive
drive.mount('/content/drive')

# Tạo danh sách contacts
from datetime import datetime

contacts = []
# Duyệt qua 100 người đầu tiên
for user in users[:100]:
    num_contacts = random.randint(5, 10)
    for _ in range(num_contacts):
        contact = random.choice(users)
        if contact['Id'] != user['Id']:
            # Chuyển đổi user['JoinDate'] thành đối tượng datetime
            join_date = datetime.strptime(user['CreatedDate'], '%Y-%m-%d').date()
            contact_entry = {
                'Id': str(uuid.uuid4()),
                'UserId': user['Id'],
                'ContactUserId': contact['Id'],
                'CreatedDate': fake.date_between(start_date=join_date, end_date='today').strftime('%Y-%m-%d'),
                'Status': random.choice([1,0])
            }
            contacts.append(contact_entry)
#In ra số lượng phần tử
print(len(contacts))

# Tạo danh sách chats
chats = []
chat_members = []
messages = []
for i in range(100):  # Tạo 2000 cuộc trò chuyện
    is_group = random.choice([True, False])
    chat = {
        #id được tạo bằng GUId
        'Id': str(uuid.uuid4()),
        'IsGroup': is_group,
        'CreatedDate': fake.date_between(start_date='-1y', end_date='today').strftime('%Y-%m-%d')
    }
    chats.append(chat)

    # Thêm thành viên vào cuộc trò chuyện
    num_members = 2 if is_group == False else random.randint(3, 10)
    members = random.sample(users, num_members)
    for member in members:
        join_date = datetime.strptime(chat['CreatedDate'], '%Y-%m-%d').date()
        chat_member = {
            'Id': str(uuid.uuid4()),
            'ChatId': chat['Id'],
            'UserId': member['Id'],
            'IsAdmin': True if is_group and member == members[0] else False, # Người đầu tiên trong members được chọn làm admin cho group chat
            'JoinAt': fake.date_between(start_date=join_date, end_date='today').strftime('%Y-%m-%d')
        }
        chat_members.append(chat_member)

    # Tạo tin nhắn cho cuộc trò chuyện
    num_messages = random.randint(10, 50)
    for _ in range(num_messages):
        sender = random.choice(members)
        start_date = datetime.strptime(chat['CreatedDate'], '%Y-%m-%d')
        message = {
            'Id': str(uuid.uuid4()),
            'ChatId': chat['Id'],
            'SenderId': sender['Id'],
            'Content': fake.text(max_nb_chars=200),
            'SentAt': fake.date_time_between(start_date=start_date, end_date='now').strftime('%Y-%m-%d %H:%M:%S')
        }
        messages.append(message)
# In ra số lượng đoạn chat và số lượng messages
print('Số lượng đoạn chat: '+str(len(chats)))
print('Số lượng tin nhắn: '+str(len(messages)))

# prompt: Lưu tất cả dữ liệu trên vào db postgre
# nhưng nhớ tạo transaction để đảm bảo tính nhất quán
import psycopg2
import json
import uuid

# Danh sách dữ liệu giả lập (giả sử bạn đã có các danh sách dữ liệu 'users', 'contacts', 'chats', 'chat_members', 'messages')

# Hàm để tạo kết nối PostgreSQL
def connect_to_db():
    return psycopg2.connect(
        host="ep-summer-sky-a1ez58e6.ap-southeast-1.aws.neon.tech",
        database="ping_me_chat_database",
        user="ping_me_chat_database_owner",
        password="8K1slSEmohAv"
    )

# Chèn dữ liệu vào bảng "Accounts"
# Hàm chèn dữ liệu hàng loạt vào bảng
def batch_insert(cur, table_name, columns, data):
    """
    Chèn hàng loạt dữ liệu vào một bảng trong cơ sở dữ liệu.

    Args:
    - cur: Đối tượng cursor của cơ sở dữ liệu.
    - table_name: Tên bảng cần chèn dữ liệu.
    - columns: Danh sách các cột tương ứng với dữ liệu cần chèn.
    - data: Danh sách các giá trị cần chèn, mỗi phần tử là một tuple tương ứng với một bản ghi.

    """
    # Tạo câu lệnh INSERT động từ tên bảng và các cột
    columns_str = ', '.join([f'"{col}"' for col in columns])
    placeholders = ', '.join(['%s'] * len(columns))
    insert_query = f'INSERT INTO "{table_name}" ({columns_str}) VALUES ({placeholders})'

    # Sử dụng executemany để chèn nhiều bản ghi cùng lúc
    cur.executemany(insert_query, data)

# Hàm chèn dữ liệu vào bảng "Accounts"
def insert_accounts(cur, users):
    default_password = "123"
    is_locked = False
    is_deleted = False
    is_verified = True

    data = [
        (
            user['Id'], user['Username'], default_password, user['FullName'], user['Email'], user['PhoneNumber'],
            user['DateOfBirth'], user['Gender'], user['Interests'], user['CreatedDate'],
            is_locked, is_deleted, is_verified
        )
        for user in users
    ]
    # Gọi hàm batch_insert với dữ liệu đã chuẩn bị
    batch_insert(
        cur,
        table_name="Accounts",
        columns=["Id", "UserName", "Password", "FullName", "Email", "PhoneNumber", "DateOfBirth", "Gender", "Interests", "CreatedDate", "IsLocked", "IsDeleted", "IsVerified"],
        data=data
    )

# Hàm chèn dữ liệu vào bảng "Contacts"
def insert_contacts(cur, contacts):
    is_deleted = False

    data = [
        (
            contact['Id'], contact['UserId'], contact['ContactUserId'], contact['CreatedDate'],
            contact['CreatedDate'], contact['Status'], is_deleted
        )
        for contact in contacts
    ]
    batch_insert(
        cur,
        table_name="Contacts",
        columns=["Id", "UserId", "ContactUserId", "CreatedDate", "AddedAt", "Status", "IsDeleted"],
        data=data
    )

# Hàm chèn dữ liệu vào bảng "Chats"
def insert_chats(cur, chats):
    is_deleted = False

    data = [
        (
            chat['Id'], chat['IsGroup'], chat['CreatedDate'], is_deleted
        )
        for chat in chats
    ]
    batch_insert(
        cur,
        table_name="Chats",
        columns=["Id", "IsGroup", "CreatedDate", "IsDeleted"],
        data=data
    )

# Hàm chèn dữ liệu vào bảng "UserChats"
def insert_user_chats(cur, chat_members):
    is_deleted = False

    data = [
        (
            chat_member['Id'], chat_member['ChatId'], chat_member['UserId'],
            chat_member['JoinAt'], chat_member['IsAdmin'], is_deleted
        )
        for chat_member in chat_members
    ]
    batch_insert(
        cur,
        table_name="UserChats",
        columns=["Id", "ChatId", "UserId", "JoinAt", "IsAdmin", "IsDeleted"],
        data=data
    )

# Hàm chèn dữ liệu vào bảng "Messages"
def insert_messages(cur, messages):
    is_deleted = False

    data = [
        (
            message['Id'], message['ChatId'], message['SenderId'],
            message['Content'], message['SentAt'], is_deleted
        )
        for message in messages
    ]
    batch_insert(
        cur,
        table_name="Messages",
        columns=["Id", "ChatId", "SenderId", "Content", "SentAt", "IsDeleted"],
        data=data
    )

# Kết nối đến cơ sở dữ liệu và thực hiện transaction
try:
    conn = connect_to_db()
    cur = conn.cursor()

    # Bắt đầu transaction
    conn.autocommit = False

    try:
        # Chèn dữ liệu vào từng bảng
        # insert_accounts(cur, users)
        # insert_contacts(cur, contacts)
        # insert_chats(cur, chats)
        # insert_user_chats(cur, chat_members)
        insert_messages(cur, messages)

        # Commit transaction
        conn.commit()
        print("Dữ liệu đã được lưu vào cơ sở dữ liệu PostgreSQL thành công!")

    except Exception as e:
        # Rollback transaction nếu có lỗi
        conn.rollback()
        print("Có lỗi xảy ra khi lưu dữ liệu vào cơ sở dữ liệu PostgreSQL:", e)

except Exception as e:
    print("Không thể kết nối đến cơ sở dữ liệu PostgreSQL:", e)

finally:
    if conn:
        cur.close()
        conn.close()

# prompt: Lấy dữ liệu từ db và tạo thành các mảng
# tôi muốn ở accounts tôi chỉ muốn lấy Id, UserName, Interests. chats thì tôi chỉ muốn lấy Id, IsGroup, CreatedDate. contacts thì tôi chỉ muốn lấy Id, UserId, ContactUserId, Status = 1. messages thì lấy Id, ChatId, SenderId. userchats thì lấy Id, ChatId, UserId.

import psycopg2
import json

# Hàm để tạo kết nối PostgreSQL
def connect_to_db():
    return psycopg2.connect(
        host="ep-summer-sky-a1ez58e6.ap-southeast-1.aws.neon.tech",
        database="ping_me_chat_database",
        user="ping_me_chat_database_owner",
        password="8K1slSEmohAv"
    )

try:
    conn = connect_to_db()
    cur = conn.cursor()

    # Lấy dữ liệu từ bảng Accounts
    cur.execute("""SELECT "Id", "UserName", "DateOfBirth", "Gender" FROM "Accounts" """)
    accounts_data = cur.fetchall()
    accounts = []
    for row in accounts_data:
        account = {
            'Id': row[0],
            'UserName': row[1],
            'DateOfBirth': row[2],
            'Gender': row[3]
        }
        accounts.append(account)

    # Lấy dữ liệu từ bảng Chats
    cur.execute("""SELECT "Id", "IsGroup", "CreatedDate" FROM "Chats" """)
    chats_data = cur.fetchall()
    chats = []
    for row in chats_data:
        chat = {
            'Id': row[0],
            'IsGroup': row[1],
            'CreatedDate': row[2]
        }
        chats.append(chat)

    # Lấy dữ liệu từ bảng Contacts
    cur.execute("""SELECT "Id", "UserId", "ContactUserId", "Status" FROM "Contacts" WHERE "Status" = 1""")
    contacts_data = cur.fetchall()
    contacts = []
    for row in contacts_data:
        contact = {
            'Id': row[0],
            'UserId': row[1],
            'ContactUserId': row[2],
            'Status': row[3]
        }
        contacts.append(contact)

    # Lấy dữ liệu từ bảng Messages
    cur.execute("""SELECT "Id", "ChatId", "SenderId", "SentAt" FROM "Messages" """)
    messages_data = cur.fetchall()
    messages = []
    for row in messages_data:
        message = {
            'Id': row[0],
            'ChatId': row[1],
            'SenderId': row[2]
        }
        if row[3] is not None:
            message['SentAt'] = row[3].strftime('%Y-%m-%d %H:%M:%S')
        messages.append(message)

    # Lấy dữ liệu từ bảng UserChats
    cur.execute("""SELECT "Id", "ChatId", "UserId" FROM "UserChats" """)
    userchats_data = cur.fetchall()
    userchats = []
    for row in userchats_data:
        userchat = {
            'Id': row[0],
            'ChatId': row[1],
            'UserId': row[2]
        }
        userchats.append(userchat)

    print("Dữ liệu từ các bảng đã được lấy thành công!")
    print("Số lượng accounts:", len(accounts))
    print("Số lượng chats:", len(chats))
    print("Số lượng contacts:", len(contacts))
    print("Số lượng messages:", len(messages))
    print("Số lượng userchats:", len(userchats))


except Exception as e:
    print("Có lỗi xảy ra khi lấy dữ liệu từ cơ sở dữ liệu:", e)

finally:
    if conn:
        cur.close()
        conn.close()

# !pip install --upgrade scikit-learn



import pandas as pd
import numpy as np
from sklearn.preprocessing import MinMaxScaler
from sklearn.model_selection import train_test_split

# 1. Tạo DataFrame từ dữ liệu đã trích xuất
accounts_df = pd.DataFrame(accounts)
contacts_df = pd.DataFrame(contacts)
messages_df = pd.DataFrame(messages)
userchats_df = pd.DataFrame(userchats)

# 2. Xử lý dữ liệu thiếu và tạo ma trận tương tác
def create_interaction_matrix(contacts_df, messages_df, userchats_df):
    # Tạo ma trận tương tác từ danh bạ
    contact_interactions = contacts_df.groupby(['UserId', 'ContactUserId']).size().reset_index(name='ContactCount')

    # Tạo ma trận tương tác từ tin nhắn
    message_interactions = messages_df.merge(userchats_df, left_on='ChatId', right_on='ChatId')
    message_interactions = message_interactions[message_interactions['SenderId'] != message_interactions['UserId']]
    message_interactions = message_interactions.groupby(['UserId', 'SenderId']).size().reset_index(name='MessageCount')

    # Kết hợp tương tác từ danh bạ và tin nhắn
    interaction_matrix = pd.merge(contact_interactions, message_interactions,
                                  left_on=['UserId', 'ContactUserId'],
                                  right_on=['UserId', 'SenderId'],
                                  how='outer')

    interaction_matrix['InteractionCount'] = interaction_matrix['ContactCount'].fillna(0) + interaction_matrix['MessageCount'].fillna(0)
    interaction_matrix = interaction_matrix[['UserId', 'ContactUserId', 'InteractionCount']]
    interaction_matrix = interaction_matrix.fillna(0)

    return interaction_matrix

interaction_matrix = create_interaction_matrix(contacts_df, messages_df, userchats_df)

# 3. Chuẩn hóa dữ liệu
scaler = MinMaxScaler()
interaction_matrix['InteractionCount'] = scaler.fit_transform(interaction_matrix[['InteractionCount']])

# 4. Tạo ma trận tương tác người dùng
# Thay đổi pivot thành pivot_table và sử dụng aggfunc để xử lý các giá trị trùng lặp
user_interaction_matrix = interaction_matrix.pivot_table(
    index='UserId',
    columns='ContactUserId',
    values='InteractionCount',
    aggfunc='sum'  # Hoặc một hàm tổng hợp khác phù hợp với dữ liệu của bạn
).fillna(0)

# 5. Chia dữ liệu thành tập train và test
train_matrix, test_matrix = train_test_split(interaction_matrix, test_size=0.2, random_state=42)

print("Kích thước ma trận tương tác:", interaction_matrix.shape)
print("Kích thước tập train:", train_matrix.shape)
print("Kích thước tập test:", test_matrix.shape)

# Lưu các DataFrame đã xử lý để sử dụng cho bước tiếp theo
interaction_matrix.to_csv('interaction_matrix.csv', index=False)
user_interaction_matrix.to_csv('user_interaction_matrix.csv')
train_matrix.to_csv('train_matrix.csv', index=False)
test_matrix.to_csv('test_matrix.csv', index=False)

print("Quá trình tiền xử lý dữ liệu đã hoàn tất.")

# prompt: Tiếp theo hãy giúp tôi các bước tiếp theo
# Để train mô hình có thể lựa chọn thuật toán phù hợp nhưng dễ tiếp cận
# Có thể sử dụng Knn

import pandas as pd
from sklearn.neighbors import NearestNeighbors
from sklearn.model_selection import train_test_split

# Load dữ liệu từ file CSV
user_interaction_matrix = pd.read_csv('user_interaction_matrix.csv', index_col='UserId')

# 1. Huấn luyện mô hình KNN
model_knn = NearestNeighbors(metric='cosine', algorithm='brute', n_neighbors=5)
model_knn.fit(user_interaction_matrix)


def get_recommendations_knn(user_id, model_knn, user_interaction_matrix, top_n=5):
    """
    Lấy danh sách các user được đề xuất cho một user cụ thể sử dụng KNN.

    Args:
      user_id: ID của user cần đề xuất.
      model_knn: Mô hình KNN đã được huấn luyện.
      user_interaction_matrix: Ma trận tương tác người dùng.
      top_n: Số lượng user được đề xuất.

    Returns:
      Danh sách các user được đề xuất (ID).
    """
    try:
        user_interactions = user_interaction_matrix.loc[[user_id]]

        if user_interactions.empty:
            return []  # Trả về danh sách rỗng nếu user không tồn tại trong ma trận

        distances, indices = model_knn.kneighbors(user_interactions, n_neighbors=top_n + 1)
        recommended_user_indices = indices.flatten()[1:]  # Loại bỏ user hiện tại
        recommended_user_ids = user_interaction_matrix.index[recommended_user_indices].tolist()
        return recommended_user_ids
    except KeyError:
        return []  # Trả về danh sách rỗng nếu user không tồn tại trong ma trận

# Ví dụ: Lấy danh sách đề xuất cho user có ID là 'user_id_example'
user_id_example = user_interaction_matrix.index[0]
recommended_users = get_recommendations_knn(user_id_example, model_knn, user_interaction_matrix)

print(f"Danh sách user được đề xuất cho user {user_id_example}: {recommended_users}")


# 2. Đánh giá mô hình
# Có thể sử dụng các phương pháp như Precision, Recall, F1-score hoặc AUC
# để đánh giá hiệu quả của mô hình, tuy nhiên cần có dữ liệu nhãn (ground truth)
# để so sánh kết quả dự đoán với dữ liệu thực tế.

# Ví dụ về đánh giá mô hình với Precision và Recall (nếu có dữ liệu nhãn)
# ...

# 3. Tối ưu hóa tham số (nếu cần)
# Có thể thử nghiệm với các giá trị khác nhau cho tham số n_neighbors,
# metric, algorithm,... để tìm ra tham số tối ưu cho mô hình.

# 4. Triển khai mô hình
# Khi mô hình đã được huấn luyện và đánh giá hiệu quả, có thể triển khai
# nó để đề xuất user cho người dùng trong ứng dụng thực tế.

# prompt: Lưu lại mô hình

import pickle

# Lưu mô hình KNN vào file
filename = 'knn_model.pkl'
pickle.dump(model_knn, open(filename, 'wb'))

print(f"Mô hình KNN đã được lưu vào file: {filename}")

# prompt: Sử dụng mô hình. Với input là userId và output là list userId của những người được gợi ý

import pickle
import pandas as pd

# Load mô hình KNN từ file
filename = 'knn_model.pkl'
model_knn = pickle.load(open(filename, 'rb'))

# Load ma trận tương tác người dùng
user_interaction_matrix = pd.read_csv('user_interaction_matrix.csv', index_col='UserId')


def get_recommendations_knn(user_id, model_knn, user_interaction_matrix, top_n=5):
    """
    Lấy danh sách các user được đề xuất cho một user cụ thể sử dụng KNN.

    Args:
      user_id: ID của user cần đề xuất.
      model_knn: Mô hình KNN đã được huấn luyện.
      user_interaction_matrix: Ma trận tương tác người dùng.
      top_n: Số lượng user được đề xuất.

    Returns:
      Danh sách các user được đề xuất (ID).
    """
    try:
        user_interactions = user_interaction_matrix.loc[[user_id]]

        if user_interactions.empty:
            return []  # Trả về danh sách rỗng nếu user không tồn tại trong ma trận

        distances, indices = model_knn.kneighbors(user_interactions, n_neighbors=top_n + 1)
        recommended_user_indices = indices.flatten()[1:]  # Loại bỏ user hiện tại
        recommended_user_ids = user_interaction_matrix.index[recommended_user_indices].tolist()
        return recommended_user_ids
    except KeyError:
        return []  # Trả về danh sách rỗng nếu user không tồn tại trong ma trận


# Ví dụ: Lấy danh sách đề xuất cho user có ID là 'user_id_input'
user_id_input = 'your_user_id'  # Thay 'your_user_id' bằng ID của user bạn muốn đề xuất
recommended_users = get_recommendations_knn(user_id_input, model_knn, user_interaction_matrix)

print(f"Danh sách user được đề xuất cho user {user_id_input}: {recommended_users}")