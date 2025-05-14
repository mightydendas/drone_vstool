using UnityEngine;
using System;
using Newtonsoft.Json.Linq;

public class CoreModule : Singleton<CoreModule> {

    public event Action Activated;
    public event Action Deactivated;

    public event Action<DroneFlightData> FlightDataRecieved;

    private void Start() {
        WebSocketClient.Instance.Connected += OnWebSocketConnected;
        WebSocketClient.Instance.Disconnected += OnWebSocketDisconnected;
    }

    private void OnWebSocketConnected(HelloResponse helloResponse) {
        if (helloResponse.Modules.Contains(nameof(CoreModule))) {
            WebSocketClient.Instance.RegisterHandler("data_broadcast", OnFlightDataRecieved);
            Activated?.Invoke();
        }
    }

    private void OnWebSocketDisconnected() {
        if (WebSocketClient.Instance != null) {
            WebSocketClient.Instance.RemoveHandler("data_broadcast");
        }
        Deactivated?.Invoke();
    }

    public void SendDroneListRequest(Action<DroneListResponse> onSuccess, Action<string> onError) {

        RequestJson request = new RequestJson() {
            Type = "drone_list",
            RequestId = Guid.NewGuid().ToString(),
        };

        WebSocketClient.Instance.Send(request, onRecieved, onError);

        void onRecieved(JObject data) {
            DroneListResponse droneList = data.ToObject<DroneListResponse>();

            if (droneList == null) {
                onError?.Invoke($"Failed to deserialize {data}");
                return;
            }

            onSuccess.Invoke(droneList);
        }
    }

    private void OnFlightDataRecieved(JObject data) {
        DroneFlightData flightData = data.ToObject<DroneFlightData>();

        if (flightData == null) {
            Debug.LogError($"Failed to deserialize {data}");
        }

        FlightDataRecieved?.Invoke(flightData);
    }
}

