using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Handles the ending sequence when player reaches the perfect corridor.
/// </summary>
public class EndingSequence : MonoBehaviour
{
    [Header("Door Interaction")]
    [Tooltip("Trigger that detects when player reaches the door")]
    public Collider doorTrigger;
    [Tooltip("How close player needs to be to door to interact")]
    public float interactionDistance = 3f;
    [Tooltip("Transform of the door (for distance checking)")]
    public Transform doorTransform;

    [Header("Fade Panels")]
    [Tooltip("White panel for fade to white void")]
    public Image whiteFadePanel;
    [Tooltip("Black panel for final fade to black")]
    public Image blackFadePanel;

    [Header("End Text")]
    [Tooltip("Text displayed at the end")]
    public TextMeshProUGUI endText;
    [Tooltip("The final message")]
    public string finalMessage = "You are still walking.";

    [Header("Timing")]
    [Tooltip("How long to fade to white")]
    public float fadeToWhiteDuration = 3f;
    [Tooltip("How long to stay in white void")]
    public float whiteVoidDuration = 2f;
    [Tooltip("How long to fade to black")]
    public float fadeToBlackDuration = 4f;
    [Tooltip("How long text stays visible")]
    public float textDisplayDuration = 5f;

    private bool endingStarted = false;
    private bool endingTriggered = false;
    private Transform player;

    private void Awake()
    {
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // Make sure fade panels start invisible
        if (whiteFadePanel != null)
        {
            Color c = whiteFadePanel.color;
            c.a = 0f;
            whiteFadePanel.color = c;
            whiteFadePanel.gameObject.SetActive(true);
        }

        if (blackFadePanel != null)
        {
            Color c = blackFadePanel.color;
            c.a = 0f;
            blackFadePanel.color = c;
            blackFadePanel.gameObject.SetActive(true);
        }

        // Hide end text
        if (endText != null)
        {
            endText.gameObject.SetActive(false);
        }
    }

    public void StartEnding()
    {
        endingStarted = true;
        Debug.Log("[EndingSequence] Ending sequence activated - player can now reach the door");
    }

    private void Update()
    {
        if (!endingStarted || endingTriggered) return;

        // Check if player is close to the door
        if (player != null && doorTransform != null)
        {
            float distance = Vector3.Distance(player.position, doorTransform.position);
            if (distance <= interactionDistance)
            {
                TriggerEndingSequence();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Alternative trigger-based detection
        if (!endingStarted || endingTriggered) return;

        if (other.CompareTag("Player"))
        {
            TriggerEndingSequence();
        }
    }

    private void TriggerEndingSequence()
    {
        if (endingTriggered) return;
        endingTriggered = true;

        Debug.Log("[EndingSequence] Player reached door - starting final sequence");

        // Disable player movement
        PlayerController playerController = player?.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // Lock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Start the sequence
        StartCoroutine(EndingSequenceCoroutine());
    }

    private IEnumerator EndingSequenceCoroutine()
    {
        // Hide gameplay UI
        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideGameplayUI();
        }

        // Fade to white void
        yield return StartCoroutine(FadePanel(whiteFadePanel, 0f, 1f, fadeToWhiteDuration));

        // Stay in white void
        yield return new WaitForSeconds(whiteVoidDuration);

        // Fade to black
        yield return StartCoroutine(FadePanel(blackFadePanel, 0f, 1f, fadeToBlackDuration));

        // Show final text
        if (endText != null)
        {
            endText.text = finalMessage;
            endText.gameObject.SetActive(true);
        }

        // Wait for text to be read
        yield return new WaitForSeconds(textDisplayDuration);

        // Optional: Fade out text or quit
        Debug.Log("[EndingSequence] Ending complete. You are still walking.");
    }

    private IEnumerator FadePanel(Image panel, float startAlpha, float endAlpha, float duration)
    {
        if (panel == null) yield break;

        float elapsed = 0f;
        Color color = panel.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            panel.color = color;
            yield return null;
        }

        color.a = endAlpha;
        panel.color = color;
    }
}
