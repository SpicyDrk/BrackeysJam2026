using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Handles the initial game start sequence with hint text and fade-in.
/// </summary>
public class GameStart : MonoBehaviour
{
    [Header("Fade Elements")]
    [Tooltip("White panel for fade-in effect")]
    public Image whiteFadePanel;
    [Tooltip("Initial hint text shown during fade-in")]
    public TextMeshProUGUI hintText;

    [Header("Settings")]
    [Tooltip("How long to show the hint")]
    public float hintDisplayTime = 2.5f;
    [Tooltip("How long to fade out the hint text")]
    public float hintFadeOutTime = 0.8f;
    [Tooltip("How long to fade from white to game")]
    public float backgroundFadeTime = 1.5f;

    private void Start()
    {
        StartCoroutine(InitialSequence());
    }

    private IEnumerator InitialSequence()
    {
        // Setup white panel (fully opaque)
        if (whiteFadePanel != null)
        {
            Color panelColor = whiteFadePanel.color;
            panelColor.a = 1f;
            whiteFadePanel.color = panelColor;
            whiteFadePanel.gameObject.SetActive(true);
            whiteFadePanel.raycastTarget = false; // Don't block input
        }

        // Setup hint text (fully visible)
        if (hintText != null)
        {
            hintText.gameObject.SetActive(true);
            hintText.alpha = 1f;
        }

        // Show hint for specified duration
        yield return new WaitForSeconds(hintDisplayTime);

        // Fade out hint text
        if (hintText != null)
        {
            float elapsed = 0f;

            while (elapsed < hintFadeOutTime)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / hintFadeOutTime);
                hintText.alpha = alpha;
                
                yield return null;
            }

            hintText.alpha = 0f;
            hintText.gameObject.SetActive(false);
        }

        // Brief pause
        yield return new WaitForSeconds(0.3f);

        // Fade out white panel to reveal game
        if (whiteFadePanel != null)
        {
            float elapsed = 0f;
            Color panelColor = whiteFadePanel.color;

            while (elapsed < backgroundFadeTime)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / backgroundFadeTime;
                panelColor.a = Mathf.Lerp(1f, 0f, t);
                whiteFadePanel.color = panelColor;
                yield return null;
            }

            // Fully transparent
            panelColor.a = 0f;
            whiteFadePanel.color = panelColor;
        }
    }
}
