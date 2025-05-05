using System;
using System.Collections;
using Newtonsoft.Json;
using PimDeWitte.UnityMainThreadDispatcher;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

public class WebSocketServerBehavior : WebSocketBehavior {

    private bool handshake_done = false;

    protected override void OnOpen() {
        base.OnOpen();
        Debug.Log("Connection open");
    }

    protected override void OnMessage(MessageEventArgs e) {
        base.OnMessage(e);
        //Debug.Log(e.Data);
        ResponseWrapper message = JsonConvert.DeserializeObject<ResponseWrapper>(e.Data);
        if (message.Type == "hello") {
            DoHandshake(ID, message.Data.ToObject<Hello>());
        } else if (handshake_done && message.Type == "data_broadcast") {
            DroneFlightData dfd = message.Data.ToObject<DroneFlightData>();
            UnityMainThreadDispatcher.Instance().Enqueue(UpdateDroneFlightData(dfd));
        } else {
            Debug.LogError("Unknown data received! " + e.Data);
        }
    }

    protected override void OnClose(CloseEventArgs e) {
        base.OnClose(e);
        Debug.Log("Connection close: " + e.Reason);
        UnityMainThreadDispatcher.Instance().Enqueue(HandleClientDisconnected());
    }

    protected override void OnError(ErrorEventArgs e) {
        base.OnError(e);
        Debug.Log("Connection error: " + e.Message + " ..exception: " + e.Exception);
    }

    private void DoHandshake(string clientID, Hello droneData) {
        ResponseJson<HelloResponse> helloResponse = new ResponseJson<HelloResponse> {
            Type = "hello_resp",
            Data = new HelloResponse() {
                ClientId = clientID,
                RtmpPort = "1935",
            }
        };

        string msg = JsonUtility.ToJson(helloResponse);
        Debug.Log("Sending:" + msg);

        Send(msg);

        handshake_done = true;

        DroneStaticData newDrone = new DroneStaticData {
            client_id = clientID,
            drone_name = droneData.DroneName,
            serial = droneData.Serial
        };

        UnityMainThreadDispatcher.Instance().Enqueue(HandleClientConnected());
        UnityMainThreadDispatcher.Instance().Enqueue(AddDrone(newDrone));
    }

    public event Action ClientConnected;
    public event Action ClientDisconnected;
    public event Action<DroneStaticData> RecievedDrone;
    public event Action<DroneFlightData> RecievedFlightData;

    private IEnumerator HandleClientConnected() {
        ClientConnected?.Invoke();
        yield return null;
    }

    private IEnumerator HandleClientDisconnected() {
        ClientDisconnected?.Invoke();
        yield return null;
    }

    private IEnumerator AddDrone(DroneStaticData newDrone) {
        RecievedDrone?.Invoke(newDrone);
        yield return null;
    }

    private IEnumerator UpdateDroneFlightData(DroneFlightData flightData) {
        RecievedFlightData?.Invoke(flightData);
        yield return null;
    }
}
