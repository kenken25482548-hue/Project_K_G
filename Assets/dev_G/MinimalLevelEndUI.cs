using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>Creates one consistent end-of-level screen for every mission at runtime.</summary>
public class MinimalLevelEndUI : MonoBehaviour
{
    private const string RootName = "MinimalLevelEnd";
    private static readonly Color Cyan = new Color(0.49f, 0.90f, 1f, 1f);
    private static readonly Color White = new Color(0.93f, 0.97f, 1f, 1f);
    private GameObject root;
    private TMP_FontAsset font;

    public bool IsVisible => root != null && root.activeSelf;

    public void ShowComplete(int cleared, int total, UnityAction next, UnityAction menu, UnityAction restart)
    {
        Build();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        root.SetActive(true);
        Transform window = root.transform.Find("Window");
        CreateText(window, "Eyebrow", "CLEAN & LEARN", 18f, Cyan, new Vector2(0f, 178f), new Vector2(480f, 32f), FontStyles.Bold);
        CreateText(window, "Title", "MISSION COMPLETE", 46f, White, new Vector2(0f, 95f), new Vector2(760f, 76f), FontStyles.Bold);
        CreateRule(window, new Vector2(0f, 42f), 340f);
        CreateText(window, "Summary", "ALL STAINS CLEARED  " + cleared + " / " + total, 21f, new Color(0.62f, 0.74f, 0.82f, 1f), new Vector2(0f, -8f), new Vector2(600f, 42f), FontStyles.Bold);
        CreateButton(window, "Next", "NEXT MISSION", new Vector2(-155f, -113f), next);
        CreateButton(window, "Restart", "REPLAY", new Vector2(0f, -113f), restart);
        CreateButton(window, "Menu", "MAIN MENU", new Vector2(155f, -113f), menu);
        CreateText(window, "NextHint", "GO TO THE NEXT LEVEL", 11f, new Color(0.84f, 0.95f, 1f, 0.92f), new Vector2(-155f, -154f), new Vector2(160f, 24f), FontStyles.Normal);
        CreateText(window, "RestartHint", "PLAY THIS LEVEL AGAIN", 11f, new Color(0.84f, 0.95f, 1f, 0.92f), new Vector2(0f, -154f), new Vector2(170f, 24f), FontStyles.Normal);
        CreateText(window, "MenuHint", "RETURN TO MAIN MENU", 11f, new Color(0.84f, 0.95f, 1f, 0.92f), new Vector2(155f, -154f), new Vector2(165f, 24f), FontStyles.Normal);
        CreateText(window, "ControlHint", "USE THE MOUSE TO SELECT", 14f, Cyan, new Vector2(0f, -210f), new Vector2(430f, 28f), FontStyles.Bold);
    }

    private void Build()
    {
        if (root != null) return;
        font = Resources.Load<TMP_FontAsset>("UI/Fonts/ChakraPetch-Bold SDF");
        if (font == null) font = TMP_Settings.defaultFontAsset;
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null) return;
        root = CreateObject(RootName, canvas.transform);
        Stretch(root.GetComponent<RectTransform>());
        Image dim = root.AddComponent<Image>();
        dim.color = new Color(0.02f, 0.16f, 0.24f, 0.66f);
        GameObject window = CreateObject("Window", root.transform);
        RectTransform r = window.GetComponent<RectTransform>();
        r.anchorMin = r.anchorMax = new Vector2(0.5f, 0.5f);
        r.pivot = new Vector2(0.5f, 0.5f);
        r.sizeDelta = new Vector2(900f, 500f);
        Image image = window.AddComponent<Image>();
        image.color = new Color(0.12f, 0.46f, 0.62f, 0.98f);
        Outline outline = window.AddComponent<Outline>();
        outline.effectColor = Cyan;
        outline.effectDistance = new Vector2(1.5f, -1.5f);
    }

    private void CreateButton(Transform parent, string name, string label, Vector2 position, UnityAction action)
    {
        GameObject obj = CreateObject(name, parent);
        RectTransform r = obj.GetComponent<RectTransform>();
        r.anchorMin = r.anchorMax = new Vector2(0.5f, 0.5f);
        r.pivot = new Vector2(0.5f, 0.5f);
        r.anchoredPosition = position;
        r.sizeDelta = new Vector2(145f, 52f);
        Image image = obj.AddComponent<Image>();
        image.color = new Color(0.04f, 0.25f, 0.37f, 1f);
        Button button = obj.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);
        ColorBlock colors = button.colors;
        colors.highlightedColor = new Color(0.44f, 0.82f, 1f, 1f);
        colors.pressedColor = new Color(0.22f, 0.55f, 0.78f, 1f);
        button.colors = colors;
        CreateText(obj.transform, "Label", label, 13f, White, Vector2.zero, new Vector2(140f, 42f), FontStyles.Bold);
    }

    private void CreateRule(Transform parent, Vector2 pos, float width)
    {
        GameObject obj = CreateObject("Rule", parent);
        RectTransform r = obj.GetComponent<RectTransform>();
        r.anchorMin = r.anchorMax = new Vector2(0.5f, 0.5f);
        r.anchoredPosition = pos;
        r.sizeDelta = new Vector2(width, 2f);
        Image image = obj.AddComponent<Image>();
        image.color = Cyan;
    }

    private void CreateText(Transform parent, string name, string value, float size, Color color, Vector2 position, Vector2 bounds, FontStyles style)
    {
        GameObject obj = CreateObject(name, parent);
        RectTransform r = obj.GetComponent<RectTransform>();
        r.anchorMin = r.anchorMax = new Vector2(0.5f, 0.5f);
        r.pivot = new Vector2(0.5f, 0.5f);
        r.anchoredPosition = position;
        r.sizeDelta = bounds;
        TextMeshProUGUI text = obj.AddComponent<TextMeshProUGUI>();
        text.font = font;
        text.text = value;
        text.fontSize = size;
        text.fontStyle = style;
        text.color = color;
        text.alignment = TextAlignmentOptions.Center;
        text.enableWordWrapping = false;
        text.raycastTarget = false;
    }

    private static GameObject CreateObject(string name, Transform parent)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform));
        obj.transform.SetParent(parent, false);
        return obj;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
