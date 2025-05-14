using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.GameEngine.Geometry;
using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InstanceCreateViewModel
{
    public readonly int ModelId;
    public readonly Instance Instance;
    public double Latitude;
    public double Longitude;
    public double Altitude;
    public double Pitch;
    public double Roll;
    public double Heading;
    public float Scale;

    public InstanceCreateViewModel(int modelId, Instance instance)
    {
        ModelId = modelId;
        Instance = instance;
    }
}
public class PageInstanceCreate : Page<InstanceCreateViewModel>
{
    [SerializeField]
    private Button buttonAddInstance;

    [SerializeField]
    private TMP_InputField inputLatitude;

    [SerializeField]
    private TMP_InputField inputLongitude;

    [SerializeField]
    private TMP_InputField inputAltitude;

    [SerializeField]
    private TMP_InputField inputPitch;

    [SerializeField]
    private TMP_InputField inputRoll;

    [SerializeField]
    private TMP_InputField inputHeading;

    [SerializeField]
    private TMP_InputField inputScale;

    private void Awake()
    {
        AwakeBase();

        if (buttonAddInstance == null)
            throw new MissingComponentException($"Missing {nameof(buttonAddInstance)} reference.");
        if (inputLatitude == null)
            throw new MissingComponentException($"Missing {nameof(inputLatitude)} reference.");
        if (inputLongitude == null)
            throw new MissingComponentException($"Missing {nameof(inputLongitude)} reference.");
        if (inputAltitude == null)
            throw new MissingComponentException($"Missing {nameof(inputAltitude)} reference.");
        if (inputPitch == null)
            throw new MissingComponentException($"Missing {nameof(inputPitch)} reference.");
        if (inputRoll == null)
            throw new MissingComponentException($"Missing {nameof(inputRoll)} reference.");
        if (inputHeading == null)
            throw new MissingComponentException($"Missing {nameof(inputHeading)} reference.");
        if (inputScale == null)
            throw new MissingComponentException($"Missing {nameof(inputScale)} reference.");

        buttonAddInstance.onClick.RemoveListener(OnAddInstanceClick);
        buttonAddInstance.onClick.AddListener(OnAddInstanceClick);

        inputLatitude.onEndEdit.RemoveListener(OnDataChanged);
        inputLatitude.onEndEdit.AddListener(OnDataChanged);
        inputLongitude.onEndEdit.RemoveListener(OnDataChanged);
        inputLongitude.onEndEdit.AddListener(OnDataChanged);
        inputAltitude.onEndEdit.RemoveListener(OnDataChanged);
        inputAltitude.onEndEdit.AddListener(OnDataChanged);
        inputPitch.onEndEdit.RemoveListener(OnDataChanged);
        inputPitch.onEndEdit.AddListener(OnDataChanged);
        inputRoll.onEndEdit.RemoveListener(OnDataChanged);
        inputRoll.onEndEdit.AddListener(OnDataChanged);
        inputHeading.onEndEdit.RemoveListener(OnDataChanged);
        inputHeading.onEndEdit.AddListener(OnDataChanged);
        inputScale.onEndEdit.RemoveListener(OnDataChanged);
        inputScale.onEndEdit.AddListener(OnDataChanged);
    }

    public override void LoadViewModel(InstanceCreateViewModel viewModel)
    {
        ViewModel = viewModel;
        ModelsUIManager.Instance.Gizmo.Instance = viewModel.Instance;
    }

    private void OnDataChanged(string value)
    {
        if (double.TryParse(inputLatitude.text, out double latitude)
            && double.TryParse(inputLongitude.text, out double longitude)
            && double.TryParse(inputAltitude.text, out double altitude))
        {
            ViewModel.Instance.Location.Position = new ArcGISPoint(latitude, longitude, altitude);
        }

        if (double.TryParse(inputPitch.text, out double pitch)
            && double.TryParse(inputRoll.text, out double roll)
            && double.TryParse(inputHeading.text, out double heading))
        {
            ViewModel.Instance.Location.Rotation = new ArcGISRotation(heading, pitch, roll);
        }

        if (float.TryParse(inputScale.text, out float scale))
        {
            ViewModel.Instance.transform.localScale = Vector3.one * scale;
        }
    }

    private void Update()
    {
        if (!inputLatitude.isFocused)
            inputLatitude.text = ViewModel.Instance.Location.Position.X.ToString();
        if (!inputLongitude.isFocused)
            inputLongitude.text = ViewModel.Instance.Location.Position.Y.ToString();
        if (!inputAltitude.isFocused)
            inputAltitude.text = ViewModel.Instance.Location.Position.Z.ToString();
        if (!inputPitch.isFocused)
            inputPitch.text = ViewModel.Instance.Location.Rotation.Pitch.ToString();
        if (!inputRoll.isFocused)
            inputRoll.text = ViewModel.Instance.Location.Rotation.Roll.ToString();
        if (!inputHeading.isFocused)
            inputHeading.text = ViewModel.Instance.Location.Rotation.Heading.ToString();
        if (!inputScale.isFocused)
            inputScale.text = ViewModel.Instance.transform.localScale.x.ToString();
    }

    private bool ValidateViewModel()
    {
        return true;
    }

    public void OnAddInstanceClick()
    {
        if (!ValidateViewModel())
            return;
        DisablePage();

        try
        {
            ViewModel.Latitude = double.Parse(inputLatitude.text);
            ViewModel.Longitude = double.Parse(inputLongitude.text);
            ViewModel.Altitude = double.Parse(inputAltitude.text);
            ViewModel.Pitch = double.Parse(inputPitch.text);
            ViewModel.Roll = double.Parse(inputRoll.text);
            ViewModel.Heading = double.Parse(inputHeading.text);
            ViewModel.Scale = float.Parse(inputScale.text);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            ShowError(ex.Message);
            EnablePage();
            return;
        }

        ModelsModule.Instance.CreateInstance(ViewModel, onSuccess, OnError);

        void onSuccess(InstanceCreateResponseJson response)
        {
            Debug.Log($"Succesfully created instance {response.Id}");
            BackPage();
        }
    }
    private void OnError(string message)
    {
        ShowError(message);
        Debug.LogError(message);
        EnablePage();
    }

    public override void OpenPage()
    {
        base.OpenPage();
        ModelsUIManager.Instance.Gizmo.gameObject.SetActive(true);
    }

    public override void ClosePage()
    {
        base.ClosePage();

        ModelsUIManager.Instance.Gizmo.gameObject.SetActive(false);
        ModelsUIManager.Instance.Gizmo.Instance = null;

        if (ViewModel != null && ViewModel.Instance != null)
        {
            Destroy(ViewModel.Instance.gameObject);
        }
    }

    public override void BackPage()
    {
        base.BackPage();
        ModelsUIManager.Instance.PageInstances.OpenPage();
        ModelsUIManager.Instance.PageInstances.OnRefreshClick();
    }
}
