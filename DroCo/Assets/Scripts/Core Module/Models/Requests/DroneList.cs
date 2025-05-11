using System;
using Newtonsoft.Json;

[Serializable]
public class DroneListResponse : IJsonResponseData {

    [JsonProperty("drones")]
    public DroneStaticData[] Drones;
}
