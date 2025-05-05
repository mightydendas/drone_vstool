using System.Collections;
using UnityEngine;
using NativeWebSocket;
using System;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class CoreModule : Singleton<CoreModule> {

    public event Action Activated;
    public event Action Deactivated;
    public event Action<DroneStaticData[]> DroneListRecieved;
    public event Action<DroneFlightData> FlightDataRecieved;

    private void Start() {
        WebSocketClient.Instance.Connected += OnWebSocketConnected;
        WebSocketClient.Instance.Disconnected += OnWebSocketDisconnected;
    }

    private void OnWebSocketConnected(HelloResponse helloResponse) {
        if (helloResponse.Modules.Contains(nameof(CoreModule))) {
            WebSocketClient.Instance.RegisterHandler("data_broadcast", OnFlightDataRecieved);
            WebSocketClient.Instance.RegisterHandler("drone_list", OnDroneListRecieved);
            Activated?.Invoke();
        }
    }

    private void OnWebSocketDisconnected() {
        if (WebSocketClient.Instance != null) {
            WebSocketClient.Instance.RemoveHandler("data_broadcast");
            WebSocketClient.Instance.RemoveHandler("drone_list");
        }
        Deactivated?.Invoke();
    }

    public void SendDroneListRequest() {

        RequestJson request = new RequestJson() {
            Type = "drone_list",
            RequestId = Guid.NewGuid().ToString(),
        };

        WebSocketClient.Instance.Send(request, OnDroneListRecieved, OnError);
    }

    private void OnDroneListRecieved(JObject data) {
        DroneListResponse droneList = data.ToObject<DroneListResponse>();

        if (droneList == null) {
            Debug.LogError($"Failed to parse data: {data}");
        }

        DroneListRecieved?.Invoke(droneList.Drones);
    }

    private void OnFlightDataRecieved(JObject data) {
        DroneFlightData flightData = data.ToObject<DroneFlightData>();

        if (flightData == null) {
            Debug.LogError($"Failed to parse data: {data}");
        }

        FlightDataRecieved?.Invoke(flightData);
    }

    private void OnError(string error) {
        Debug.LogError(error);
    }
}

public class WebSocketClient : Singleton<WebSocketClient> {

    private class PendingRequest {

        public readonly string Type;
        public readonly string Id;
        public readonly Action<JObject> OnSuccess;
        public readonly Action<string> OnError;
        public bool Handled;

        public PendingRequest(string type, string id, Action<JObject> onSuccess, Action<string> onError) {
            Type = type;
            OnSuccess = onSuccess;
            OnError = onError;
        }

        public void StartTimeout(int seconds) {
            WebSocketClient.Instance.StartCoroutine(Timeout(seconds));
        }

        private IEnumerator Timeout(int seconds) {
            yield return new WaitForSecondsRealtime(seconds);
            if (Handled)
                yield break;
            WebSocketClient.Instance.pendingRequests.Remove(Id);
            OnError.Invoke("Timed out");
        }
    }

    private Dictionary<string, PendingRequest> pendingRequests = new Dictionary<string, PendingRequest>();
    private Dictionary<string, Action<JObject>> unsolicitedHandlers = new Dictionary<string, Action<JObject>>();

    /// <summary>
    /// Drone Server URI
    /// </summary>
    private string APIDomainWS = "";
    /// <summary>
    /// Websocket context
    /// </summary>
    private WebSocket websocket;

    private void Update() {
        if (websocket != null && websocket.State == WebSocketState.Open)
            websocket.DispatchMessageQueue();
    }

    public async void ConnectToServer(string domain, int port) {

        Disconnect();

        Debug.Log("Starting client");

        try {
            APIDomainWS = $"ws://{domain}:{port}";
            websocket = new WebSocket(APIDomainWS);

            websocket.OnOpen += OnConnected;
            websocket.OnError += OnError;
            websocket.OnClose += OnClose;
            websocket.OnMessage += OnMessageReceived;

            await websocket.Connect();
        } catch (UriFormatException ex) {
            Debug.LogError(ex);
        }
    }

    public void Disconnect() {
        if (websocket != null && websocket.State == WebSocketState.Open) {
            Debug.Log("Disconnecting client");
            websocket.CancelConnection();
            websocket = null;
        }
    }

