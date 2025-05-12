using Newtonsoft.Json;

public sealed class ErrorData : IJsonResponseData {
    [JsonProperty("message")]
    public string Message;
}

