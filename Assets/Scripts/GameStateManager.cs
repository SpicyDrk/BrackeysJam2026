using UnityEngine;

/// <summary>
/// Manages overall game state: streak tracking, win/lose conditions, difficulty progression.
/// </summary>
public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    [Header("Win Condition")]
    [SerializeField] private int requiredStreak = 8; // Number of correct choices to win

    [Header("State")]
    [SerializeField] private int currentStreak = 0;
    [SerializeField] private int totalChoices = 0;
    [SerializeField] private int correctChoices = 0;

    [Header("Debug")]
    [SerializeField] private bool debugMode = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnPlayerChoice(bool wasCorrect, bool hadAnomaly)
    {
        totalChoices++;

        if (wasCorrect)
        {
            correctChoices++;
            currentStreak++;
            Log($"CORRECT! Streak: {currentStreak}/{requiredStreak}");

            // Check win condition
            if (currentStreak >= requiredStreak)
            {
                OnGameWon();
            }
        }
        else
        {
            Log($"WRONG! Streak reset from {currentStreak} to 0");
            currentStreak = 0;
            OnStreakBroken();
        }

        // Update UI (we'll create this later)
        UpdateUI();
    }

    private void OnGameWon()
    {
        Log("GAME WON! Player escaped the corridor!");
        // TODO: Trigger ending sequence
        // For now, just log
        Debug.Log("<color=green>===== YOU WIN! =====</color>");
    }

    private void OnStreakBroken()
    {
        // Optional: Apply penalty (move player deeper, increase difficulty, etc.)
        Log("Streak broken - continuing...");
    }

    private void UpdateUI()
    {
        // TODO: Update UI canvas with current streak
        // For now, we'll just use Debug.Log
    }

    public int GetCurrentStreak() => currentStreak;
    public int GetRequiredStreak() => requiredStreak;
    public int GetTotalChoices() => totalChoices;
    public int GetCorrectChoices() => correctChoices;
    public float GetAccuracy() => totalChoices > 0 ? (float)correctChoices / totalChoices : 0f;

    private void Log(string message)
    {
        if (debugMode)
        {
            Debug.Log($"[GameState] {message}");
        }
    }

    private void OnGUI()
    {
        if (!debugMode) return;

        // Simple on-screen debug display
        GUI.Box(new Rect(10, 10, 250, 120), "Game State");
        GUI.Label(new Rect(20, 35, 230, 20), $"Streak: {currentStreak} / {requiredStreak}");
        GUI.Label(new Rect(20, 55, 230, 20), $"Total Choices: {totalChoices}");
        GUI.Label(new Rect(20, 75, 230, 20), $"Correct: {correctChoices}");
        GUI.Label(new Rect(20, 95, 230, 20), $"Accuracy: {GetAccuracy():P0}");
    }
}