    public void RegisterHandler(string messageType, Action<JObject> handler) {
        unsolicitedHandlers[messageType] = handler;
    }

    public void RemoveHandler(string messageType) {
        unsolicitedHandlers.Remove(messageType);
    }

    public void Send(RequestJson request, Action<JObject> onSuccess, Action<string> onError) {
        request.RequestId = Guid.NewGuid().ToString();
        string json = JsonConvert.SerializeObject(request);

        pendingRequests[request.RequestId] = new PendingRequest(request.Type, request.RequestId, onSuccess, onError);

        websocket.SendText(json);
    }

    public void SendRaw(string type, string json, Action<JObject> onSuccess, Action<string> onError) {
        string requestId = Guid.NewGuid().ToString();
        pendingRequests[requestId] = new PendingRequest(type, requestId, onSuccess, onError);
        websocket.SendText(json);
    }

    private void OnMessageReceived(byte[] data) {
        string json = Encoding.UTF8.GetString(data);
        Debug.Log("Recieved: " + json);
        ResponseWrapper message = JsonConvert.DeserializeObject<ResponseWrapper>(json);

        if (!string.IsNullOrEmpty(message.RequestId) && pendingRequests.TryGetValue(message.RequestId, out var pending)) {

            pendingRequests.Remove(message.RequestId);

            if (string.IsNullOrEmpty(message.Error)) {
                pending.OnSuccess.Invoke(message.Data);
            } else {
                pending.OnError.Invoke(message.Error);
            }

        } else if (unsolicitedHandlers.TryGetValue(message.Type, out var handler)) {
            handler?.Invoke(message.Data);
        } else {
            Debug.LogWarning("Unhandled message: " + json);
        }
    }

    public void SendDroneListRequestNew(Action<JObject> onSuccess, Action<string> onError) {
        string json = "{\"type\":\"drone_list\", \"data\": {}}";
        SendRaw("drone_list", json, onSuccess, onError);
    }

    public event Action<HelloResponse> Connected;
    public event Action Disconnected;

    private void OnClose(WebSocketCloseCode closeCode) {
        Debug.Log("Connection closed!");
        Disconnected?.Invoke();
    }

    private void OnError(string errorMsg) {
        Debug.LogError(errorMsg);
        Disconnected?.Invoke();
    }

    private void OnConnected() {
        Debug.Log("Connected - sending handshake");
        string requestId = Guid.NewGuid().ToString();
        RequestJson<Hello> request = new RequestJson<Hello>() {
            Type = "hello",
            RequestId = requestId,
            Data = new Hello() {
                ClientType = 1,
            }
        };
        Send(request, OnHelloResponse, OnHandshakeError);
    }

    private void OnHelloResponse(JObject data) {
        HelloResponse response = data.ToObject<HelloResponse>();

        if (response == null) {
            OnHandshakeError($"Failed to parse response: {data}");
            return;
        }

        Debug.Log("Handshake successful.");
        Connected?.Invoke(response);
    }

    private void OnHandshakeError(string error) {
        Debug.LogError(error);
        Disconnect();
    }
    
    private async void OnApplicationQuit() {
        if (websocket != null) {
            await websocket.Close();
        }
    }
}

public class ResponseWrapper {

    [JsonProperty("type", Required = Required.Always)]
    public string Type;

    [JsonProperty("request_id")]
    public string RequestId;

    [JsonProperty("data")]
    public JObject Data;

    [JsonProperty("error")]
    public string Error;
}



public abstract class MessageJson {
    [JsonProperty("type", Required = Required.Always)]
    public string Type;

    [JsonProperty("request_id")]
    public string RequestId;
}

public class NotificationJson<T> : MessageJson where T : IJsonNotificationData {
    [JsonProperty("data")]
    public T Data;
}

public abstract class ResponseJson : MessageJson {

    [JsonProperty("error")]
    public string Error;
}

public class RequestJson : MessageJson {
}

public sealed class RequestJson<T> : RequestJson where T : IJsonRequestData {

    [JsonProperty("data")]
    public T Data;
}

public sealed class ResponseJson<T> : ResponseJson where T : IJsonResponseData {

    [JsonProperty("data")]
    public T Data;
}

 
public interface IJsonRequestData {
}

public interface IJsonResponseData {
}

public interface IJsonNotificationData {
}

