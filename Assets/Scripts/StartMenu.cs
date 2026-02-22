using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StartMenu : MonoBehaviour
{
    public Button Startbutton;
    [Tooltip("White panel for fade to white effect")]
    public Image whiteFadePanel;
    [Tooltip("How long to fade to white before loading game")]
    public float fadeToWhiteDuration = 2f;

    private bool isStarting = false;

    void Awake()
    {
        if (Startbutton != null)
        {
            Startbutton.onClick.RemoveAllListeners();
            Startbutton.onClick.AddListener(StartGame);
        }

        // Setup white fade panel (start transparent)
        if (whiteFadePanel != null)
        {
            Color c = whiteFadePanel.color;
            c.a = 0f;
            whiteFadePanel.color = c;
            whiteFadePanel.gameObject.SetActive(true);
            whiteFadePanel.raycastTarget = false;
        }
    }

    public void StartGame()
    {
        if (isStarting) return;
        StartCoroutine(StartGameWithFade());
    }

    private IEnumerator StartGameWithFade()
    {
        isStarting = true;

        // Fade to white
        if (whiteFadePanel != null)
        {
            float elapsed = 0f;
            Color color = whiteFadePanel.color;

            while (elapsed < fadeToWhiteDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeToWhiteDuration;
                color.a = Mathf.Lerp(0f, 1f, t);
                whiteFadePanel.color = color;
                yield return null;
            }

            color.a = 1f;
            whiteFadePanel.color = color;
        }
        else
        {
            yield return new WaitForSeconds(fadeToWhiteDuration);
        }

        // Load game scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
}
