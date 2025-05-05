using Newtonsoft.Json;

public sealed class InstanceGetAllRequestJson : IJsonRequestData
{
    [JsonProperty("model_id", Required = Required.Always)]
    public int? ModelId;
}

public sealed class InstanceGetAllResponseJson : IJsonResponseData
{
    [JsonProperty("instances", Required = Required.Always)]
    public InstanceJson[] Instances;
}
