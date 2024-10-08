
# !pip install flask

from flask import Flask, request, jsonify
import pickle
import pandas as pd

app = Flask(__name__)

# Load mô hình KNN và ma trận tương tác người dùng
model_knn = pickle.load(open('knn_model.pkl', 'rb'))
user_interaction_matrix = pd.read_csv('user_interaction_matrix.csv', index_col='UserId')


def get_recommendations_knn(user_id, model_knn, user_interaction_matrix, top_n=20):
    """
    Lấy danh sách các user được đề xuất cho một user cụ thể sử dụng KNN.
    """
    try:
        user_interactions = user_interaction_matrix.loc[[user_id]]

        if user_interactions.empty:
            return []

        distances, indices = model_knn.kneighbors(user_interactions, n_neighbors=top_n + 1)
        recommended_user_indices = indices.flatten()[1:]
        recommended_user_ids = user_interaction_matrix.index[recommended_user_indices].tolist()
        return recommended_user_ids
    except KeyError:
        return []


@app.route('/recommend', methods=['GET'])
def recommend_users():
    user_id = request.args.get('user_id')
    if user_id:
        recommended_users = get_recommendations_knn(user_id, model_knn, user_interaction_matrix)
        return jsonify({'recommendations': recommended_users})
    else:
        return jsonify({'error': 'Missing user_id parameter'}), 400


if __name__ == '__main__':
    app.run(debug=True, host='0.0.0.0', port=5000)