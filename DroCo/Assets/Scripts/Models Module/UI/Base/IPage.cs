
public interface IPage
{
    void DisablePage();
    void EnablePage();
    void ShowError(string error);
    void ShowStatus(string status);
    void ClearStatus();
    void OpenPage();
    void ClosePage();
}
