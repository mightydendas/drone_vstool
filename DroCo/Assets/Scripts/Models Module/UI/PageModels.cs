using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageModelsViewModel
{
    public int ForceDeleteIndex = -1;
    public ModelSimpleJson[] Models;
}

public class PageModels : Page<PageModelsViewModel>
{
    [SerializeField]
    private Button buttonRefresh;

    [SerializeField]
    private Button buttonAddModel;

    [SerializeField]
    private GameObject pageElementModelPrefab;

    [SerializeField]
    private GameObject content;

    private List<PageElementModel> models = new List<PageElementModel>();

    private void Awake()
    {
        AwakeBase();

        if (buttonRefresh == null)
            throw new MissingComponentException($"Missing {nameof(buttonRefresh)} reference.");
        if (buttonAddModel == null)
            throw new MissingComponentException($"Missing {nameof(buttonAddModel)} reference.");
        if (pageElementModelPrefab == null)
            throw new MissingComponentException($"Missing {nameof(pageElementModelPrefab)} reference.");
        if (content == null)
            throw new MissingComponentException($"Missing {nameof(content)} reference.");

        buttonRefresh.onClick.RemoveListener(OnRefreshClick);
        buttonRefresh.onClick.AddListener(OnRefreshClick);

        buttonAddModel.onClick.RemoveListener(OnAddModelClick);
        buttonAddModel.onClick.AddListener(OnAddModelClick);
    }

    public override void LoadViewModel(PageModelsViewModel viewModel)
    {
        this.ViewModel = viewModel;

        // clear children
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }

        // add children
        foreach (ModelSimpleJson item in ViewModel.Models)
        {
            GameObject panelObject = Instantiate(pageElementModelPrefab, content.transform);

            if (!panelObject.TryGetComponent(out PageElementModel pageElement))
            {
                Debug.LogError("Page Element Model Prefab is missing PageElementModel script.");
                return;
            }

            pageElement.LoadData(item);
            models.Add(pageElement);
        }
    }

    public void OnRefreshClick()
    {
        DisablePage();

        ModelsModule.Instance.GetAll(handleSuccess, HandleError);

        void handleSuccess(ModelGetAllResponseJson response)
        {
            try
            {
                var viewModel = new PageModelsViewModel();
                viewModel.Models = response.Models;
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

    public void OnAddModelClick()
    {
        ModelsUIManager.Instance.PageModelCreate.OpenPage();
        ModelsUIManager.Instance.PageModelCreate.LoadViewModel(null);
        this.ClosePage();
    }

    private void HandleError(string message)
    {
        ShowError(message);
        Debug.LogError(message);
        EnablePage();
    }

    public override void BackPage()
    {
        this.CloseUI();
    }
}
