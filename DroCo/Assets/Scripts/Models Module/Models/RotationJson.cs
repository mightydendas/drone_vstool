using Newtonsoft.Json;

public sealed class RotationJson
{
    [JsonProperty("pitch", Required = Required.Always)]
    public double Pitch;

    [JsonProperty("roll", Required = Required.Always)]
    public double Roll;

    [JsonProperty("heading", Required = Required.Always)]
    public double Heading;
}
