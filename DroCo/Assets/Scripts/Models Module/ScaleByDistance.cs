using UnityEngine;

public class ScaleByDistance : MonoBehaviour
{
    [SerializeField]
    private Transform targetTransform;

    [SerializeField]
    private float scaleFactor = 1.0f;

    void Update()
    {
        if (targetTransform == null)
            return;

        float distance = Vector3.Distance(transform.position, targetTransform.transform.position);
        float scale = distance * scaleFactor;
        transform.localScale = Vector3.one * scale;
    }
}
