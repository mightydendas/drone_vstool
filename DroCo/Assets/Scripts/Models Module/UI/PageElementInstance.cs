using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PageElementInstance : PageElement<InstanceJson>
{
    [SerializeField]
    private Button buttonShow;

    [SerializeField]
    private Button buttonEdit;

    [SerializeField]
    private Button buttonDelete;

    [SerializeField]
    private TextMeshProUGUI textId;

    private void Awake()
    {
        if (buttonShow == null)
            throw new MissingComponentException($"Missing {nameof(buttonShow)} reference.");
        if (buttonEdit == null)
            throw new MissingComponentException($"Missing {nameof(buttonEdit)} reference.");
        if (buttonDelete == null)
            throw new MissingComponentException($"Missing {nameof(buttonDelete)} reference.");
        if (textId == null)
            throw new MissingComponentException($"Missing {nameof(textId)} reference.");

        buttonShow.onClick.RemoveListener(OnShowClick);
        buttonShow.onClick.AddListener(OnShowClick);

        buttonEdit.onClick.RemoveListener(OnEditClick);
        buttonEdit.onClick.AddListener(OnEditClick);

        buttonDelete.onClick.RemoveListener(OnDeleteClick);
        buttonDelete.onClick.AddListener(OnDeleteClick);
    }

    public override void LoadData(InstanceJson instanceJson)
    {
        ViewModel = instanceJson;

        textId.text = instanceJson.Id.ToString();
    }

    private void OnShowClick()
    {

    }

    private void OnEditClick()
    {

    }

    private void OnDeleteClick()
    {
        Page.DisablePage();

        ModelsModule.Instance.DeleteInstance(ViewModel.Id, handleSuccess, HandleError);

        void handleSuccess(InstanceDeleteResponseJson response)
        {
            try
            {
                if (!response.Success)
                {
                    HandleError("Failed to delete");
                    return;
                }

                Debug.Log($"Successfully deleted instance {ViewModel.Id}");
                ModelsUIManager.Instance.PageInstances.OnRefreshClick();
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
