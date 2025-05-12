using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider), typeof(Renderer))]
public class GizmoMove : MonoBehaviour
{
    [SerializeField]
    private Vector3 dragAxis;

    private Camera mainCamera;
    private Gizmo gizmo;
    private new Renderer renderer;

    private bool isDragging = false;
    private bool isHovering = false;
    private Vector3 lastMouseWorldPos;
    private Color baseColor;

    private void Awake()
    {
        mainCamera = Camera.main;
        gizmo = GetComponentInParent<Gizmo>();
        if (gizmo == null)
            throw new MissingComponentException($"Missing required component {nameof(Gizmo)} in parent");
        renderer = GetComponent<Renderer>();
        baseColor = renderer.material.color;
    }

    private void OnEnable()
    {
        gizmo.ClickAction.action.started += OnClickStarted;
        gizmo.ClickAction.action.canceled += OnClickCanceled;
    }

    private void OnDisable()
    {
        gizmo.ClickAction.action.started -= OnClickStarted;
        gizmo.ClickAction.action.canceled -= OnClickCanceled;
    }

    private void OnClickStarted(InputAction.CallbackContext context)
    {
        Vector2 mousePos = gizmo.PointAction.action.ReadValue<Vector2>();
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.red, 0.5f);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, gizmo.GizmoLayerMask))
        {
            if (hit.transform == transform)
            {
                isDragging = true;
                gizmo.ArcGisCamera.enabled = false;
                lastMouseWorldPos = GetMouseWorldOnPlane();
            }
        }
    }

    private void OnClickCanceled(InputAction.CallbackContext context)
    {
        if (isDragging)
        {
            gizmo.ArcGisCamera.enabled = true;
            isDragging = false;
        }
    }
    
    private void Update()
    {
        if (gizmo.Instance == null)
            return;

        Vector2 mousePos = gizmo.PointAction.action.ReadValue<Vector2>();
        Ray ray = mainCamera.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, gizmo.GizmoLayerMask))
        {
            if (hit.transform == transform)
            {
                if (!isHovering && gizmo.CanHighlight)
                {
                    isHovering = true;
                    gizmo.CanHighlight = false;
                    ChangeColor(new Color(baseColor.r, baseColor.g, baseColor.b, 0.8f));
                }
            }
            else
            {
                if (isHovering && !isDragging)
                {
                    isHovering = false;
                    gizmo.CanHighlight = true;
                    ChangeColor(baseColor);
                }
            }
        }
        else
        {
            if (isHovering && !isDragging)
            {
                isHovering = false;
                gizmo.CanHighlight = true;
                ChangeColor(baseColor);
            }
        }

        if (isDragging)
        {
            Vector3 currentMouseWorld = GetMouseWorldOnPlane();
            Vector3 delta = currentMouseWorld - lastMouseWorldPos;
            Vector3 projected = Vector3.Project(delta, dragAxis.normalized);
            gizmo.Instance.transform.position += projected;
            lastMouseWorldPos = currentMouseWorld;
        }
    }

    private void ChangeColor(Color color)
    {
        renderer.material.color = color;
    }

    private Vector3 GetMouseWorldOnPlane()
    {
        // nefunguje kdyz mainCamera.transform.forward je stejny vector jak dragAxis
        // kdyz je mirne odlisny, tak funguje ale spatne

        Vector2 mousePos = gizmo.PointAction.action.ReadValue<Vector2>();
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        Plane plane = new Plane(mainCamera.transform.forward * -1, transform.position);

        if (plane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }

        return transform.position;
    }
}
