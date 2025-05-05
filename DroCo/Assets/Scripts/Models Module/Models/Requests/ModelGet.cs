using Newtonsoft.Json;

public sealed class ModelGetRequestJson : IJsonRequestData
{
    [JsonProperty("id", Required = Required.Always)]
    public int Id;
}

public sealed class ModelGetResponseJson : IJsonResponseData
{
    [JsonProperty("model", Required = Required.Always)]
    public ModelJson Model;
}
