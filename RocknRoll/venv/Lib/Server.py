import socket
import json
import numpy as np
from sklearn.ensemble import RandomForestRegressor

# model training dummy data
X = np.array([[1,0.2],[2,0.4],[3,0.6],[4,0.8]])
y = np.array([2.0,2.5,3.1,3.6])
model = RandomForestRegressor(n_estimators=10, random_state=1)
model.fit(X, y)

# Socket creation on local host
HOST = "127.0.0.1"
PORT = 5000
server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server.bind((HOST, PORT))
server.listen(1)

print("ML Python server started on", HOST, ":", PORT)

while True:
    conn, addr = server.accept()
    print("Got connection from", addr)

    data = conn.recv(1024)
    if not data:
        conn.close()
        continue

    try:
        data_str = data.decode()
        req = json.loads(data_str)
        wave = float(req.get("wave", 1))
        perf = float(req.get("performance", 0.5))
#initiate list of prediction to start filling it
        preds = []
        for tree in model.estimators_:
            prediction_list = tree.predict([[wave, perf]])
            preds.append(prediction_list[0])

        mean_pred = float(np.mean(preds))
        std_pred = float(np.std(preds))
        conf = 1 - (std_pred / 2)
        conf = max(0.05, conf)
        response = {
            "speed": mean_pred,
            "confidence": conf
        }
        resp_text = json.dumps(response, ensure_ascii=False)
        print("Sending JSON:", resp_text)

        conn.sendall(resp_text.encode('utf-8'))
    except Exception as e:
        error_response = {"error": str(e)}
        conn.sendall(json.dumps(error_response).encode())

    conn.close()

