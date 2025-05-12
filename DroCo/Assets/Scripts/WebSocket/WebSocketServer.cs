using System;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class WebSocketServer : Singleton<WebSocketServer> {

    public string Address;
    public string Port;

    private WebSocketSharp.Server.WebSocketServer Server;

    private string clientID;

    public event Action<string> ServerRunning;
    public event Action ClientConnected;
    public event Action ClientDisconnected;
    public event Action<DroneStaticData> RecievedDrone;
    public event Action<DroneFlightData> RecievedFlightData;

    private Dictionary<string, Action<JObject>> unsolicitedHandlers = new Dictionary<string, Action<JObject>>();

    public void RegisterHandler(string messageType, Action<JObject> handler) {
        unsolicitedHandlers[messageType] = handler;
    }

    public void RemoveHandler(string messageType) {
        unsolicitedHandlers.Remove(messageType);
    }

    private void OnClientConnected() {
        ClientConnected?.Invoke();
    }

    private void OnClientDisconnected() {
        ClientDisconnected?.Invoke();
    }

    private void OnRecievedDrone(DroneStaticData drone) {
        RecievedDrone?.Invoke(drone);
    }

    private void OnRecievedFlightData(DroneFlightData flightData) {
        RecievedFlightData?.Invoke(flightData);
    }

    public void StartServer() {
        Debug.Log("Starting server");

        try {
            Server = new WebSocketSharp.Server.WebSocketServer("ws://" + Address + ":" + Port);
            Server.AddWebSocketService<WebSocketServerBehavior>("/", b => {
                b.ClientConnected += OnClientConnected;
                b.ClientDisconnected += OnClientDisconnected;
                b.RecievedDrone += OnRecievedDrone;
                b.RecievedFlightData += OnRecievedFlightData;
            });

            //Server.AddWebSocketService<TestBehavior>("/test");

            Server.Start();

            Debug.Log("Server started on " + Address + " and port " + Port);

        } catch (Exception ex) {
            Debug.LogError(ex.Message);
            Port = (int.Parse(Port) + 1).ToString();
            StartServer();
            return;
        }

        ServerRunning?.Invoke(GetLocalIPAddress() + ":" + Port);
    }

    private string GetLocalIPAddress() {
        try {
            // Get all network interfaces
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            string ethernetIP = null;
            string wifiIP = null;
            string otherIP = null;

            foreach (NetworkInterface ni in interfaces) {
                // Check if the network interface is up and has IP addresses
                if (ni.OperationalStatus == OperationalStatus.Up) {
                    foreach (UnicastIPAddressInformation ipInfo in ni.GetIPProperties().UnicastAddresses) {
                        // We're only interested in IPv4 addresses
                        if (ipInfo.Address.AddressFamily == AddressFamily.InterNetwork) {
                            // Check if it's Ethernet
                            if (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet) {
                                ethernetIP = ipInfo.Address.ToString();
                            }
                            // Check if it's Wi-Fi
                            else if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) {
                                wifiIP = ipInfo.Address.ToString();
                            }
                            // Store other network interfaces
                            else if (otherIP == null) {
                                otherIP = ipInfo.Address.ToString();
                            }
                        }
                    }
                }
            }

            // Prioritize Ethernet, then Wi-Fi, then others
            if (ethernetIP != null)
                return ethernetIP;
            if (wifiIP != null)
                return wifiIP;
            if (otherIP != null)
                return otherIP;

            throw new Exception("No valid network adapters found!");
        } catch (Exception e) {
            Debug.LogError("Error retrieving local IP address: " + e.Message);
            return "0.0.0.0";
        }
    }

    public void CloseServer() {
        Debug.Log("Closing server");
        if (Server != null) {
            Server.Stop();
            Server = null;
        }
    }

    private void SendMessageToClient() {
        clientID = Server.WebSocketServices["/test"].Sessions.IDs.First();
        Debug.Log("Sending test hello message to client " + clientID);
        try {
            Server.WebSocketServices["/test"].Sessions.SendTo("test hello message", clientID);
        } catch (Exception e) {
            Debug.LogError(e);
        }
    }

    private void OnApplicationQuit() {
        if (Server != null) {
            Server.Stop();
            Server = null;
        }
    }
}
