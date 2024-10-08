from flask import Flask, request, jsonify
import pickle

app = Flask(__name__)

# Load the KNN model
with open('knn_model.pkl', 'rb') as model_file:
    knn_model = pickle.load(model_file)

@app.route('/predict', methods=['POST'])
def predict():
    data = request.get_json(force=True)
    features = data['features']
    
    # Ensure the input is in the correct format
    if not isinstance(features, list):
        return jsonify({'error': 'Invalid input format. Expected a list of features.'}), 400
    
    prediction = knn_model.predict([features])
    return jsonify({'prediction': prediction.tolist()})

if __name__ == '__main__':
    app.run(debug=True)