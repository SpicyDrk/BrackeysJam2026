using UnityEngine;

/// <summary>
/// Represents a toggleable anomaly point in a corridor segment.
/// Can be a GameObject that appears/disappears, changes material, etc.
/// </summary>
public class AnomalyAnchor : MonoBehaviour
{
    [Header("Anomaly Configuration")]
    [SerializeField] private AnomalyType anomalyType = AnomalyType.ToggleObject;
    [SerializeField] private GameObject targetObject; // Object to toggle/modify
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material anomalyMaterial;

    private Renderer targetRenderer;
    private bool isActive = false;

    public enum AnomalyType
    {
        ToggleObject,      // Show/hide object
        ChangeMaterial,    // Swap material
        ChangePosition,    // Move object
        ChangeScale        // Scale object
    }

    private void Awake()
    {
        if (targetObject != null && anomalyType == AnomalyType.ChangeMaterial)
        {
            targetRenderer = targetObject.GetComponent<Renderer>();
        }

        // Start in normal state
        SetActive(false);
    }

    public void SetActive(bool active)
    {
        isActive = active;
        ApplyAnomalyState();
    }

    private void ApplyAnomalyState()
    {
        if (targetObject == null) return;

        switch (anomalyType)
        {
            case AnomalyType.ToggleObject:
                targetObject.SetActive(isActive);
                break;

            case AnomalyType.ChangeMaterial:
                if (targetRenderer != null)
                {
                    targetRenderer.material = isActive ? anomalyMaterial : normalMaterial;
                }
                break;

            case AnomalyType.ChangePosition:
                // Implement position change logic
                break;

            case AnomalyType.ChangeScale:
                // Implement scale change logic
                break;
        }
    }

    public bool IsActive() => isActive;
}
