using Newtonsoft.Json;

public sealed class ModelJson : IJsonResponseData
{
    [JsonProperty("id", Required = Required.Always)]
    public int Id;

    [JsonProperty("name", Required = Required.Always)]
    public string Name;

    [JsonProperty("data", Required = Required.Always)]
    public string Data;

    [JsonProperty("file_type", Required = Required.Always)]
    public SupportedFileType FileType;
}
