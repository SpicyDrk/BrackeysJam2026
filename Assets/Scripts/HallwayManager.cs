using UnityEngine;

/// <summary>
/// Manages 3 hallway segments: Previous, Current, Future
/// Each segment includes entry/exit curves that seamlessly connect.
/// Handles player progression and decision logic.
/// </summary>
public class HallwayManager : MonoBehaviour
{
    public static HallwayManager Instance { get; private set; }

    [Header("Hallway Setup")]
    [SerializeField] private HallwaySegment hallwayPrefab;
    [SerializeField] private Transform player;

    [Header("Reference Configuration")]
    [SerializeField] private HallwayConfiguration referenceConfig;

    [Header("Runtime State")]
    private HallwaySegment previousSegment;
    private HallwaySegment currentSegment;
    private HallwaySegment futureSegment;

    private HallwayConfiguration currentFutureConfig;
    private bool isCorrectPath = true; // Is the current path correct?

    [Header("Debug")]
    [SerializeField] private bool debugMode = true;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Start()
    {
        InitializeHallways();
    }

    private void InitializeHallways()
    {
        if (hallwayPrefab == null)
        {
            Debug.LogError("HallwayManager: No hallway prefab assigned!");
            return;
        }

        Vector3 spawnPos = transform.position;
        Quaternion spawnRot = transform.rotation;

        // Spawn Current (reference hallway) - includes entry and exit curves
        currentSegment = Instantiate(hallwayPrefab, spawnPos, spawnRot, transform);
        currentSegment.name = "Current_Hallway";
        currentSegment.ApplyConfiguration(referenceConfig);

        // Spawn Future - position at end of current (curves will connect seamlessly)
        Vector3 futurePos = currentSegment.GetEndPosition();
        Quaternion futureRot = currentSegment.transform.rotation; // Inherit rotation from curve
        futureSegment = Instantiate(hallwayPrefab, futurePos, futureRot, transform);
        futureSegment.name = "Future_Hallway";
        
        // Future is same as reference initially
        currentFutureConfig = referenceConfig.Clone();
        futureSegment.ApplyConfiguration(currentFutureConfig);

        Log("Hallway system initialized (segments include curves at both ends)");
    }

    /// <summary>
    /// Called when player makes a decision (continue forward or turn back)
    /// </summary>
    public void OnPlayerDecision(bool wentForward)
    {
        bool futureMatchesReference = currentFutureConfig.IsIdenticalTo(referenceConfig);

        // Correct action: If same → go forward, If different → go back
        bool correctAction = (futureMatchesReference && wentForward) || (!futureMatchesReference && !wentForward);

        Log($"Player went {(wentForward ? "FORWARD" : "BACK")} | Future matches reference: {futureMatchesReference} | Correct: {correctAction}");

        if (correctAction)
        {
            OnCorrectChoice();
        }
        else
        {
            OnIncorrectChoice();
        }
    }

    private void OnCorrectChoice()
    {
        Log("CORRECT! Advancing...");
        
        // Shift segments forward
        AdvanceHallways();
    }

    private void OnIncorrectChoice()
    {
        Log("WRONG! Resetting...");
        
        // TODO: Implement penalty/reset logic
        // For now, just advance anyway
        AdvanceHallways();
    }

    private void AdvanceHallways()
    {
        // Destroy old previous if it exists (player has moved past it)
        if (previousSegment != null)
            Destroy(previousSegment.gameObject);

        // Shift: Current becomes Previous, Future becomes Current
        previousSegment = currentSegment;
        previousSegment.name = "Previous_Hallway";

        currentSegment = futureSegment;
        currentSegment.name = "Current_Hallway";

        // Create new Future - connect at end of current segment
        Vector3 futurePos = currentSegment.GetEndPosition();
        Quaternion futureRot = currentSegment.transform.rotation; // Match rotation for curve alignment
        futureSegment = Instantiate(hallwayPrefab, futurePos, futureRot);
        futureSegment.name = "Future_Hallway";
        futureSegment = Instantiate(hallwayPrefab, futurePos, transform.rotation, transform);
        futureSegment.name = "Future_Hallway";

        // Decide if future should match reference or be different
        bool shouldBeDifferent = Random.value > 0.5f; // 50/50 for now

        if (shouldBeDifferent)
        {
            currentFutureConfig = HallwayConfiguration.CreateRandomVariation(referenceConfig);
        }
        else
        {
            currentFutureConfig = referenceConfig.Clone();
        }

        futureSegment.ApplyConfiguration(currentFutureConfig);

        Log($"New future hallway: {(shouldBeDifferent ? "DIFFERENT" : "SAME")}");
    }

    /// <summary>
    /// Called when player enters the decision zone
    /// </summary>
    public void OnPlayerReachedDecisionPoint()
    {
        Log("Player reached decision point");
        // Can add UI prompt here
    }

    private void Log(string message)
    {
        if (debugMode)
            Debug.Log($"[HallwayManager] {message}");
    }
}
