using Newtonsoft.Json;

public class ResponseJson : MessageJson {

}

public sealed class ResponseJson<T> : ResponseJson where T : IJsonResponseData {

    [JsonProperty("data")]
    public T Data;
}
