using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class ResponseWrapper {

    [JsonProperty("type", Required = Required.Always)]
    public string Type;

    [JsonProperty("request_id")]
    public string RequestId;

    [JsonProperty("data")]
    public JObject Data;
}

