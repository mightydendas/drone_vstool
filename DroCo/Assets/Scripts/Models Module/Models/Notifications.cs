using Newtonsoft.Json;

public sealed class ModelCreatedNotificationJson : IJsonNotificationData
{
    [JsonProperty("model", Required = Required.Always)]
    public ModelJson Model;
}

public sealed class ModelUpdatedNotificationJson : IJsonNotificationData
{
    [JsonProperty("model", Required = Required.Always)]
    public ModelJson Model;
}

public sealed class ModelDeletedNotificationJson : IJsonNotificationData
{
    [JsonProperty("id", Required = Required.Always)]
    public int Id;
}

public sealed class InstanceCreatedNotificationJson : IJsonNotificationData
{
    [JsonProperty("instance", Required = Required.Always)]
    public InstanceJson Instance;
}

public sealed class InstanceUpdatedNotificationJson : IJsonNotificationData
{
    [JsonProperty("instance", Required = Required.Always)]
    public InstanceJson Instance;
}

public sealed class InstanceDeletedNotificationJson : IJsonNotificationData
{
    [JsonProperty("id", Required = Required.Always)]
    public int Id;
}