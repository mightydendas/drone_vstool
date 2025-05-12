using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PageElementModel : PageElement<ModelSimpleJson>
{
    [SerializeField]
    private Button buttonInstances;

    [SerializeField]
    private Button buttonEdit;

    [SerializeField]
    private Button buttonDelete;

    [SerializeField]
    private TextMeshProUGUI textId;

    [SerializeField]
    private TextMeshProUGUI textName;

    private void Awake()
    {
        if (buttonInstances == null)
            throw new MissingComponentException($"Missing {nameof(buttonInstances)} reference.");
        if (buttonEdit == null)
            throw new MissingComponentException($"Missing {nameof(buttonEdit)} reference.");
        if (buttonDelete == null)
            throw new MissingComponentException($"Missing {nameof(buttonDelete)} reference.");
        if (textId == null)
            throw new MissingComponentException($"Missing {nameof(textId)} reference.");
        if (textName == null)
            throw new MissingComponentException($"Missing {nameof(textName)} reference.");

        buttonInstances.onClick.RemoveListener(OnOpenInstancesClick);
        buttonInstances.onClick.AddListener(OnOpenInstancesClick);

        buttonDelete.onClick.RemoveListener(OnDeleteClick);
        buttonDelete.onClick.AddListener(OnDeleteClick);

        buttonEdit.onClick.RemoveListener(OnEditClick);
        buttonEdit.onClick.AddListener(OnEditClick);
    }

    public override void LoadData(ModelSimpleJson viewModel)
    {
        ViewModel = viewModel;

        textId.text = viewModel.Id.ToString();
        textName.text = viewModel.Name;
    }

    public void OnOpenInstancesClick()
    {
        Page.DisablePage();

        ModelsModule.Instance.GetAllInstances(ViewModel.Id, handleSuccess, HandleError);

        void handleSuccess(InstanceGetAllResponseJson response)
        {
            try
            {
                var viewModel = new PageInstancesViewModel(ViewModel.Id, response.Instances);
                Page.ClosePage();
                ModelsUIManager.Instance.PageInstances.OpenPage();
                ModelsUIManager.Instance.PageInstances.LoadViewModel(viewModel);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                Page.EnablePage();
            }
        }
    }

    public void OnEditClick()
    {
        Page.DisablePage();
    }

    public void OnDeleteClick()
    {
        Page.DisablePage();

        ModelsModule.Instance.Delete(ViewModel.Id, false, handleSuccess, HandleError);

        void handleSuccess(ModelDeleteResponseJson response)
        {
            try
            {
                Debug.Log($"Successfully deleted model {response.Id}");
                ModelsUIManager.Instance.PageModels.OnRefreshClick();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                Page.EnablePage();
            }
        }
    }

    private void HandleError(string message)
    {
        Page.ShowError(message);
        Debug.LogError(message);
        Page.EnablePage();
    }
}
