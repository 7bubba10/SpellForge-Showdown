using UnityEngine;
using System.Threading.Tasks;
using System.Text.Json;
using SocketIOClient;

public class SocketRunner : MonoBehaviour
{
    [SerializeField] string serverUrl = "http://localhost:3003/game";

    private SocketIOClient.SocketIO client;

    private async void Start()
    {
        client = new SocketIOClient.SocketIO(serverUrl, new SocketIOOptions
        {
            Reconnection = true,
            ReconnectionAttempts = 20,
            ReconnectionDelay = 1000
        });

        client.OnConnected += (_, __) => Debug.Log("[unity] connected");
        client.OnDisconnected += (_, reason) => Debug.Log($"[unity] disconnected: {reason}");

        // tick payload looks like: { "t": 123 }
        client.On("tick", res =>
        {
            try
            {
                var root = res.GetValue<JsonElement>(); // single JSON object arg
                if (root.TryGetProperty("t", out var tProp) && tProp.ValueKind == JsonValueKind.Number)
                    Debug.Log($"[unity] tick {tProp.GetInt32()}");
                else
                    Debug.Log($"[unity] tick payload: {res.ToString()}");
            }
            catch
            {
                Debug.Log($"[unity] tick raw: {res.ToString()}");
            }
        });

        client.On("lobby:players", res =>
        {
            // usually one JSON object arg with players/state; just print it for now
            Debug.Log($"[unity] roster: {res.ToString()}");
        });

        await SafeConnect();

        await client.EmitAsync("lobby:create", new { hostName = "Aidan (Unity)" });
        await Task.Delay(300);
        await client.EmitAsync("lobby:setReady", new { ready = true });
    }

    private async Task SafeConnect()
    {
        try { await client.ConnectAsync(); }
        catch (System.Exception ex) { Debug.LogError($"[unity] connect failed: {ex.Message}"); }
    }

    private async void OnDestroy()
    {
        if (client != null)
        {
            try { await client.DisconnectAsync(); } catch { }
            client.Dispose();
        }
    }

    void Awake() {DontDestroyOnLoad(gameObject);}
}
