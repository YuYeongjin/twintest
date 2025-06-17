using UnityEngine;
using Fleck;
using System.Collections.Generic;
public class UnityWebSocketServer : MonoBehaviour
{
    private WebSocketServer _server;
    private List<IWebSocketConnection> _clients = new List<IWebSocketConnection>();

    [Header("포트 번호")]
    public int port = 8081;

    void Start()
    {
        // 1) 0.0.0.0:port 에서 모든 인터페이스 리스닝
        _server = new WebSocketServer($"ws://0.0.0.0:{port}");
        
        // 2) 서버 시작
        _server.Start(socket =>
        {
            socket.OnOpen = () =>
            {
                Debug.Log($"✅ 클라이언트 연결됨 ({socket.ConnectionInfo.ClientIpAddress})");
                _clients.Add(socket);
            };
            socket.OnClose = () =>
            {
                Debug.Log($"🔌 클라이언트 연결 끊김 ({socket.ConnectionInfo.ClientIpAddress})");
                _clients.Remove(socket);
            };
            socket.OnMessage = message =>
            {
                Debug.Log($"📨 받은 메시지: {message}");
            };
            socket.OnError = ex =>
            {
                Debug.LogError($"❌ WS 서버 에러: {ex.Message}");
            };
        });

        Debug.Log($"🚀 Fleck WS 서버 기동: ws://<YourIP>:{port}");
    }

    public void Broadcast(string jsonPayload)
    {
        foreach (var client in _clients)
        {
            client.Send(jsonPayload);
        }
    }

    void OnApplicationQuit()
    {
        _server.Dispose();
        Debug.Log("🛑 Fleck WS 서버 종료");
    }
}
