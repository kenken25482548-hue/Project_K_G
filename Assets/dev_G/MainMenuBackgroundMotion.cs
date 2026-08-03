using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuBackgroundMotion : MonoBehaviour
{
    private sealed class LightParticle
    {
        public RectTransform rect;
        public Image image;
        public float baseX;
        public float startY;
        public float speed;
        public float drift;
        public float phase;
        public float alpha;
    }

    private readonly List<LightParticle> particles = new List<LightParticle>();

    private RawImage background;
    private RawImage lightSweep;
    private Texture2D sweepTexture;
    private Texture2D particleTexture;
    private Sprite particleSprite;
    private bool initialized;

    public void Configure(RawImage backgroundImage, RawImage sweepImage)
    {
        background = backgroundImage;
        lightSweep = sweepImage;

        if (background != null && background.texture != null)
            background.texture.wrapMode = TextureWrapMode.Clamp;

        BuildSweepTexture();
        BuildParticleSprite();
        BuildParticles();
        initialized = true;
    }

    void Update()
    {
        if (!initialized)
            return;

        float time = Time.unscaledTime;

        AnimateBackground(time);
        AnimateLightSweep(time);
        AnimateParticles(time);
    }

    void AnimateBackground(float time)
    {
        if (background == null)
            return;

        float viewSize = 0.955f + Mathf.Sin(time * 0.22f) * 0.004f;
        float available = 1f - viewSize;
        float x = available * 0.5f + Mathf.Sin(time * 0.13f) * 0.008f;
        float y = available * 0.5f + Mathf.Cos(time * 0.17f) * 0.006f;
        background.uvRect = new Rect(x, y, viewSize, viewSize);

        // A blank RawImage is used by the minimal menu. Preserve its navy tint;
        // only apply the bright photo pulse when a real background texture exists.
        if (background.texture != null)
        {
            float lightPulse = 0.965f + Mathf.Sin(time * 0.45f) * 0.035f;
            background.color = new Color(
                0.93f * lightPulse,
                0.97f * lightPulse,
                lightPulse,
                1f);
        }
    }

    void AnimateLightSweep(float time)
    {
        if (lightSweep == null)
            return;

        RectTransform sweepRect = lightSweep.rectTransform;
        float normalizedX = -0.22f + Mathf.Repeat(time * 0.055f, 1.48f);
        sweepRect.anchorMin = new Vector2(normalizedX, 0.5f);
        sweepRect.anchorMax = new Vector2(normalizedX, 0.5f);
        sweepRect.anchoredPosition = new Vector2(0f, Mathf.Sin(time * 0.31f) * 42f);
        sweepRect.localRotation = Quaternion.Euler(
            0f,
            0f,
            -12f + Mathf.Sin(time * 0.24f) * 1.5f);

        Color color = lightSweep.color;
        color.a = 0.055f + Mathf.Sin(time * 0.68f) * 0.018f;
        lightSweep.color = color;
    }

    void AnimateParticles(float time)
    {
        for (int i = 0; i < particles.Count; i++)
        {
            LightParticle particle = particles[i];
            float y = -0.08f + Mathf.Repeat(
                particle.startY + time * particle.speed,
                1.16f);
            float x = particle.baseX +
                      Mathf.Sin(time * 0.38f + particle.phase) * particle.drift;

            Vector2 anchor = new Vector2(x, y);
            particle.rect.anchorMin = anchor;
            particle.rect.anchorMax = anchor;
            particle.rect.anchoredPosition = Vector2.zero;

            float pulse = 0.45f +
                          (Mathf.Sin(time * 0.9f + particle.phase) + 1f) * 0.275f;
            Color color = particle.image.color;
            color.a = particle.alpha * pulse;
            particle.image.color = color;
        }
    }

    void BuildSweepTexture()
    {
        sweepTexture = new Texture2D(256, 1, TextureFormat.RGBA32, false)
        {
            name = "Main Menu Light Sweep",
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };

        for (int x = 0; x < sweepTexture.width; x++)
        {
            float normalized = x / (sweepTexture.width - 1f);
            float alpha = Mathf.Pow(Mathf.Sin(normalized * Mathf.PI), 3.5f);
            sweepTexture.SetPixel(x, 0, new Color(1f, 1f, 1f, alpha));
        }

        sweepTexture.Apply(false, true);

        if (lightSweep != null)
            lightSweep.texture = sweepTexture;
    }

    void BuildParticleSprite()
    {
        const int size = 32;
        particleTexture = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            name = "Main Menu Light Mote",
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };

        Vector2 center = new Vector2((size - 1f) * 0.5f, (size - 1f) * 0.5f);
        float radius = size * 0.5f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center) / radius;
                float alpha = Mathf.Pow(Mathf.Clamp01(1f - distance), 2.2f);
                particleTexture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        particleTexture.Apply(false, true);
        particleSprite = Sprite.Create(
            particleTexture,
            new Rect(0f, 0f, size, size),
            new Vector2(0.5f, 0.5f),
            size);
        particleSprite.name = "Main Menu Light Mote Sprite";
    }

    void BuildParticles()
    {
        System.Random random = new System.Random(2548);

        for (int i = 0; i < 24; i++)
        {
            GameObject particleObject = new GameObject(
                $"LightMote_{i + 1:00}",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image));
            particleObject.layer = LayerMask.NameToLayer("UI");
            particleObject.transform.SetParent(transform, false);

            RectTransform rect = particleObject.GetComponent<RectTransform>();
            float moteSize = Mathf.Lerp(3f, 13f, (float)random.NextDouble());
            rect.sizeDelta = new Vector2(moteSize, moteSize);
            rect.localScale = Vector3.one;

            Image image = particleObject.GetComponent<Image>();
            image.sprite = particleSprite;
            image.raycastTarget = false;

            bool warm = random.NextDouble() > 0.82;
            image.color = warm
                ? new Color(1f, 0.72f, 0.38f, 0.12f)
                : new Color(0.30f, 0.80f, 1f, 0.14f);

            LightParticle particle = new LightParticle
            {
                rect = rect,
                image = image,
                baseX = Mathf.Lerp(0.43f, 1.02f, (float)random.NextDouble()),
                startY = Mathf.Lerp(0f, 1.15f, (float)random.NextDouble()),
                speed = Mathf.Lerp(0.007f, 0.023f, (float)random.NextDouble()),
                drift = Mathf.Lerp(0.003f, 0.018f, (float)random.NextDouble()),
                phase = Mathf.Lerp(0f, Mathf.PI * 2f, (float)random.NextDouble()),
                alpha = Mathf.Lerp(0.08f, 0.24f, (float)random.NextDouble())
            };

            particles.Add(particle);
        }
    }

    void OnDestroy()
    {
        ReleaseGeneratedObject(particleSprite);
        ReleaseGeneratedObject(particleTexture);
        ReleaseGeneratedObject(sweepTexture);
    }

    static void ReleaseGeneratedObject(UnityEngine.Object generatedObject)
    {
        if (generatedObject == null)
            return;

        if (Application.isPlaying)
            Destroy(generatedObject);
        else
            DestroyImmediate(generatedObject);
    }
}
