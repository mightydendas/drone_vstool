using Newtonsoft.Json;

public sealed class ModelSimpleJson : IJsonResponseData
{
    [JsonProperty("id", Required = Required.Always)]
    public int Id;

    [JsonProperty("name", Required = Required.Always)]
    public string Name;
}
