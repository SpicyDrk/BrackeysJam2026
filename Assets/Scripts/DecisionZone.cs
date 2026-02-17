using UnityEngine;

/// <summary>
/// Place at the end of each corridor segment to detect if player continues forward or turns back.
/// This is where the player makes their anomaly decision.
/// </summary>
public class DecisionZone : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float forwardThreshold = 2f; // Distance past trigger to count as "forward"
    [SerializeField] private bool debugMode = true;

    private bool playerInZone = false;
    private Vector3 playerEntryPosition;
    private bool decisionMade = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !decisionMade)
        {
            playerInZone = true;
            playerEntryPosition = other.transform.position;
            Log("Player entered decision zone");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!playerInZone || decisionMade) return;
        if (!other.CompareTag("Player")) return;

        // Check if player has moved significantly forward
        Vector3 currentPosition = other.transform.position;
        float forwardDistance = Vector3.Dot(currentPosition - playerEntryPosition, transform.forward);

        if (forwardDistance > forwardThreshold)
        {
            // Player chose to continue forward
            OnPlayerDecision(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && playerInZone && !decisionMade)
        {
            // Player left the zone going backward
            Vector3 exitPosition = other.transform.position;
            float forwardDistance = Vector3.Dot(exitPosition - playerEntryPosition, transform.forward);

            if (forwardDistance < 0)
            {
                // Player turned around
                OnPlayerDecision(false);
            }

            playerInZone = false;
        }
    }

    private void OnPlayerDecision(bool wentForward)
    {
        decisionMade = true;
        playerInZone = false;

        // Get current anomaly state from CorridorManager
        bool hasAnomaly = CorridorManager.Instance?.CurrentLoopHasAnomaly() ?? false;

        // Determine if choice was correct
        bool correctChoice = (hasAnomaly && !wentForward) || (!hasAnomaly && wentForward);

        Log($"Player chose: {(wentForward ? "FORWARD" : "TURN BACK")} | Anomaly: {hasAnomaly} | Correct: {correctChoice}");

        // Notify game state manager (we'll create this later)
        GameStateManager.Instance?.OnPlayerChoice(correctChoice, hasAnomaly);

        // Reset for next use
        Invoke(nameof(ResetDecision), 1f);
    }

    private void ResetDecision()
    {
        decisionMade = false;
    }

    private void Log(string message)
    {
        if (debugMode)
        {
            Debug.Log($"[DecisionZone] {message}");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
        
        // Show forward threshold line
        Gizmos.color = Color.blue;
        Vector3 thresholdPos = transform.position + transform.forward * forwardThreshold;
        Gizmos.DrawLine(transform.position, thresholdPos);
        Gizmos.DrawWireSphere(thresholdPos, 0.3f);
    }
}
