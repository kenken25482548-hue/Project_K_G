using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Displays a small mission identifier at the start of every gameplay scene.</summary>
public class LevelMissionBanner : MonoBehaviour
{
    private CanvasGroup group;
    private float elapsed;
    private const float HoldDuration = 2.6f;
    private const float FadeDuration = 0.65f;

    public void Show(string missionNumber, string englishName, string thaiName)
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null) return;

        GameObject root = CreateObject("MissionBanner", canvas.transform);
        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = rootRect.anchorMax = new Vector2(0.5f, 1f);
        rootRect.pivot = new Vector2(0.5f, 1f);
        rootRect.anchoredPosition = new Vector2(0f, -46f);
        rootRect.sizeDelta = new Vector2(520f, 118f);

        Image panel = root.AddComponent<Image>();
        panel.color = new Color(0.025f, 0.17f, 0.27f, 0.94f);
        Outline outline = root.AddComponent<Outline>();
        outline.effectColor = new Color(0.49f, 0.90f, 1f, 1f);
        outline.effectDistance = new Vector2(1f, -1f);
        group = root.AddComponent<CanvasGroup>();

        TMP_FontAsset thaiFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/MiPancake SDF");
        TMP_FontAsset titleFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/MiPancake SDF");
        CreateText(root.transform, "Mission", "MISSION " + missionNumber, 16f,
            new Color(0.49f, 0.90f, 1f, 1f), new Vector2(0f, 30f), new Vector2(420f, 28f), titleFont);
        CreateText(root.transform, "Name", englishName, 28f,
            new Color(0.94f, 0.99f, 1f, 1f), new Vector2(0f, -2f), new Vector2(460f, 38f), titleFont);
        CreateText(root.transform, "ThaiName", thaiName, 19f,
            new Color(0.76f, 0.90f, 0.96f, 1f), new Vector2(0f, -39f), new Vector2(460f, 30f), thaiFont);
    }

    void Update()
    {
        if (group == null) return;
        elapsed += Time.unscaledDeltaTime;
        if (elapsed > HoldDuration)
            group.alpha = 1f - Mathf.Clamp01((elapsed - HoldDuration) / FadeDuration);
        if (elapsed > HoldDuration + FadeDuration)
            Destroy(gameObject);
    }

    private static void CreateText(Transform parent, string name, string value, float size, Color color, Vector2 position, Vector2 bounds, TMP_FontAsset font)
    {
        GameObject obj = CreateObject(name, parent);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = bounds;
        TextMeshProUGUI text = obj.AddComponent<TextMeshProUGUI>();
        text.font = font != null ? font : TMP_Settings.defaultFontAsset;
        text.text = value;
        text.fontSize = size;
        text.fontStyle = FontStyles.Bold;
        text.color = color;
        text.alignment = TextAlignmentOptions.Center;
        text.extraPadding = false;
        text.raycastTarget = false;
    }

    private static GameObject CreateObject(string name, Transform parent)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform));
        obj.transform.SetParent(parent, false);
        return obj;
    }
}
