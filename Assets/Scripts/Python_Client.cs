using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class Python_Client : MonoBehaviour
{
    public string ip = "127.0.0.1";
    public int port = 5000;

    public TextMeshProUGUI uiText;

    public async void AskPythonAsync(int wave, float performance)
    {
        await SendToPythonAsync(wave, performance);
    }

    private async Task SendToPythonAsync(int wave, float performance)
    {
        try
        {
            using (TcpClient client = new TcpClient())
            {
                await client.ConnectAsync(ip, port);
                using (NetworkStream stream = client.GetStream())
                {
                    var req = new RequestData { wave = wave, performance = performance };
                    string jsonText = JsonUtility.ToJson(req);
                    byte[] dataToSend = Encoding.UTF8.GetBytes(jsonText);

                    await stream.WriteAsync(dataToSend, 0, dataToSend.Length);

                    byte[] buffer = new byte[1024];
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    string reply = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    ResponseData resp = JsonUtility.FromJson<ResponseData>(reply);
                    Debug.Log("uiText is " + (uiText == null ? "NULL" : "ASSIGNED"));
                    if (uiText != null)
                        uiText.text = $"Speed: {resp.speed:F2}\nConf: {(resp.confidence * 100f):F0}%";
                    Debug.Log("UI Text now: " + uiText.text);
                    Debug.Log("Received from Python: " + reply);

                    // Apply the predicted speed to ALL enemies
                    Enemy[] enemies = FindObjectsOfType<Enemy>();
                    foreach (Enemy e in enemies)
                        e.speed = resp.speed;
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Python socket error: " + e.Message);
        }
    }

    [Serializable]
    public class RequestData
    {
        public int wave;
        public float performance;
    }

    [Serializable]
    public class ResponseData
    {
        public float speed;
        public float confidence;
    }
}
