using Newtonsoft.Json;

public sealed class InstanceUpdateRequestJson : IJsonRequestData
{
    [JsonProperty("id", Required = Required.Always)]
    public int Id;

    [JsonProperty("model_id", Required = Required.Always)]
    public int ModelId;

    [JsonProperty("position", Required = Required.Always)]
    public PositionJson Position;

    [JsonProperty("rotation", Required = Required.Always)]
    public RotationJson Rotation;

    [JsonProperty("scale", Required = Required.Always)]
    public float Scale;
}


public sealed class InstanceUpdateResponseJson : IJsonResponseData
{
    [JsonProperty("id", Required = Required.Always)]
    public int Id;
}