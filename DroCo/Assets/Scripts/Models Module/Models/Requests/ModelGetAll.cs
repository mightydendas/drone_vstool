using Newtonsoft.Json;

public sealed class ModelGetAllResponseJson : IJsonResponseData
{
    [JsonProperty("models", Required = Required.Always)]
    public ModelSimpleJson[] Models;
}
