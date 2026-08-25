using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Runtime case-file screen shown before each mission begins.</summary>
public class MissionStoryUI : MonoBehaviour
{
    public static bool IsShowing { get; private set; }

    private MissionStoryData mission;
    private TMP_FontAsset thaiFont;
    private TMP_FontAsset titleFont;
    private GameObject root;
    private GameObject overlayCanvasObject;

    public void Show(MissionStoryData data)
    {
        mission = data;
        StartCoroutine(ShowWhenSceneUiIsReady());
    }

    private IEnumerator ShowWhenSceneUiIsReady()
    {
        yield return null;
        Build();
        IsShowing = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Build()
    {
        MissionLevelData level = MissionLevelCatalog.Get(mission.sceneName);
        overlayCanvasObject = new GameObject("MissionStoryOverlay", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        overlayCanvasObject.transform.SetParent(transform, false);
        Canvas canvas = overlayCanvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 3000;
        CanvasScaler scaler = overlayCanvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        // The case-file briefing intentionally keeps the game's distinctive display font.
        // All other Thai UI uses Kanit for maximum readability.
        thaiFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/MiPancake SDF");
        titleFont = thaiFont;
        if (thaiFont == null) thaiFont = TMP_Settings.defaultFontAsset;
        if (titleFont == null) titleFont = thaiFont;

        root = CreateObject("MissionStoryScreen", canvas.transform);
        Stretch(root.GetComponent<RectTransform>());
        Image dim = root.AddComponent<Image>();
        dim.color = new Color(0.005f, 0.025f, 0.055f, 0.95f);

        GameObject card = CreateObject("CaseFile", root.transform);
        RectTransform cardRect = card.GetComponent<RectTransform>();
        cardRect.anchorMin = cardRect.anchorMax = new Vector2(0.5f, 0.5f);
        cardRect.pivot = new Vector2(0.5f, 0.5f);
        cardRect.sizeDelta = new Vector2(940f, 590f);
        Image cardImage = card.AddComponent<Image>();
        cardImage.color = new Color(0.035f, 0.19f, 0.29f, 0.99f);
        Outline outline = card.AddComponent<Outline>();
        outline.effectColor = new Color(0.49f, 0.90f, 1f, 1f);
        outline.effectDistance = new Vector2(1.5f, -1.5f);

        CreateRule(card.transform, new Vector2(0f, 252f), 820f, new Color(0.49f, 0.90f, 1f, 1f));
        CreateText(card.transform, "File", "CASE FILE  /  LEVEL " + level.number.ToString("00") + "  /  " + level.difficulty, 19f,
            new Color(0.49f, 0.90f, 1f, 1f), new Vector2(0f, 215f), new Vector2(720f, 28f), titleFont, TextAlignmentOptions.Center, 2f);
        CreateText(card.transform, "Chapter", mission.chapter, 42f, Color.white,
            new Vector2(0f, 152f), new Vector2(820f, 60f), titleFont, TextAlignmentOptions.Center, 1f);
        CreateText(card.transform, "Room", "MISSION " + mission.missionNumber + "  —  " + mission.englishRoom + "  /  " + mission.thaiRoom, 21f,
            new Color(0.75f, 0.91f, 0.98f, 1f), new Vector2(0f, 103f), new Vector2(800f, 30f), thaiFont, TextAlignmentOptions.Center);
        CreateRule(card.transform, new Vector2(0f, 72f), 760f, new Color(0.52f, 0.77f, 0.88f, 0.38f));
        CreateText(card.transform, "BriefingLabel", "INCIDENT REPORT", 16f,
            new Color(0.49f, 0.90f, 1f, 1f), new Vector2(-285f, 43f), new Vector2(220f, 24f), titleFont, TextAlignmentOptions.Left, 1f);
        CreateText(card.transform, "Challenge", level.challenge + "  /  " + level.stainTarget + " STAINS  /  " + level.maxWrongUses + " ERRORS", 13f,
            new Color(0.66f, 0.86f, 0.95f, 1f), new Vector2(240f, 43f), new Vector2(360f, 22f), titleFont, TextAlignmentOptions.Right, 0f);
        CreateText(card.transform, "Briefing", mission.briefing, 20f, Color.white,
            new Vector2(0f, -30f), new Vector2(740f, 105f), thaiFont, TextAlignmentOptions.Center);

        GameObject objective = CreateObject("Objective", card.transform);
        RectTransform objectiveRect = objective.GetComponent<RectTransform>();
        objectiveRect.anchorMin = objectiveRect.anchorMax = new Vector2(0.5f, 0.5f);
        objectiveRect.anchoredPosition = new Vector2(0f, -165f);
        objectiveRect.sizeDelta = new Vector2(740f, 78f);
        Image objectiveImage = objective.AddComponent<Image>();
        objectiveImage.color = new Color(0.02f, 0.10f, 0.17f, 0.88f);
        CreateText(objective.transform, "ObjectiveLabel", "MISSION OBJECTIVE", 18f,
            new Color(0.49f, 0.90f, 1f, 1f), new Vector2(0f, 18f), new Vector2(500f, 22f), titleFont, TextAlignmentOptions.Center, 1f);
        CreateText(objective.transform, "ObjectiveText", mission.objective, 23f, Color.white,
            new Vector2(0f, -15f), new Vector2(670f, 32f), thaiFont, TextAlignmentOptions.Center);

        CreateButton(card.transform, "Begin", "BEGIN MISSION", new Vector2(0f, -250f), BeginMission);
    }

    private void BeginMission()
    {
        IsShowing = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (overlayCanvasObject != null) Destroy(overlayCanvasObject);
        Destroy(gameObject);
    }

    private void Update()
    {
        if (!IsShowing) return;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void CreateButton(Transform parent, string name, string label, Vector2 position, UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObject = CreateObject(name, parent);
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(280f, 55f);
        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.04f, 0.35f, 0.49f, 1f);
        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);
        ColorBlock colors = button.colors;
        colors.highlightedColor = new Color(0.55f, 0.92f, 1f, 1f);
        colors.pressedColor = new Color(0.25f, 0.62f, 0.78f, 1f);
        button.colors = colors;
        CreateText(buttonObject.transform, "Label", label, 22f, Color.white,
            Vector2.zero, new Vector2(250f, 40f), titleFont, TextAlignmentOptions.Center, 2f);
    }

    private void CreateRule(Transform parent, Vector2 position, float width, Color color)
    {
        GameObject rule = CreateObject("Rule", parent);
        RectTransform rect = rule.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(width, 2f);
        Image image = rule.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
    }

    private void CreateText(Transform parent, string name, string value, float size, Color color, Vector2 position, Vector2 bounds, TMP_FontAsset font, TextAlignmentOptions alignment, float spacing = 0f)
    {
        GameObject obj = CreateObject(name, parent);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = bounds;
        TextMeshProUGUI text = obj.AddComponent<TextMeshProUGUI>();
        text.font = font;
        text.text = value;
        text.fontSize = size;
        text.color = color;
        text.fontStyle = ContainsThai(value) ? FontStyles.Normal : FontStyles.Bold;
        text.alignment = alignment;
        text.characterSpacing = 0f;
        text.lineSpacing = value.IndexOf('\n') >= 0 ? 12f : 0f;
        text.extraPadding = false;
        text.enableWordWrapping = true;
        text.raycastTarget = false;
    }

    private static GameObject CreateObject(string name, Transform parent)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform));
        obj.transform.SetParent(parent, false);
        return obj;
    }

    private static bool ContainsThai(string value)
    {
        foreach (char c in value)
            if (c >= '\u0E00' && c <= '\u0E7F') return true;
        return false;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
