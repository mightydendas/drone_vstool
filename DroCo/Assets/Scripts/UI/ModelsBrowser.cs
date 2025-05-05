using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelsBrowser : MonoBehaviour
{

    private GameObject modelsPanel;

    private void Start()
    {
        modelsPanel = this.gameObject;
        modelsPanel.SetActive(false);
    }

    private void Update()
    {
        
    }


    public void OpenPanel() {
        modelsPanel.SetActive(true);
    }

    public void ClosePanel() {
        modelsPanel.SetActive(false);
    }
}
