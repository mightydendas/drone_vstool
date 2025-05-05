using Newtonsoft.Json;

public sealed class InstanceDeleteRequestJson : IJsonRequestData
{
    [JsonProperty("id", Required = Required.Always)]
    public int Id;
}

public sealed class InstanceDeleteResponseJson : IJsonResponseData
{
    [JsonProperty("success", Required = Required.Always)]
    public bool Success;
}