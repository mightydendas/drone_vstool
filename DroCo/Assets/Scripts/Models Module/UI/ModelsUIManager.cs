using System;
using UnityEngine;
using UnityEngine.UI;

public class ModelsUIManager : Singleton<ModelsUIManager>
{
    public Button OpenButton;

    public PageModels PageModels;

    public PageModelCreate PageModelCreate;

    public PageInstances PageInstances;

    public PageInstanceCreate PageInstanceCreate;

    public Gizmo Gizmo;

    public event Action Opened;

    public event Action Closed;

    private void Awake()
    {
        if (OpenButton == null)
            throw new MissingReferenceException($"Missing {nameof(OpenButton)} reference.");
        if (PageModels == null)
            throw new MissingReferenceException($"Missing {nameof(PageModels)} reference.");
        if (PageModelCreate == null)
            throw new MissingReferenceException($"Missing {nameof(PageModelCreate)} reference.");
        if (PageInstances == null)
            throw new MissingReferenceException($"Missing {nameof(PageInstances)} reference.");
        if (PageInstanceCreate == null)
            throw new MissingReferenceException($"Missing {nameof(PageInstanceCreate)} reference.");
        if (Gizmo == null)
            throw new MissingReferenceException($"Missing {nameof(Gizmo)} reference.");
    }

    private void Start()
    {
        ModelsModule.Instance.Activated += OnModelsModuleActivated;
        ModelsModule.Instance.Deactivated += OnModelsModuleDeactivated;

        OnModelsModuleDeactivated();
        CloseUI();
    }

    private void OnModelsModuleActivated()
    {
        OpenButton.onClick.RemoveListener(OpenUI);
        OpenButton.onClick.AddListener(OpenUI);
        OpenButton.gameObject.SetActive(true);
    }

    private void OnModelsModuleDeactivated()
    {
        if (OpenButton != null) // muze byt null pri ukonceni aplikace
        {
            OpenButton.onClick.RemoveListener(OpenUI);
            OpenButton.gameObject.SetActive(false);
        }
    }

    public void CloseUI()
    {
        PageModels.ClosePage();
        PageModelCreate.ClosePage();
        PageInstances.ClosePage();
        PageInstanceCreate.ClosePage();
        OpenButton.gameObject.SetActive(true);
        Closed?.Invoke();
    }

    public void OpenUI()
    {
        PageModels.OpenPage();
        PageModels.OnRefreshClick();
        OpenButton.gameObject.SetActive(false);
        Opened?.Invoke();
    }
}
