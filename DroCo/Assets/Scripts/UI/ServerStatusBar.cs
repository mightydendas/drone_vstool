using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ServerStatusBar : MonoBehaviour {

    [SerializeField]
    private TMP_Text serverIpText;
    [SerializeField]
    private TMP_Text connectionStatusText;
    [SerializeField]
    private Image outline;

    public void OnPointerEnter() {
        outline.color = Color.white;
    }

    public void OnPointerExit() {
        outline.color = Color.grey;
    }

    public void SetServerIP(string ip) {
        serverIpText.text = ip;
    }

    public void SetServerStatus(ConnectionStatus status) {
        switch (status) {
            case ConnectionStatus.Closed:
                connectionStatusText.text = status.ToString();
                connectionStatusText.color = Color.red;
                break;
            case ConnectionStatus.Listening:
                connectionStatusText.text = status.ToString();
                connectionStatusText.color = Color.white;
                break;
            case ConnectionStatus.Connected:
                connectionStatusText.text = status.ToString();
                connectionStatusText.color = Color.green;
                break;
            case ConnectionStatus.Disconnected:
                connectionStatusText.text = status.ToString();
                connectionStatusText.color = Color.red;
                break;
        }
    }
}
