using UnityEngine;
using System.Collections.Generic;

public class CorridorSegment : MonoBehaviour
{
    [Header("Segment Configuration")]
    [SerializeField] private float segmentLength = 10f;
    [SerializeField] private Transform segmentEnd; // Mark where this segment ends
    [SerializeField] private Transform segmentStart; // Mark where this segment starts

    [Header("Detection")]
    [SerializeField] private BoxCollider entryTrigger; // Trigger when player enters segment

    [Header("Anomaly System")]
    [SerializeField] private List<AnomalyAnchor> anomalyAnchors = new List<AnomalyAnchor>();

    private bool hasAnomaly = false;
    private int segmentIndex = 0; // Track which position in the corridor chain this is

    private void Awake()
    {
        // Auto-setup entry trigger if not assigned
        if (entryTrigger == null)
        {
            entryTrigger = GetComponentInChildren<BoxCollider>();
        }

        if (entryTrigger != null)
        {
            entryTrigger.isTrigger = true;
        }
    }

    private void OnValidate()
    {
        // Auto-calculate segment length if end marker exists
        if (segmentStart != null && segmentEnd != null)
        {
            segmentLength = Vector3.Distance(segmentStart.position, segmentEnd.position);
        }
    }

    public float GetLength() => segmentLength;
    
    public Vector3 GetEndPosition()
    {
        if (segmentEnd != null)
            return segmentEnd.position;
        
        return transform.position + transform.forward * segmentLength;
    }

    public Vector3 GetStartPosition()
    {
        if (segmentStart != null)
            return segmentStart.position;
        
        return transform.position;
    }

    public void SetSegmentIndex(int index)
    {
        segmentIndex = index;
    }

    public int GetSegmentIndex() => segmentIndex;

    public void SetAnomaly(bool active)
    {
        hasAnomaly = active;
        
        // Apply anomaly to all anchors (we'll make this more sophisticated later)
        foreach (var anchor in anomalyAnchors)
        {
            if (anchor != null)
            {
                anchor.SetActive(active);
            }
        }
    }

    public bool HasAnomaly() => hasAnomaly;

    public void ClearAnomaly()
    {
        SetAnomaly(false);
    }

    // Called when player enters this segment's trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CorridorManager.Instance?.OnPlayerEnteredSegment(this);
        }
    }

    private void OnDrawGizmos()
    {
        // Visualize segment bounds
        Gizmos.color = hasAnomaly ? Color.red : Color.green;
        
        Vector3 start = GetStartPosition();
        Vector3 end = GetEndPosition();
        
        Gizmos.DrawLine(start, end);
        Gizmos.DrawWireSphere(start, 0.3f);
        Gizmos.DrawWireSphere(end, 0.3f);
    }

    private void OnDrawGizmosSelected()
    {
        // Show segment index and anomaly state
        Gizmos.color = Color.yellow;
        Vector3 labelPos = GetStartPosition() + Vector3.up * 2f;
        
#if UNITY_EDITOR
        UnityEditor.Handles.Label(labelPos, $"Segment {segmentIndex}\n{(hasAnomaly ? "ANOMALY" : "Normal")}");
#endif
    }
}
