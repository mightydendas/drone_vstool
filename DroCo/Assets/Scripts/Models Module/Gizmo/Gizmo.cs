using Esri.ArcGISMapsSDK.Components;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ArcGISLocationComponent))]
public class Gizmo : MonoBehaviour
{
    [SerializeField]
    private GameObject moveGizmo;

    [SerializeField]
    private GameObject rotateGizmo;

    public MonoBehaviour ArcGisCamera;

    public InputActionReference ClickAction;

    public InputActionReference PointAction;

    [HideInInspector]
    public bool CanHighlight = true;

    [HideInInspector]
    public Instance Instance;

    [HideInInspector]
    public int GizmoLayerMask;

    private void Awake()
    {
        GizmoLayerMask = LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer));
    }

    void OnEnable()
    {
        ClickAction.action.Enable();
        PointAction.action.Enable();
    }

    void OnDisable()
    {
        ClickAction.action.Disable();
        PointAction.action.Disable();
    }

    void Update()
    {
        if (Instance == null)
            return;

        transform.position = Instance.transform.position;
    }

    public void ShowRotateGizmo()
    {
        moveGizmo.SetActive(false);
        rotateGizmo.SetActive(true);
    }

    public void ShowMoveGizmo()
    {
        moveGizmo.SetActive(true);
        rotateGizmo.SetActive(false);
    }
}
