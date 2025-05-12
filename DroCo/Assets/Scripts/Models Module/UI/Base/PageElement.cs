using UnityEngine;

public abstract class PageElement<TViewModel> : MonoBehaviour
{
    private IPage page = null;
    public IPage Page
    {
        get
        {
            if (this.page != null)
                return this.page;
            IPage page = gameObject.GetComponentInParent<IPage>();
            if (page == null)
                throw new MissingComponentException("Page Element doesn't have a parent game object that contains Page script.");
            return this.page = page;
        }
    }
    public TViewModel ViewModel;

    public abstract void LoadData(TViewModel data);
}
