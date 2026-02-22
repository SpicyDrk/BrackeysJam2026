using UnityEngine;

/// <summary>
/// Manages all audio in the game - ambience, footsteps, and feedback sounds.
/// </summary>
public class SoundManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [Tooltip("Audio source for continuous ambience")]
    public AudioSource ambienceSource;
    [Tooltip("Audio source for atmospheric sounds (scary/muffled voices)")]
    public AudioSource atmosphericSource;
    [Tooltip("Audio source for footstep sounds")]
    public AudioSource footstepSource;
    [Tooltip("Audio source for UI/feedback sounds")]
    public AudioSource feedbackSource;

    [Header("Ambience")]
    public AudioClip ambienceClip;
    [Range(0f, 1f)]
    public float ambienceVolume = 0.3f;

    [Header("Atmospheric Sounds (Scary/Ambient)")]
    [Tooltip("Atmospheric sounds like muffled voices, distant noises")]
    public AudioClip[] atmosphericClips;
    [Range(0f, 1f)]
    public float atmosphericVolume = 0.2f;
    [Tooltip("How often atmospheric sounds play (in seconds)")]
    public float atmosphericInterval = 10f;

    [Header("Footsteps")]
    public AudioClip footstepClip;
    [Range(0f, 1f)]
    public float footstepVolume = 0.5f;
    [Tooltip("Random pitch variation range")]
    public Vector2 pitchRange = new Vector2(0.9f, 1.1f);
    [Tooltip("Time between footsteps when walking")]
    public float walkStepInterval = 0.5f;
    [Tooltip("Time between footsteps when sprinting")]
    public float sprintStepInterval = 0.35f;

    [Header("Feedback Sounds")]
    public AudioClip correctSound;
    public AudioClip incorrectSound;
    [Range(0f, 1f)]
    public float feedbackVolume = 0.7f;

    private float stepTimer = 0f;
    private bool isMoving = false;
    private bool isSprinting = false;
    private float atmosphericTimer = 0f;
    private bool atmosphericEnabled = true;
    private Coroutine fadeCoroutine;

    private static SoundManager instance;
    public static SoundManager Instance => instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        // Auto-create audio sources if not assigned
        if (ambienceSource == null)
        {
            GameObject ambienceObj = new GameObject("AmbienceSource");
            ambienceObj.transform.SetParent(transform);
            ambienceSource = ambienceObj.AddComponent<AudioSource>();
        }

        if (footstepSource == null)
        {
            GameObject footstepObj = new GameObject("FootstepSource");
            footstepObj.transform.SetParent(transform);
            footstepSource = footstepObj.AddComponent<AudioSource>();
        }

        if (feedbackSource == null)
        {
            GameObject feedbackObj = new GameObject("FeedbackSource");
            feedbackObj.transform.SetParent(transform);
            feedbackSource = feedbackObj.AddComponent<AudioSource>();
        }

        if (atmosphericSource == null)
        {
            GameObject atmosphericObj = new GameObject("AtmosphericSource");
            atmosphericObj.transform.SetParent(transform);
            atmosphericSource = atmosphericObj.AddComponent<AudioSource>();
            atmosphericSource.spatialBlend = 0f; // 2D sound
        }
    }

    private void Start()
    {
        PlayAmbience();
    }

    private void Update()
    {
        // Handle footstep timing
        if (isMoving)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = isSprinting ? sprintStepInterval : walkStepInterval;
            }
        }
        else
        {
            stepTimer = 0f;
        }

        // Handle atmospheric sounds
        if (atmosphericEnabled && atmosphericClips != null && atmosphericClips.Length > 0)
        {
            atmosphericTimer -= Time.deltaTime;
            if (atmosphericTimer <= 0f)
            {
                PlayRandomAtmospheric();
                atmosphericTimer = atmosphericInterval + Random.Range(-2f, 3f); // Add variation
            }
        }
    }

    public void PlayAmbience()
    {
        if (ambienceClip != null && ambienceSource != null)
        {
            ambienceSource.clip = ambienceClip;
            ambienceSource.volume = ambienceVolume;
            ambienceSource.loop = true;
            ambienceSource.Play();
        }
    }

    public void StopAmbience()
    {
        if (ambienceSource != null)
        {
            ambienceSource.Stop();
        }
    }

    public void SetMovementState(bool moving, bool sprinting)
    {
        isMoving = moving;
        isSprinting = sprinting;
    }

    private void PlayFootstep()
    {
        if (footstepClip != null && footstepSource != null)
        {
            // Randomize pitch for variation
            footstepSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
            footstepSource.PlayOneShot(footstepClip, footstepVolume);
        }
    }

    public void PlayCorrectSound()
    {
        if (correctSound != null && feedbackSource != null)
        {
            feedbackSource.PlayOneShot(correctSound, feedbackVolume);
        }
    }

    public void PlayIncorrectSound()
    {
        if (incorrectSound != null && feedbackSource != null)
        {
            feedbackSource.PlayOneShot(incorrectSound, feedbackVolume);
        }
    }

    public void PlayOneShot(AudioClip clip, float volume = 1f)
    {
        if (clip != null && feedbackSource != null)
        {
            feedbackSource.PlayOneShot(clip, volume);
        }
    }

    private void PlayRandomAtmospheric()
    {
        if (atmosphericClips != null && atmosphericClips.Length > 0 && atmosphericSource != null)
        {
            AudioClip clip = atmosphericClips[Random.Range(0, atmosphericClips.Length)];
            if (clip != null)
            {
                atmosphericSource.PlayOneShot(clip, atmosphericVolume);
            }
        }
    }

    /// <summary>
    /// Fades out ambience and atmospheric sounds over time. Keeps footsteps active.
    /// Called when player reaches win condition.
    /// </summary>
    public void FadeOutAmbientSounds(float fadeTime = 2f)
    {

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeOutCoroutine(fadeTime));
    }

    private System.Collections.IEnumerator FadeOutCoroutine(float fadeTime)
    {
        float elapsed = 0f;
        float startAmbienceVolume = ambienceSource != null ? ambienceSource.volume : 0f;
        float startAtmosphericVolume = atmosphericSource != null ? atmosphericSource.volume : 0f;

        // Disable new atmospheric sounds from triggering
        atmosphericEnabled = false;

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeTime;

            if (ambienceSource != null)
            {
                ambienceSource.volume = Mathf.Lerp(startAmbienceVolume, 0f, t);
            }

            if (atmosphericSource != null)
            {
                atmosphericSource.volume = Mathf.Lerp(startAtmosphericVolume, 0f, t);
            }

            yield return null;
        }

        // Ensure fully stopped
        if (ambienceSource != null)
        {
            ambienceSource.volume = 0f;
            ambienceSource.Stop();
        }

        if (atmosphericSource != null)
        {
            atmosphericSource.volume = 0f;
            atmosphericSource.Stop();
        }
    }

    /// <summary>
    /// Emergency method to find and stop ALL audio sources in the scene except footsteps.
    /// Use this if ambience is still playing from an unknown source.
    /// </summary>
    public void StopAllAmbientAudioSources()
    {
        AudioSource[] allSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        Debug.Log($"[SoundManager] Found {allSources.Length} audio sources in scene");

        foreach (AudioSource source in allSources)
        {
            // Don't stop footsteps or feedback sources
            if (source == footstepSource || source == feedbackSource)
            {
                Debug.Log($"[SoundManager] Keeping: {source.gameObject.name} (footsteps/feedback)");
                continue;
            }

            // Stop everything else and log it
            if (source.isPlaying)
            {
                Debug.Log($"[SoundManager] Stopping: {source.gameObject.name} - clip: {(source.clip != null ? source.clip.name : "null")}, volume: {source.volume}");
                source.Stop();
                source.volume = 0f;
            }
        }
    }
}
