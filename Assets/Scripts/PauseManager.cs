using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

/// <summary>
/// Manages pause menu and scene restarting with fade effects.
/// </summary>
public class PauseManager : MonoBehaviour
{
    [Header("Pause Menu UI")]
    [Tooltip("Root panel for pause menu")]
    public GameObject pauseMenuPanel;
    [Tooltip("Resume button")]
    public Button resumeButton;
    [Tooltip("Restart button")]
    public Button restartButton;
    [Tooltip("Quit button (optional)")]
    public Button quitButton;

    [Header("Fade Panel")]
    [Tooltip("White panel for fade to white on restart")]
    public Image whiteFadePanel;
    [Tooltip("How long to fade to white")]
    public float fadeToWhiteDuration = 1f;

    [Header("Pause Settings")]
    [Tooltip("Enable pause with Escape key")]
    public bool enablePause = true;

    private bool isPaused = false;
    private bool isRestarting = false;
    private static PauseManager instance;
    public static PauseManager Instance => instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        // Setup buttons
        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.AddListener(Resume);
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartLevel);
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(QuitGame);
        }

        // Make sure pause menu starts hidden
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        // Setup white fade panel
        if (whiteFadePanel != null)
        {
            Color c = whiteFadePanel.color;
            c.a = 0f;
            whiteFadePanel.color = c;
            whiteFadePanel.gameObject.SetActive(true);
            whiteFadePanel.raycastTarget = false;
        }
    }

    private void Update()
    {
        // Don't allow pausing during restart
        if (isRestarting || !enablePause) return;

        // Toggle pause with ESC key using new Input System
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RestartLevel()
    {
        if (isRestarting) return;
        StartCoroutine(RestartWithFade());
    }

    private IEnumerator RestartWithFade()
    {
        isRestarting = true;
        Time.timeScale = 1f;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        // Fade to white
        if (whiteFadePanel != null)
        {
            yield return StartCoroutine(FadeToWhite(fadeToWhiteDuration));
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        // Reload current scene
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }

    private IEnumerator FadeToWhite(float duration)
    {
        if (whiteFadePanel == null) yield break;

        float elapsed = 0f;
        Color color = whiteFadePanel.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            color.a = Mathf.Lerp(0f, 1f, t);
            whiteFadePanel.color = color;
            yield return null;
        }

        color.a = 1f;
        whiteFadePanel.color = color;
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public bool IsPaused() => isPaused;
}
