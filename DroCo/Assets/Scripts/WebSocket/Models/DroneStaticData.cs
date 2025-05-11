using System;
using Newtonsoft.Json;

[Serializable]
public class DroneStaticData {

    [JsonProperty("client_id")]
    public string ClientId;

    [JsonProperty("drone_name")]
    public string DroneName;

    [JsonProperty("serial")]
    public string Serial;
}
