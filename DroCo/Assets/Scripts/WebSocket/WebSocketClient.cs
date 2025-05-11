using System.Collections;
using UnityEngine;
using NativeWebSocket;
using System;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

    private void OnMessageReceived(byte[] data) {
        string json = Encoding.UTF8.GetString(data);
        Debug.Log("Recieved: " + json);
        ResponseWrapper message = JsonConvert.DeserializeObject<ResponseWrapper>(json);

        if (!string.IsNullOrEmpty(message.RequestId) && pendingRequests.TryGetValue(message.RequestId, out PendingRequest pending)) {

            pendingRequests.Remove(message.RequestId);

            if (message.Type == "error") {
                ErrorData error = message.Data.ToObject<ErrorData>() ?? throw new ApplicationException("Faield to parse ErrorData");
                pending.OnError.Invoke(error.Message);
                return;
            }

            pending.OnSuccess.Invoke(message.Data);
            return;

        }

        if (unsolicitedHandlers.TryGetValue(message.Type, out Action<JObject> handler)) {
            handler?.Invoke(message.Data);
            return;
        }

        Debug.LogWarning("Unhandled message: " + json);
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
