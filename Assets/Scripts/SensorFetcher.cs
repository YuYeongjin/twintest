using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;
using NativeWebSocket;
public class SensorFetcher : MonoBehaviour
{
    [System.Serializable]
    public class SensorData
    {
        public string location;
        public int temperature;
        public string timestamp;
    }

    WebSocket websocket;

    async void Start()
    {
        websocket = new WebSocket("ws://localhost:8081/ws/unity");

        websocket.OnOpen += () =>
        {
            Debug.Log("WebSocket 연결됨");
            websocket.SendText("Unity에서 연결 완료");
        };

        websocket.OnMessage += (bytes) =>
        {
            string message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("📨 수신 메시지: " + message);
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("❌ WebSocket 오류: " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("🔌 연결 종료됨");
        };

        await websocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket?.DispatchMessageQueue();
#endif
    }

    async void OnApplicationQuit()
    {
        await websocket.Close();
    }



    IEnumerator FetchData()
    {
        UnityWebRequest request = UnityWebRequest.Get("http://localhost:8080/api/sensor/latest");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            SensorData data = JsonUtility.FromJson<SensorData>(request.downloadHandler.text);
            Debug.Log($"📡 [REST] 위치: {data.location}, 온도: {data.temperature}");
        }
        else
        {
            Debug.LogError("❌ REST 통신 실패: " + request.error);
        }
    }
}