using System;
using System.Linq;
using TMPro;
using TriLibCore.Extensions;
using TriLibCore.SFB;
using UnityEngine;
using UnityEngine.UI;

public class ModelCreateViewModel
{
    public string Name = "";
    public SupportedFileType FileType = SupportedFileType.FBX;
    public ItemWithStream FileWithStream = null;
}

public class PageModelCreate : Page<ModelCreateViewModel>
{
    [SerializeField]
    private TMP_InputField inputName;

    [SerializeField]
    private TMP_Dropdown dropdownFileType;

    [SerializeField]
    private TMP_InputField inputFilePath;

    [SerializeField]
    private Button buttonBrowse;

    [SerializeField]
    private Button buttonAddModel;

    private void Awake()
    {
        AwakeBase();

        if (inputName == null)
            throw new MissingReferenceException($"Missing {nameof(inputName)} reference.");
        if (dropdownFileType == null)
            throw new MissingReferenceException($"Missing {nameof(dropdownFileType)} reference.");
        if (inputFilePath == null)
            throw new MissingReferenceException($"Missing {nameof(inputFilePath)} reference.");
        if (buttonBrowse == null)
            throw new MissingReferenceException($"Missing {nameof(buttonBrowse)} reference.");
        if (buttonAddModel == null)
            throw new MissingReferenceException($"Missing {nameof(buttonAddModel)} reference.");

        buttonAddModel.onClick.RemoveListener(OnAddModelClick);
        buttonAddModel.onClick.AddListener(OnAddModelClick);

        buttonBrowse.onClick.RemoveListener(OnBrowseClick);
        buttonBrowse.onClick.AddListener(OnBrowseClick);

        dropdownFileType.ClearOptions();
        dropdownFileType.AddOptions(Enum.GetNames(typeof(SupportedFileType)).ToList());
    }

    private void OnGUI()
    {
        if (ViewModel == null)
            return;

        ViewModel.Name = inputName.text;
        ViewModel.FileType = (SupportedFileType)dropdownFileType.value;
        inputFilePath.text = ViewModel.FileWithStream?.Name ?? "";

        ValidateViewModel();
    }

    public override void LoadViewModel(ModelCreateViewModel viewModel)
    {
        ViewModel = new ModelCreateViewModel();

        inputName.text = ViewModel.Name;
        dropdownFileType.value = (int)ViewModel.FileType;
        inputFilePath.text = "";
    }

    private bool ValidateViewModel()
    {
        ClearStatus();

        if (ViewModel == null)
            return true;

        if (string.IsNullOrEmpty(ViewModel.Name))
        {
            ShowError("Model name is required");
            return false;
        }

        if (string.IsNullOrEmpty(ViewModel.FileWithStream?.Name))
        {
            ShowError("File path is required");
            return false;
        }

        return true;
    }

    private void OnBrowseClick()
    {
        var result = StandaloneFileBrowser.OpenFilePanel("Select file", Application.persistentDataPath, "zip", false);

        if (result.Count == 0)
            return;

        ViewModel.FileWithStream = result[0];
    }

    public void OnAddModelClick()
    {
        if (!ValidateViewModel())
            return;

        DisablePage();

        using var stream = ViewModel.FileWithStream.OpenStream();
        byte[] bytes = stream.ReadBytes();
        string data = Convert.ToBase64String(bytes);

        ModelsModule.Instance.Create(ViewModel.Name, ViewModel.FileType, data, handleSuccess, HandleError);

        void handleSuccess(ModelCreateResponseJson response)
        {
            Debug.Log($"Succesfully created model {response.Id}");
            BackPage();
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
        ModelsUIManager.Instance.PageModels.OnRefreshClick();
    }
}
