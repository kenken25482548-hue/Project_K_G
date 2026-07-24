using UnityEngine;

/// <summary>
/// Animates the standalone blue floor marker used to point out a stain.
/// The prefab has no gameplay or cleaning logic, so it can be placed freely.
/// </summary>
public sealed class StainMarkerBlueZone : MonoBehaviour
{
    [Header("Animated Parts")]
    [SerializeField] private Transform outerRing;
    [SerializeField] private Transform innerRing;
    [SerializeField] private SpriteRenderer outerRenderer;
    [SerializeField] private SpriteRenderer innerRenderer;
    [SerializeField] private Light glowLight;

    [Header("Motion")]
    [SerializeField] private float outerRotationSpeed = 7f;
    [SerializeField] private float innerRotationSpeed = -11f;
    [SerializeField, Min(0.05f)] private float pulseSpeed = 1.35f;
    [SerializeField, Range(0f, 0.2f)] private float outerScalePulse = 0.035f;
    [SerializeField, Range(0f, 0.2f)] private float innerScalePulse = 0.065f;

    [Header("Glow")]
    [SerializeField, Range(0f, 1f)] private float outerMinimumAlpha = 0.52f;
    [SerializeField, Range(0f, 1f)] private float innerMinimumAlpha = 0.16f;
    [SerializeField, Range(0f, 2f)] private float lightPulseAmount = 0.28f;

    private Vector3 outerBaseScale;
    private Vector3 innerBaseScale;
    private Color outerBaseColor;
    private Color innerBaseColor;
    private float baseLightIntensity;
    private float phase;

    private void Awake()
    {
        CacheStartingValues();
    }

    private void OnEnable()
    {
        CacheStartingValues();
    }

    private void CacheStartingValues()
    {
        if (outerRing != null)
        {
            outerBaseScale = outerRing.localScale;
        }

        if (innerRing != null)
        {
            innerBaseScale = innerRing.localScale;
        }

        if (outerRenderer != null)
        {
            outerBaseColor = outerRenderer.color;
        }

        if (innerRenderer != null)
        {
            innerBaseColor = innerRenderer.color;
        }

        if (glowLight != null)
        {
            baseLightIntensity = glowLight.intensity;
        }
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        phase += deltaTime * pulseSpeed * Mathf.PI * 2f;

        if (outerRing != null)
        {
            outerRing.Rotate(0f, 0f, outerRotationSpeed * deltaTime, Space.Self);
            float scale = 1f + Mathf.Sin(phase) * outerScalePulse;
            outerRing.localScale = outerBaseScale * scale;
        }

        if (innerRing != null)
        {
            innerRing.Rotate(0f, 0f, innerRotationSpeed * deltaTime, Space.Self);
            float scale = 1f + Mathf.Sin(phase + Mathf.PI) * innerScalePulse;
            innerRing.localScale = innerBaseScale * scale;
        }

        float pulse = Mathf.Sin(phase) * 0.5f + 0.5f;

        if (outerRenderer != null)
        {
            Color color = outerBaseColor;
            color.a = Mathf.Lerp(outerMinimumAlpha, outerBaseColor.a, pulse);
            outerRenderer.color = color;
        }

        if (innerRenderer != null)
        {
            Color color = innerBaseColor;
            color.a = Mathf.Lerp(innerMinimumAlpha, innerBaseColor.a, 1f - pulse);
            innerRenderer.color = color;
        }

        if (glowLight != null)
        {
            glowLight.intensity = baseLightIntensity + pulse * lightPulseAmount;
        }
    }
}
