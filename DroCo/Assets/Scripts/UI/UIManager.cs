using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager> {

    public MainScreen MainScreen;

    private void Start() {
        ModelsUIManager.Instance.Opened += OnModelsUIOpened;
        ModelsUIManager.Instance.Closed += OnModelsUIClosed;
    }

    private void OnModelsUIOpened() {
        MainScreen.gameObject.SetActive(false);
    }

    private void OnModelsUIClosed() {
        MainScreen.gameObject.SetActive(true);
    }
}
