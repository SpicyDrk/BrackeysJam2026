using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    Light lightComponent;
    void Start()
    {
        lightComponent = GetComponent<Light>();
        if (lightComponent == null)
        {
            Debug.LogError("LightFlicker script requires a Light component on the same GameObject.");
            enabled = false;
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //flicker light
        lightComponent.intensity = 1f + Mathf.PerlinNoise(Time.time * 10f, 0f) * 0.5f;
        lightComponent.colorTemperature = 6500f + Mathf.PerlinNoise(Time.time * 10f, 1f) * 500f;
    }
}
