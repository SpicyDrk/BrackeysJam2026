using UnityEngine;
using UnityEngine.UI;
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

    [Header("Fade In Settings")]
    [Tooltip("White panel for fade-in effect at scene start")]
    public Image whiteFadePanel;
    [Tooltip("Duration of fade-in from white")]
    public float fadeInDuration = 1.5f;
    [Tooltip("Enable fade-in at scene start")]
    public bool fadeInOnStart = true;
    [Tooltip("Initial hint text shown during fade-in")]
    public TextMeshProUGUI initialHintText;
    [Tooltip("Message shown at game start")]
    public string initialHintMessage = "I should remember everything in this hallway...";

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

        // Fade in from white at scene start
        if (fadeInOnStart && whiteFadePanel != null)
        {
            StartCoroutine(FadeInFromWhite());
        }
    }

    private IEnumerator FadeInFromWhite()
    {
        // Start with white panel fully opaque
        Color color = whiteFadePanel.color;
        color.a = 1f;
        whiteFadePanel.color = color;
        whiteFadePanel.gameObject.SetActive(true);

        // Show initial hint text
        if (initialHintText != null)
        {
            initialHintText.text = initialHintMessage;
            initialHintText.gameObject.SetActive(true);
            Color textColor = initialHintText.color;
            textColor.a = 1f;
            initialHintText.color = textColor;
        }

        yield return new WaitForSeconds(2f); // Show hint for 2 seconds

        // Fade out hint text
        if (initialHintText != null)
        {
            float elapsed = 0f;
            float textFadeDuration = 0.5f;
            Color textColor = initialHintText.color;
            
            while (elapsed < textFadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / textFadeDuration;
                textColor.a = Mathf.Lerp(1f, 0f, t);
                initialHintText.color = textColor;
                yield return null;
            }
            
            initialHintText.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(0.2f); // Brief delay before fading background

        // Fade from white to transparent
        float elapsed2 = 0f;
        while (elapsed2 < fadeInDuration)
        {
            elapsed2 += Time.deltaTime;
            float t = elapsed2 / fadeInDuration;
            color.a = Mathf.Lerp(1f, 0f, t);
            whiteFadePanel.color = color;
            yield return null;
        }

        color.a = 0f;
        whiteFadePanel.color = color;
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

    /// <summary>
    /// Fades the white panel from current state to target alpha
    /// </summary>
    public IEnumerator FadeWhitePanel(float startAlpha, float endAlpha, float duration)
    {
        if (whiteFadePanel == null) yield break;

        float elapsed = 0f;
        Color color = whiteFadePanel.color;
        color.a = startAlpha;
        whiteFadePanel.color = color;
        whiteFadePanel.gameObject.SetActive(true);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            whiteFadePanel.color = color;
            yield return null;
        }

        color.a = endAlpha;
        whiteFadePanel.color = color;
    }
}
