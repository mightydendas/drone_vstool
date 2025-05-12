using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
public class HelloResponse : IJsonResponseData {
    [JsonProperty("client_id")]
    public string ClientId {
        get; set;
    }

    [JsonProperty("rtmp_port")]
    public string RtmpPort {
        get; set;
    }

    [JsonProperty("modules")]
    public List<string> Modules {
    get; set; }
}
