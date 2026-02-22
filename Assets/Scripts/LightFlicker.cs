using UnityEngine;

/// <summary>
/// Makes a light flicker with distance-based noise intensity.
/// Can trigger atmospheric sounds at certain distances.
/// </summary>
public class LightFlicker : MonoBehaviour
{
    [Header("Flicker Settings")]
    [Tooltip("Base intensity of the light")]
    public float baseIntensity = 1f;
    [Tooltip("Maximum additional intensity variation")]
    public float flickerAmount = 0.5f;
    [Tooltip("Speed of the flickering")]
    public float flickerSpeed = 10f;
    [Tooltip("Base color temperature")]
    public float baseColorTemp = 6500f;
    [Tooltip("Color temperature variation")]
    public float colorTempVariation = 500f;

    [Header("Distance-Based Noise")]
    [Tooltip("Reference to the player transform")]
    public Transform player;
    [Tooltip("Distance at which flickering starts")]
    public float flickerStartDistance = 20f;
    [Tooltip("Distance at which flickering is at maximum intensity")]
    public float flickerMaxDistance = 5f;
    [Tooltip("Enable distance-based flickering")]
    public bool useDistanceFlicker = false;

    [Header("Sound Integration")]
    [Tooltip("Audio clip for flickering buzz/hum noise")]
    public AudioClip flickerSound;
    [Tooltip("Audio source for local flicker sounds (optional)")]
    public AudioSource localAudioSource;
    [Range(0f, 1f)]
    public float flickerSoundVolume = 0.3f;
    [Tooltip("How often to play the flicker sound (in seconds)")]
    public float soundInterval = 5f;

    private Light lightComponent;
    private float noiseOffset;
    private float soundTimer;

    private void Start()
    {
        lightComponent = GetComponent<Light>();
        if (lightComponent == null)
        {
            Debug.LogError("LightFlicker script requires a Light component on the same GameObject.");
            enabled = false;
            return;
        }

        noiseOffset = Random.Range(0f, 100f); // Random offset for varied flicker patterns
        soundTimer = Random.Range(0f, soundInterval);

        // Auto-find player if not assigned and using distance flicker
        if (useDistanceFlicker && player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        // Create local audio source if using sounds but none assigned
        if (flickerSound != null && localAudioSource == null)
        {
            localAudioSource = gameObject.AddComponent<AudioSource>();
            localAudioSource.spatialBlend = 1f; // 3D sound
            localAudioSource.maxDistance = flickerStartDistance;
            localAudioSource.rolloffMode = AudioRolloffMode.Linear;
        }
    }

    private void Update()
    {
        float distanceFactor = 1f;

        // Calculate distance-based intensity if enabled
        if (useDistanceFlicker && player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            // Map distance to flicker intensity (0 = far, 1 = close)
            if (distance >= flickerStartDistance)
            {
                distanceFactor = 0f; // No flicker when far
            }
            else if (distance <= flickerMaxDistance)
            {
                distanceFactor = 1f; // Max flicker when close
            }
            else
            {
                // Interpolate between max and start distance
                distanceFactor = 1f - ((distance - flickerMaxDistance) / (flickerStartDistance - flickerMaxDistance));
            }
        }

        // Apply flickering using Perlin noise
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, noiseOffset);
        float flickerValue = (noise - 0.5f) * 2f; // Convert 0-1 to -1 to 1
        
        // Apply distance factor to flicker amount
        float actualFlicker = flickerValue * flickerAmount * distanceFactor;
        lightComponent.intensity = baseIntensity + actualFlicker;

        // Color temperature flicker
        float colorNoise = Mathf.PerlinNoise(Time.time * flickerSpeed, noiseOffset + 1f);
        lightComponent.colorTemperature = baseColorTemp + (colorNoise - 0.5f) * 2f * colorTempVariation;

        // Handle flickering sounds
        if (flickerSound != null && localAudioSource != null && distanceFactor > 0.1f)
        {
            soundTimer -= Time.deltaTime;
            if (soundTimer <= 0f)
            {
                localAudioSource.PlayOneShot(flickerSound, flickerSoundVolume * distanceFactor);
                soundTimer = soundInterval + Random.Range(-1f, 2f); // Add variation
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!useDistanceFlicker) return;

        // Draw distance ranges
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, flickerMaxDistance);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, flickerStartDistance);
    }
}
