using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine.Networking;

namespace DroCo.Editor {
    public class HttpClient {

        public static Task<Message<TResponseData>> SendRequest<TResponseData>(Message request) {
            return SendRequest<object, TResponseData>(request);
        }

        public static async Task<Message<TResponseData>> SendRequest<TRequestData, TResponseData>(Message<TRequestData> request) {

            string hostname = EditorPrefs.GetString("DroCo.HttpClientHostname", "");
            int port = EditorPrefs.GetInt("DroCo.HttpClientPort", 0);
            string apiKey = EditorPrefs.GetString("DroCo.HttpClientApiKey", "");

            if (hostname == "" || port == 0 || apiKey == "") {
                throw new Exception("Configure HttpClient settings in Tools/DroCo/ConnectionSettings");
            }

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
    }
}
