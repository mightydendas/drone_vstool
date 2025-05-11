using Newtonsoft.Json;

public class RequestJson : MessageJson {
}

public sealed class RequestJson<T> : RequestJson where T : IJsonRequestData {

    [JsonProperty("data")]
    public T Data;
}
