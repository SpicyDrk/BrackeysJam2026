using UnityEngine;

/// <summary>
/// Manages a single static hallway segment.
/// Changes props each loop and teleports player back to start.
/// </summary>
public class HallwayManager : MonoBehaviour
{
    public static HallwayManager Instance { get; private set; }

    [Header("Hallway Setup")]
    [SerializeField] private HallwaySegment hallwaySegment; // Reference to the static hallway in scene
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerSpawnPoint; // Where to teleport player back to (for forward decision)
    [SerializeField] private Transform backwardSpawnPoint; // Where to teleport player when going backward (just before decision plane)
    
    [Header("Decision Detection")]
    [SerializeField] private Transform forwardDecisionPlane; // Position this where forward decision should trigger
    [SerializeField] private Transform backwardDecisionPlane; // Position at symmetry point where backward decision triggers
    [SerializeField] private float decisionTolerance = 0.1f; // How far past plane to trigger decision (small value for precision)

    [Header("Reference Configuration")]
    [SerializeField] private HallwayConfiguration referenceConfig;

    [Header("Ending Sequence")]
    [Tooltip("GameObjects to disable when player wins (e.g., normal hallway, decision planes)")]
    [SerializeField] private GameObject[] disableOnWin;
    [Tooltip("GameObjects to enable when player wins (e.g., perfect corridor, ending door)")]
    [SerializeField] private GameObject[] enableOnWin;

    [Header("Runtime State")]
    private HallwayConfiguration currentConfig;
    private bool currentMatchesReference = true;
    private float previousForwardDistance;
    private float previousBackwardDistance;
    private bool decisionMade = false;
    private bool hasCompletedFirstPass = false; // Track if player has seen the reference hallway

    [Header("Game Progress")]
    private int currentStreak = 0;
    private int bestStreak = 0;
    private const int STREAK_TO_WIN = 8;

