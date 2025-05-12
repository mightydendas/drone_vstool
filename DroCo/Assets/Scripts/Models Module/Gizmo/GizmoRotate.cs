using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider), typeof(Renderer))]
public class GizmoRotate : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 0.2f;

    [SerializeField]
    private Vector3 rotationAxis;

    private Camera mainCamera;
    private Gizmo gizmo;
    private new Renderer renderer;

    private bool isDragging = false;
    private bool isHovering = false;
    private Vector2 lastMousePos;
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
        gizmo.ClickAction.action.canceled += OnClickReleased;
    }

    private void OnDisable()
    {
        gizmo.ClickAction.action.started -= OnClickStarted;
        gizmo.ClickAction.action.canceled -= OnClickReleased;
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
                lastMousePos = gizmo.PointAction.action.ReadValue<Vector2>();
            }
        }
    }

    private void OnClickReleased(InputAction.CallbackContext context)
    {
        isDragging = false;
        gizmo.ArcGisCamera.enabled = true;
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
            Vector2 currentMousePos = gizmo.PointAction.action.ReadValue<Vector2>();
            Vector2 delta = currentMousePos - lastMousePos;
            float rotate = delta.x * rotationSpeed;
            gizmo.Instance.transform.Rotate(rotationAxis, rotate, Space.World);
            lastMousePos = currentMousePos;
        }
    }

    private void ChangeColor(Color color)
    {
        renderer.material.color = color;
    }
}
