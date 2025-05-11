using Newtonsoft.Json;

public class NotificationJson<T> : MessageJson where T : IJsonNotificationData {
    [JsonProperty("data")]
    public T Data;
}

