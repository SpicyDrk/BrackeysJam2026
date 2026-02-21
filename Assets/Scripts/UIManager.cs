using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Manages UI elements for gameplay feedback and testing.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Feedback Display")]
    [Tooltip("Text element for showing correct/incorrect feedback")]
    public TextMeshProUGUI feedbackText;
    [Tooltip("How long feedback stays on screen")]
    public float feedbackDuration = 2f;
    [Tooltip("Color for correct feedback")]
    public Color correctColor = new Color(0.3f, 1f, 0.3f, 1f);
    [Tooltip("Color for incorrect feedback")]
    public Color incorrectColor = new Color(1f, 0.3f, 0.3f, 1f);

    [Header("Streak Display")]
    [Tooltip("Text element for showing current streak")]
    public TextMeshProUGUI streakText;
    [Tooltip("Text element for showing best streak")]
    public TextMeshProUGUI bestStreakText;

    [Header("Debug/Testing Display")]
    [Tooltip("Text element for debug info (can be toggled off for release)")]
    public TextMeshProUGUI debugText;
    public bool showDebugInfo = true;

    [Header("Game State Display")]
    [Tooltip("Text showing current hallway matches reference or not")]
    public TextMeshProUGUI hallwayStateText;

    private Coroutine feedbackCoroutine;
    private static UIManager instance;
    public static UIManager Instance => instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        ClearFeedback();
        UpdateStreak(0);
        
        if (debugText != null)
            debugText.gameObject.SetActive(showDebugInfo);
    }

    public void ShowCorrectFeedback()
    {
        ShowFeedback("CORRECT!", correctColor);
    }

    public void ShowIncorrectFeedback()
    {
        ShowFeedback("WRONG!", incorrectColor);
    }

    public void ShowFeedback(string message, Color color)
    {
        if (feedbackText == null) return;

        if (feedbackCoroutine != null)
        {
            StopCoroutine(feedbackCoroutine);
        }

        feedbackText.text = message;
        feedbackText.color = color;
        feedbackText.gameObject.SetActive(true);

        feedbackCoroutine = StartCoroutine(HideFeedbackAfterDelay());
    }

    private IEnumerator HideFeedbackAfterDelay()
    {
        yield return new WaitForSeconds(feedbackDuration);
        ClearFeedback();
    }

    public void ClearFeedback()
    {
        if (feedbackText != null)
        {
            feedbackText.text = "";
            feedbackText.gameObject.SetActive(false);
        }
    }

    public void UpdateStreak(int currentStreak, int bestStreak = -1)
    {
        if (streakText != null)
        {
            streakText.text = $"Streak: {currentStreak}";
        }

        if (bestStreak >= 0 && bestStreakText != null)
        {
            bestStreakText.text = $"Best: {bestStreak}";
        }
    }

    public void UpdateDebugInfo(string info)
    {
        if (debugText != null && showDebugInfo)
        {
            debugText.text = info;
        }
    }

    public void UpdateHallwayState(bool matchesReference)
    {
        if (hallwayStateText != null)
        {
            hallwayStateText.text = matchesReference ? "SAME" : "DIFFERENT";
            hallwayStateText.color = matchesReference ? correctColor : incorrectColor;
        }
    }

    public void ShowWinScreen()
    {
        ShowFeedback("YOU WIN!", correctColor);
        // Note: Gameplay UI will be hidden when ending sequence starts
    }

    public void HideGameplayUI()
    {
        // Hide gameplay UI elements for ending sequence
        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);
        if (streakText != null)
            streakText.gameObject.SetActive(false);
        if (bestStreakText != null)
            bestStreakText.gameObject.SetActive(false);
        if (debugText != null)
            debugText.gameObject.SetActive(false);
        if (hallwayStateText != null)
            hallwayStateText.gameObject.SetActive(false);
    }

    public void ToggleDebugInfo()
    {
        showDebugInfo = !showDebugInfo;
        if (debugText != null)
        {
            debugText.gameObject.SetActive(showDebugInfo);
        }
    }
}
