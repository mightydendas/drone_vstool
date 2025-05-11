using Newtonsoft.Json;

public sealed class ModelDeleteRequestJson : IJsonRequestData
{
    [JsonProperty("id", Required = Required.Always)]
    public int Id;

    [JsonProperty("force", Required = Required.Always)]
    public bool Force;
}

public sealed class ModelDeleteResponseJson : IJsonResponseData
{
    [JsonProperty("id", Required = Required.Always)]
    public int Id;
}