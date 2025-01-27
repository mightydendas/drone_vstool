using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TriLibCore.General;
using UnityEngine;
using UnityEngine.Networking;

public class HttpClient : Singleton<HttpClient> {

    [SerializeField]
    private string hostname;

    [SerializeField]
    private int port;

    [SerializeField]
    private string apiKey = "9BA6EA03-B550-4052-B8D3-F1645F8B225B";

    public Task<Message<TResponseData>> SendRequestAsync<TResponseData>(Message request) {
        return SendRequestAsync<object, TResponseData>(request);
    }

    public async Task<Message<TResponseData>> SendRequestAsync<TRequestData, TResponseData>(Message<TRequestData> request) {

        string url = $"http://{hostname}:{port}/";

        UnityWebRequest webRequest = new UnityWebRequest(url, "POST");
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("x-api-key", apiKey);

        string requestJson = JsonConvert.SerializeObject(request);
        byte[] requestData = Encoding.UTF8.GetBytes(requestJson);

        webRequest.uploadHandler = new UploadHandlerRaw(requestData);
        webRequest.downloadHandler = new DownloadHandlerBuffer();

        UnityWebRequestAsyncOperation op = webRequest.SendWebRequest();

        while (op.isDone == false) {
            await Task.Yield();
        }

        if (webRequest.result == UnityWebRequest.Result.Success) {
            string responseJson = Encoding.UTF8.GetString(webRequest.downloadHandler.data);
            return JsonConvert.DeserializeObject<Message<TResponseData>>(responseJson);
        } else {
            throw new Exception(webRequest.error);
        }
    }

    public void SendRequest<TResponseData>(Message request, Action<Message<TResponseData>> onSuccess, Action<string> onError = null) {
        StartCoroutine(SendRequestCoroutine(request, onSuccess, onError));
    }

    public void SendRequest<TRequestData, TResponseData>(Message<TRequestData> request, Action<Message<TResponseData>> onSuccess, Action<string> onError = null) {
        StartCoroutine(SendRequestCoroutine(request, onSuccess, onError));
    }

    private IEnumerator SendRequestCoroutine<TRequestData, TResponseData>(Message<TRequestData> request, Action<Message<TResponseData>> onSuccess, Action<string> onError = null) {

        string url = $"http://{hostname}:{port}/";

        UnityWebRequest webRequest = new UnityWebRequest(url, "POST");
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("x-api-key", apiKey);

        string requestJson = JsonConvert.SerializeObject(request);
        byte[] requestData = Encoding.UTF8.GetBytes(requestJson);

        webRequest.uploadHandler = new UploadHandlerRaw(requestData);
        webRequest.downloadHandler = new DownloadHandlerBuffer();

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success) {
            string responseJson = Encoding.UTF8.GetString(webRequest.downloadHandler.data);
            onSuccess.Invoke(JsonConvert.DeserializeObject<Message<TResponseData>>(responseJson));
        } else {
            onError?.Invoke(webRequest.error);
        }
    }
}

public class Message<T> {

    [JsonProperty("type")]
    public string Type {
        get; set;
    }

    [JsonProperty("data")]
    public T Data {
        get; set;
    } = default;
}

public class Message : Message<object> {

}

public class ModelDto {

    [JsonProperty("id")]
    public int Id {
        get; set;
    }

    [JsonProperty("name")]
    public string Name {
        get; set;
    }

    [JsonProperty("data")]
    public string Data {
        get; set;
    }

    [JsonProperty("alphaMaterialMode")]
    public AlphaMaterialMode AlphaMaterialMode {
        get; set;
    }

    [JsonProperty("useUnityNativeNormalCalculator")]
    public bool UseUnityNativeNormalCalculator {
        get; set;
    }

    [JsonProperty("useUnityNativeTextureLoader")]
    public bool UseUnityNativeTextureLoader {
        get; set;
    }

    [JsonProperty("getCompatibleTextureFormat")]
    public bool GetCompatibleTextureFormat {
        get; set;
    }

    [JsonProperty("enforceAlphaChannelTextures")]
    public bool EnforceAlphaChannelTextures {
        get; set;
    }
}

public class ModelListDto {

    [JsonProperty("id")]
    public int Id {
        get; set;
    }

    [JsonProperty("name")]
    public string Name {
        get; set;
    }
}

public class ModelCreateDto {

    [JsonProperty("name")]
    public string Name {
        get; set;
    }

    [JsonProperty("data")]
    public string Data {
        get; set;
    }

    [JsonProperty("alphaMaterialMode")]
    public AlphaMaterialMode AlphaMaterialMode {
        get; set;
    }

    [JsonProperty("useUnityNativeNormalCalculator")]
    public bool UseUnityNativeNormalCalculator {
        get; set;
    }

    [JsonProperty("useUnityNativeTextureLoader")]
    public bool UseUnityNativeTextureLoader {
        get; set;
    }

    [JsonProperty("getCompatibleTextureFormat")]
    public bool GetCompatibleTextureFormat {
        get; set;
    }

    [JsonProperty("enforceAlphaChannelTextures")]
    public bool EnforceAlphaChannelTextures {
        get; set;
    }
}

public class InstanceDto {

    [JsonProperty("id")]
    public int Id {
        get; set;
    }

    [JsonProperty("model_id")]
    public int ModelId {
        get; set;
    }

    [JsonProperty("position")]
    public PositionGeo Position {
        get; set;
    }

    [JsonProperty("rotation")]
    public RotationGeo Rotation {
        get; set;
    }

    [JsonProperty("scale")]
    public float Scale {
        get; set;
    }
}

public class InstanceCreateDto {

    [JsonProperty("model_id")]
    public int ModelId {
        get; set;
    }

    [JsonProperty("position")]
    public PositionGeo Position {
        get; set;
    }

    [JsonProperty("rotation")]
    public RotationGeo Rotation {
        get; set;
    }

    [JsonProperty("scale")]
    public float Scale {
        get; set;
    }
}

public class PositionGeo {

    [JsonProperty("altitude")]
    public float Altitude {
        get; set;
    }

    [JsonProperty("latitude")]
    public float Latitude {
        get; set;
    }

    [JsonProperty("longitude")]
    public float Longitude {
        get; set;
    }
}

public class RotationGeo {

    [JsonProperty("pitch")]
    public float Pitch {
        get; set;
    }

    [JsonProperty("roll")]
    public float Roll {
        get; set;
    }

    [JsonProperty("yaw")]
    public float Yaw {
        get; set;
    }

    [JsonProperty("compass")]
    public float Compass {
        get; set;
    }
}