    [Header("Debug")]
    [SerializeField] private bool debugMode = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("[HallwayManager] Instance set");
        }
        else
            Destroy(gameObject);

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player != null)
                Debug.Log($"[HallwayManager] Found player: {player.name}");
            else
                Debug.LogWarning("[HallwayManager] Player not found! Make sure Player has 'Player' tag.");
        }
    }

    private void Start()
    {
        InitializeHallway();
        
        if (player != null)
        {
            if (forwardDecisionPlane != null)
            {
                // Use world X-axis to detect crossing
                previousForwardDistance = player.position.x - forwardDecisionPlane.position.x;
                Log($"Initial forward plane distance (X): {previousForwardDistance:F2}");
            }
            
            if (backwardDecisionPlane != null)
            {
                // Use world X-axis to detect crossing
                previousBackwardDistance = player.position.x - backwardDecisionPlane.position.x;
                Log($"Initial backward plane distance (X): {previousBackwardDistance:F2}");
            }
        }
    }

    private void Update()
    {
        // Don't process decisions while paused
        if (PauseManager.Instance != null && PauseManager.Instance.IsPaused())
            return;

        if (player == null || decisionMade) return;

        // Check forward plane (detect crossing along world X axis)
        if (forwardDecisionPlane != null)
        {
            float currentDistance = player.position.x - forwardDecisionPlane.position.x;

            // Player crossed forward plane (from positive X to negative X, moving left)
            if (previousForwardDistance > 0 && currentDistance <= 0)
            {
                Log($"Player crossed FORWARD plane (X)! Distance: {previousForwardDistance:F2} -> {currentDistance:F2}");
                OnPlayerDecision(wentForward: true);
            }

            previousForwardDistance = currentDistance;
        }

        // Check backward plane (detect crossing along world X axis)
        if (backwardDecisionPlane != null)
        {
            float currentDistance = player.position.x - backwardDecisionPlane.position.x;

            // Player crossed backward plane (from negative X to positive X, moving right/backward)
            if (previousBackwardDistance < 0 && currentDistance >= 0)
            {
                Log($"Player crossed BACKWARD plane (X)! Distance: {previousBackwardDistance:F2} -> {currentDistance:F2}");
                OnPlayerDecision(wentForward: false);
            }

            previousBackwardDistance = currentDistance;
        }
    }

    private void InitializeHallway()
    {
        if (hallwaySegment == null)
        {
            Debug.LogError("HallwayManager: No hallway segment assigned!");
            return;
        }

        // Set up initial reference hallway
        currentConfig = referenceConfig.Clone();
        hallwaySegment.ApplyConfiguration(currentConfig);
        currentMatchesReference = true;

        Log("Hallway initialized with reference configuration");
    }

    /// <summary>
    /// Called when player makes a decision at the threshold
    /// </summary>
    public void OnPlayerDecision(bool wentForward)
    {
        if (decisionMade) return;
        decisionMade = true;

        // Mark first pass complete when player crosses forward plane
        if (wentForward && !hasCompletedFirstPass)
        {
            hasCompletedFirstPass = true;
            Log("First pass completed - randomization enabled for next loops");
        }

        // Correct action: If same → go forward, If different → go back
        bool correctAction = (currentMatchesReference && wentForward) || (!currentMatchesReference && !wentForward);

        Log($"Player went {(wentForward ? "FORWARD" : "BACK")} | Current matches reference: {currentMatchesReference} | Correct: {correctAction}");

        if (correctAction)
        {
            OnCorrectChoice();
        }
        else
        {
            OnIncorrectChoice();
        }

        // Teleport player and prepare next loop
        TeleportPlayer(wentForward);
        PrepareNextLoop();
        
        // Reset decision flag for next loop
        decisionMade = false;
    }

    private void OnCorrectChoice()
    {
        Log("CORRECT!");
        
        // Increment streak
        currentStreak++;
        if (currentStreak > bestStreak)
        {
            bestStreak = currentStreak;
        }

        // Update UI
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowCorrectFeedback();
            UIManager.Instance.UpdateStreak(currentStreak, bestStreak);
        }

        // Play sound
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayCorrectSound();
        }

        // Check win condition
        if (currentStreak >= STREAK_TO_WIN)
        {
            OnWin();
        }
    }

    private void OnIncorrectChoice()
    {
        Log("WRONG!");
        
        // Reset streak
        currentStreak = 0;

        // Update UI
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowIncorrectFeedback();
            UIManager.Instance.UpdateStreak(currentStreak, bestStreak);
        }

        // Play sound
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayIncorrectSound();
        }
    }

    private void OnWin()
    {
        Log($"YOU WIN! Reached {STREAK_TO_WIN} correct choices!");
        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowWinScreen();
        }

        // Trigger the ending sequence
        TriggerEndingSequence();
    }

    private void TriggerEndingSequence()
    {
        Log("Triggering ending sequence - transitioning to perfect corridor");

        // Fade out ambient music and atmospheric sounds (keeps footsteps active)
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.FadeOutAmbientSounds(fadeTime: 3f);
            // Also force stop all ambient audio sources to catch any rogue sources
            SoundManager.Instance.StopAllAmbientAudioSources();
            Log("Fading out ambient and atmospheric sounds");
        }

        // Disable normal gameplay objects (hallway, decision planes, etc.)
        foreach (GameObject obj in disableOnWin)
        {
            if (obj != null)
            {
                obj.SetActive(false);
                Log($"Disabled: {obj.name}");
            }
        }

        // Enable ending objects (perfect corridor, ending door)
        foreach (GameObject obj in enableOnWin)
        {
            if (obj != null)
            {
                obj.SetActive(true);
                Log($"Enabled: {obj.name}");
            }
        }

        // Notify ending sequence controller to start
        EndingSequence endingSequence = FindFirstObjectByType<EndingSequence>();
        if (endingSequence != null)
        {
            endingSequence.StartEnding();
        }
    }

    private void TeleportPlayer(bool wentForward)
    {
        if (player == null)
        {
            Debug.LogWarning("HallwayManager: Player not found, cannot teleport");
            return;
        }

        // Store current rotation and position
        Quaternion currentRotation = player.rotation;
        Vector3 currentPosition = player.position;
        
        // Disable CharacterController temporarily to prevent physics conflicts
        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
            controller.enabled = false;
        
        Vector3 newPosition;
        Quaternion newRotation;
        
        if (wentForward)
        {
            // Forward decision: Teleport to spawn, preserve lateral Z offset from forward plane
            if (playerSpawnPoint == null || forwardDecisionPlane == null)
            {
                Debug.LogWarning("HallwayManager: Player spawn point or forward plane not set!");
                return;
            }
            
            // Calculate Z offset from forward decision plane, apply to spawn point
            float lateralOffset = currentPosition.z - forwardDecisionPlane.position.z;
            newPosition = playerSpawnPoint.position;
            newPosition.y = currentPosition.y; // Preserve height
            newPosition.z = playerSpawnPoint.position.z + lateralOffset; // Preserve lateral position
            newRotation = currentRotation; // Keep current rotation for seamless feel
            
            Log($"FORWARD teleport: {currentPosition} -> {newPosition} (lateral: {lateralOffset:F2})");
        }
        else
        {
            // Backward decision: Teleport to backward spawn, FLIP Z offset, rotate 180°
            if (backwardSpawnPoint == null || backwardDecisionPlane == null)
            {
                Debug.LogWarning("HallwayManager: Backward spawn point or backward plane not set!");
                return;
            }
            
            // Calculate Z offset from backward plane, flip for 180° rotation, apply to spawn point
            float lateralOffset = currentPosition.z - backwardDecisionPlane.position.z;
            newPosition = backwardSpawnPoint.position;
            newPosition.y = currentPosition.y; // Preserve height
            newPosition.z = backwardSpawnPoint.position.z - lateralOffset; // Flip lateral for rotation
            
            // Rotate 180° around Y axis so player now faces forward again
            newRotation = currentRotation * Quaternion.Euler(0, 180, 0);
            
            Log($"BACKWARD teleport + 180° rotation: {currentPosition} -> {newPosition} (lateral: {-lateralOffset:F2})");
            Log($"Rotation: {currentRotation.eulerAngles} -> {newRotation.eulerAngles}");
        }
        
        player.position = newPosition;
        player.rotation = newRotation;
        
        // Re-enable CharacterController
        if (controller != null)
            controller.enabled = true;

        // Update distance trackers to new position (using world X-axis)
        if (forwardDecisionPlane != null)
        {
            previousForwardDistance = player.position.x - forwardDecisionPlane.position.x;
        }
        
        if (backwardDecisionPlane != null)
        {
            previousBackwardDistance = player.position.x - backwardDecisionPlane.position.x;
        }
    }

    private void PrepareNextLoop()
    {
        // First pass: Always show reference hallway so player can memorize it
        // After first pass: Randomly decide if next loop matches reference or is different
        bool shouldMatch;
        
        if (!hasCompletedFirstPass)
        {
            shouldMatch = true; // Always match on first pass
            Log("First pass - showing reference hallway");
        }
        else
        {
            shouldMatch = Random.value > 0.5f; // 50/50 for subsequent passes
        }

        Log($"=== PrepareNextLoop: shouldMatch={shouldMatch} ===");

        if (shouldMatch)
        {
            currentConfig = referenceConfig.Clone();
            currentMatchesReference = true;
            Log("Cloning REFERENCE config (should be SAME)");
        }
        else
        {
            Log("Creating VARIATION (should be DIFFERENT)");
            currentConfig = HallwayConfiguration.CreateRandomVariation(referenceConfig);
            currentMatchesReference = false;
            
            // Verify it's actually different
            bool actuallyDifferent = !currentConfig.IsIdenticalTo(referenceConfig);
            Log($"Variation created - Actually different from reference: {actuallyDifferent}");
            if (!actuallyDifferent)
            {
                Debug.LogError("[HallwayManager] BUG: Variation is identical to reference!");
            }
        }

        // Apply configuration to hallway
        hallwaySegment.ClearAllProps();
        hallwaySegment.ApplyConfiguration(currentConfig);

        Log($"Next hallway: {(shouldMatch ? "SAME" : "DIFFERENT")}");

        // Update UI to show current state (for testing/debug)
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHallwayState(currentMatchesReference);
        }
    }

    public bool CurrentMatchesReference() => currentMatchesReference;

    private void Log(string message)
    {
        if (debugMode)
            Debug.Log($"[HallwayManager] {message}");
    }
}
