using UnityEngine;

/// <summary>
/// Placed at the end of the straight section (before the exit curve).
/// Detects if player continues forward or turns back.
/// </summary>
public class DecisionTrigger : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float forwardThreshold = 3f; // Distance to count as "went forward"
    [SerializeField] private bool debugMode = true;

    private bool hasDecided = false;
    private Vector3 playerEntryPosition;
    private Vector3 playerEntryForward;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasDecided)
        {
            playerEntryPosition = other.transform.position;
            playerEntryForward = transform.forward;
            Log("Player entered decision zone");
            
            HallwayManager.Instance?.OnPlayerReachedDecisionPoint();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !hasDecided)
        {
            Vector3 exitPosition = other.transform.position;
            Vector3 displacement = exitPosition - playerEntryPosition;
            float forwardDistance = Vector3.Dot(displacement, playerEntryForward);

            bool wentForward = forwardDistance > 0;

            Log($"Player exited: {(wentForward ? "FORWARD" : "BACKWARD")} (distance: {forwardDistance:F2})");

            hasDecided = true;
            HallwayManager.Instance?.OnPlayerDecision(wentForward);

            // Reset after delay
            Invoke(nameof(ResetDecision), 2f);
        }
    }

    private void ResetDecision()
    {
        hasDecided = false;
    }

    private void Log(string message)
    {
        if (debugMode)
            Debug.Log($"[DecisionTrigger] {message}");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider>()?.size ?? Vector3.one);
    }
}
