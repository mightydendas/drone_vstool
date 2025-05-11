using UnityEngine;
using System;
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

