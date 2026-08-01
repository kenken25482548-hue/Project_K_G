using UnityEngine;

/// <summary>
/// Animates the standalone marker used to highlight a collectible item.
/// Keep this prefab as a child of the item so it disappears with the item.
/// </summary>
public sealed class ItemMarkerGoldenOrbit : MonoBehaviour
{
    [Header("Animated Parts")]
    [SerializeField] private Transform outerOrbit;
    [SerializeField] private Transform innerOrbit;
    [SerializeField] private Transform floatingIndicator;
    [SerializeField] private SpriteRenderer outerRenderer;
    [SerializeField] private SpriteRenderer innerRenderer;
    [SerializeField] private SpriteRenderer floatingRenderer;
    [SerializeField] private Light glowLight;

    [Header("Motion")]
    [SerializeField] private float outerRotationSpeed = 16f;
    [SerializeField] private float innerRotationSpeed = -23f;
    [SerializeField, Min(0.05f)] private float pulseSpeed = 1.4f;
    [SerializeField, Min(0.05f)] private float hoverSpeed = 1.7f;
    [SerializeField, Range(0f, 0.25f)] private float hoverDistance = 0.07f;
    [SerializeField, Range(0f, 0.15f)] private float scalePulse = 0.035f;

    [Header("Glow")]
    [SerializeField, Range(0f, 1f)] private float outerMinimumAlpha = 0.24f;
    [SerializeField, Range(0f, 1f)] private float innerMinimumAlpha = 0.06f;
    [SerializeField, Range(0f, 1f)] private float floatingMinimumAlpha = 0.18f;
    [SerializeField, Range(0f, 1f)] private float lightPulseAmount = 0.035f;

    private Vector3 outerBaseScale;
    private Vector3 innerBaseScale;
    private Vector3 floatingBaseScale;
    private Vector3 floatingBasePosition;
    private Color outerBaseColor;
    private Color innerBaseColor;
    private Color floatingBaseColor;
    private float baseLightIntensity;
    private float phase;
    private Camera targetCamera;

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
        if (outerOrbit != null)
            outerBaseScale = outerOrbit.localScale;

        if (innerOrbit != null)
            innerBaseScale = innerOrbit.localScale;

        if (floatingIndicator != null)
        {
            floatingBaseScale = floatingIndicator.localScale;
            floatingBasePosition = floatingIndicator.localPosition;
        }

        if (outerRenderer != null)
            outerBaseColor = outerRenderer.color;

        if (innerRenderer != null)
            innerBaseColor = innerRenderer.color;

        if (floatingRenderer != null)
            floatingBaseColor = floatingRenderer.color;

        if (glowLight != null)
            baseLightIntensity = glowLight.intensity;

        targetCamera = Camera.main;
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        phase += deltaTime * pulseSpeed * Mathf.PI * 2f;

        if (outerOrbit != null)
        {
            outerOrbit.Rotate(0f, 0f, outerRotationSpeed * deltaTime, Space.Self);
            float scale = 1f + Mathf.Sin(phase) * scalePulse;
            outerOrbit.localScale = outerBaseScale * scale;
        }

        if (innerOrbit != null)
        {
            innerOrbit.Rotate(0f, 0f, innerRotationSpeed * deltaTime, Space.Self);
            float scale = 1f + Mathf.Sin(phase + Mathf.PI) * scalePulse;
            innerOrbit.localScale = innerBaseScale * scale;
        }

        if (floatingIndicator != null)
        {
            float bob = Mathf.Sin(Time.time * hoverSpeed * Mathf.PI * 2f) * hoverDistance;
            floatingIndicator.localPosition = floatingBasePosition + Vector3.up * bob;

            float scale = 1f + Mathf.Sin(phase + Mathf.PI * 0.5f) * scalePulse;
            floatingIndicator.localScale = floatingBaseScale * scale;

            if (targetCamera == null)
                targetCamera = Camera.main;

            if (targetCamera != null)
                floatingIndicator.forward = targetCamera.transform.forward;
        }

        float pulse = Mathf.Sin(phase) * 0.5f + 0.5f;

        SetRendererAlpha(outerRenderer, outerBaseColor, Mathf.Lerp(outerMinimumAlpha, outerBaseColor.a, pulse));
        SetRendererAlpha(innerRenderer, innerBaseColor, Mathf.Lerp(innerMinimumAlpha, innerBaseColor.a, 1f - pulse));
        SetRendererAlpha(
            floatingRenderer,
            floatingBaseColor,
            Mathf.Lerp(floatingMinimumAlpha, floatingBaseColor.a, pulse));

        if (glowLight != null)
            glowLight.intensity = baseLightIntensity + pulse * lightPulseAmount;
    }

    private static void SetRendererAlpha(SpriteRenderer renderer, Color baseColor, float alpha)
    {
        if (renderer == null)
            return;

        Color color = baseColor;
        color.a = alpha;
        renderer.color = color;
    }
}
