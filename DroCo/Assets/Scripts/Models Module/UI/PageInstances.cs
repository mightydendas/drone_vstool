using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageInstancesViewModel
{
    public readonly int ModelId;
    public readonly InstanceJson[] Instances;

    public PageInstancesViewModel(int modelId, InstanceJson[] instances)
    {
        ModelId = modelId;
        Instances = instances;
    }
}

public class PageInstances : Page<PageInstancesViewModel>
{
    [SerializeField]
    private Button buttonRefresh;

    [SerializeField]
    private Button buttonAddInstance;

    [SerializeField]
    private GameObject pageElementInstancePrefab;

    [SerializeField]
    private GameObject content;

    private List<PageElementInstance> instances = new List<PageElementInstance>();

    private void Awake()
    {
        AwakeBase();

        if (buttonRefresh == null)
            throw new UnassignedReferenceException($"Missing {nameof(buttonRefresh)} reference.");
        if (buttonAddInstance == null)
            throw new UnassignedReferenceException($"Missing {nameof(buttonAddInstance)} reference.");
        if (pageElementInstancePrefab == null)
            throw new UnassignedReferenceException($"Missing {nameof(pageElementInstancePrefab)} reference.");
        if (content == null)
            throw new UnassignedReferenceException($"Missing {nameof(content)} reference.");

        buttonRefresh.onClick.RemoveListener(OnRefreshClick);
        buttonRefresh.onClick.AddListener(OnRefreshClick);

        buttonAddInstance.onClick.RemoveListener(OnAddInstanceClick);
        buttonAddInstance.onClick.AddListener(OnAddInstanceClick);
    }

    public override void LoadViewModel(PageInstancesViewModel viewModel)
    {
        ViewModel = viewModel;

        // clear children
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }

        // add children
        foreach (InstanceJson item in viewModel.Instances)
        {
            GameObject panelIntanceObject = GameObject.Instantiate(pageElementInstancePrefab, content.transform);

            if (!panelIntanceObject.TryGetComponent(out PageElementInstance pageElement))
            {
                Debug.LogError("Page Element Instance Prefab is missing PageElementInstance script.");
                return;
            }

            pageElement.LoadData(item);
            instances.Add(pageElement);
        }
    }

    public void OnRefreshClick()
    {
        DisablePage();

        ModelsModule.Instance.GetAllInstances(ViewModel.ModelId, handleSuccess, HandleError);

        void handleSuccess(InstanceGetAllResponseJson response)
        {
            try
            {
                var viewModel = new PageInstancesViewModel(ViewModel.ModelId, response.Instances);
                LoadViewModel(viewModel);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                EnablePage();
            }
        }
    }

    public void OnAddInstanceClick()
    {
        DisablePage();

        ModelsManager.Instance.LoadInstanceTemp(ViewModel.ModelId, onProgress, onSuccess, HandleError);

        void onSuccess(Instance instanceObject)
        {
            var viewModel = new InstanceCreateViewModel(ViewModel.ModelId, instanceObject);
            ModelsUIManager.Instance.PageInstanceCreate.OpenPage();
            ModelsUIManager.Instance.PageInstanceCreate.LoadViewModel(viewModel);
            ClosePage();
        }

        void onProgress(string status)
        {
            ShowStatus(status);
        }
    }

    private void HandleError(string message)
    {
        ShowError(message);
        Debug.LogError(message);
        EnablePage();
    }

    public override void BackPage()
    {
        base.BackPage();
        ModelsUIManager.Instance.PageModels.OpenPage();
    }
}
