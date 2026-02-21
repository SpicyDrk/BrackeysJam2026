using UnityEngine;

/// <summary>
/// Manages all audio in the game - ambience, footsteps, and feedback sounds.
/// </summary>
public class SoundManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [Tooltip("Audio source for continuous ambience")]
    public AudioSource ambienceSource;
    [Tooltip("Audio source for footstep sounds")]
    public AudioSource footstepSource;
    [Tooltip("Audio source for UI/feedback sounds")]
    public AudioSource feedbackSource;

    [Header("Ambience")]
    public AudioClip ambienceClip;
    [Range(0f, 1f)]
    public float ambienceVolume = 0.3f;

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
}
