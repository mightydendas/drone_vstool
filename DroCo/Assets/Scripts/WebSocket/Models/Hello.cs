using System;
using Newtonsoft.Json;

[Serializable]
internal class Hello : IJsonRequestData {

    [JsonProperty("ctype")]
    public int ClientType;

    [JsonProperty("drone_name")]
    public string DroneName;

    [JsonProperty("serial")]
    public string Serial;
}
