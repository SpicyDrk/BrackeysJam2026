using UnityEngine;

/// <summary>
/// Placed in the middle segment between the two turns.
/// If player reaches this point = went forward.
/// Triggers teleport immediately on entry.
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class DecisionTrigger : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private bool debugMode = true;

    private bool decisionMade = false;

    private void OnTriggerEnter(Collider other)
    {
        Log($"Trigger entered by: {other.gameObject.name}, Tag: {other.tag}");
        
        if (other.CompareTag("Player") && !decisionMade)
        {
            Log("Player detected! Triggering forward decision.");
            // Player reached this point = they went forward through the curves
            OnPlayerDecision(wentForward: true);
        }
        else if (!other.CompareTag("Player"))
        {
            Log($"Not player - wrong tag. Expected 'Player', got '{other.tag}'");
        }
        else if (decisionMade)
        {
            Log("Decision already made, ignoring trigger");
        }
    }

    /// <summary>
    /// Call this method when player turns back (e.g., from another trigger or detection system)
    /// </summary>
    public void TriggerBackwardDecision()
    {
        if (!decisionMade)
        {
            OnPlayerDecision(wentForward: false);
        }
    }

    private void OnPlayerDecision(bool wentForward)
    {
        if (decisionMade) return;
        
        decisionMade = true;

        Log($"Player chose: {(wentForward ? "FORWARD" : "TURN BACK")}");

        // Notify HallwayManager
        if (HallwayManager.Instance != null)
        {
            Log("Notifying HallwayManager...");
            HallwayManager.Instance.OnPlayerDecision(wentForward);
        }
        else
        {
            Debug.LogError("[DecisionTrigger] HallwayManager.Instance is NULL! Cannot notify.");
        }

        // Reset after teleport completes
        Invoke(nameof(ResetDecision), 1f);
    }

    private void ResetDecision()
    {
        decisionMade = false;
        Log("Decision trigger reset - ready for next loop");
    }

    private void Log(string message)
    {
        if (debugMode)
            Debug.Log($"[DecisionTrigger] {message}");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green; // Green = forward trigger
        BoxCollider box = GetComponent<BoxCollider>();
        if (box != null)
        {
            Gizmos.DrawWireCube(transform.position, box.size);
        }
    }
}
