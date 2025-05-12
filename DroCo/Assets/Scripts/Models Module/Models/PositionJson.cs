using Newtonsoft.Json;

public sealed class PositionJson
{
    [JsonProperty("latitude", Required = Required.Always)]
    public double Latitude;

    [JsonProperty("longitude", Required = Required.Always)]
    public double Longitude;

    [JsonProperty("altitude", Required = Required.Always)]
    public double Altitude;
}
