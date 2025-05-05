using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public abstract class Page<TViewModel> : MonoBehaviour, IPage
{
    public TViewModel ViewModel;

    [SerializeField]
    private Button buttonBack;

    [SerializeField]
    private Button buttonClose;

    [SerializeField]
    private TMP_Text textStatus;

    private CanvasGroup canvasGroup;

    protected void AwakeBase()
    {
        if (buttonBack == null)
            throw new MissingReferenceException($"Missing {nameof(buttonBack)} reference.");
        if (buttonClose == null)
            throw new MissingReferenceException($"Missing {nameof(buttonClose)} reference.");
        if (textStatus == null)
            throw new MissingReferenceException($"Missing {nameof(textStatus)} reference.");
        if ((canvasGroup = GetComponent<CanvasGroup>()) == null)
            throw new MissingComponentException($"Missing {nameof(canvasGroup)} component.");

        buttonBack.onClick.RemoveListener(BackPage);
        buttonBack.onClick.AddListener(BackPage);

        buttonClose.onClick.RemoveListener(CloseUI);
        buttonClose.onClick.AddListener(CloseUI);
    }

    public void DisablePage()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.interactable = false;
    }

    public void EnablePage()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.interactable = true;
    }

    public void ShowError(string error)
    {
        textStatus.color = Color.red;
        textStatus.gameObject.SetActive(true);
        textStatus.text = error;
    }

    public void ShowStatus(string status)
    {
        textStatus.color = Color.black;
        textStatus.gameObject.SetActive(true);
        textStatus.text = status;
    }

    public void ClearStatus()
    {
        textStatus.gameObject.SetActive(false);
        textStatus.text = "";
    }

    public abstract void LoadViewModel(TViewModel viewModel);

    public virtual void OpenPage()
    {
        EnablePage();
        ClearStatus();
        gameObject.SetActive(true);
    }

    protected void CloseUI()
    {
        ModelsUIManager.Instance.CloseUI();
    }

    public virtual void ClosePage()
    {
        ClearStatus();
        gameObject.SetActive(false);
    }

    public virtual void BackPage()
    {
        ClosePage();
    }
}
