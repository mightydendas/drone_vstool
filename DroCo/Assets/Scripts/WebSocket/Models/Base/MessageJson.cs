using Newtonsoft.Json;

public abstract class MessageJson {
    [JsonProperty("type", Required = Required.Always)]
    public string Type;

    [JsonProperty("request_id")]
    public string RequestId;
}

