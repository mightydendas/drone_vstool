using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RunAsBar : MonoBehaviour {

    [SerializeField]
    private Image outline;

    public void OnPointerEnter() {
        outline.color = Color.white;
    }

    public void OnPointerExit() {
        outline.color = Color.grey;
    }

    public void OnDropdownChanged(TMP_Dropdown dropdown) {
        AppMode mode;
        switch (dropdown.value) {
            case 0:
                mode = AppMode.Client;
                break;
            case 1:
                mode = AppMode.Server;
                break;
            default:
                mode = AppMode.Client;
                break;
        }

        GameManager.Instance.ChangeAppMode(mode);
    }

}
