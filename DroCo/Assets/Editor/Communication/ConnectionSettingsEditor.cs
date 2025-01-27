using UnityEditor;
using UnityEngine;

namespace DroCo.Editor {
    public class ConnectionSettingsEditor : EditorWindow {

        private string httpHostname;
        private int httpPort;
        private string httpApiKey;

        [MenuItem("Tools/DroCo/Connection Settings")]
        public static void ShowWindow() {
            GetWindow<ConnectionSettingsEditor>("Connection settings");
        }

        private void OnGUI() {

            GUILayout.Label("Http settings", EditorStyles.boldLabel);

            httpHostname = EditorPrefs.HasKey("DroCo.HttpClientHostname") ? EditorPrefs.GetString("DroCo.HttpClientHostname") : httpHostname;
            httpHostname = EditorGUILayout.TextField("Hostname", httpHostname);

            httpPort = EditorPrefs.HasKey("DroCo.HttpClientPort") ? EditorPrefs.GetInt("DroCo.HttpClientPort") : httpPort;
            httpPort = EditorGUILayout.IntField("Port", httpPort);

            httpApiKey = EditorPrefs.HasKey("DroCo.HttpClientApiKey") ? EditorPrefs.GetString("DroCo.HttpClientApiKey") : httpApiKey;
            httpApiKey = EditorGUILayout.TextField("Api Key", httpApiKey);

            if (GUILayout.Button("Save")) {
                EditorPrefs.SetString("DroCo.HttpClientHostname", httpHostname);
                EditorPrefs.SetInt("DroCo.HttpClientPort", httpPort);
                EditorPrefs.SetString("DroCo.HttpClientApiKey", httpApiKey);
            }
        }
    }
}
