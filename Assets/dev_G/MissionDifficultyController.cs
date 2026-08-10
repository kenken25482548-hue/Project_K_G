using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Converts the existing room scenes into a four-step difficulty curve without
/// requiring per-scene wiring. Higher levels activate more stains, leave more
/// decoy items available, and allow fewer incorrect item uses.
/// </summary>
public class MissionDifficultyController : MonoBehaviour
{
    public MissionLevelData CurrentLevel { get; private set; }
    public int WrongUses { get; private set; }
    public int CurrentCombo { get; private set; }
    public int BestCombo { get; private set; }

    private LevelFlowManager levelFlow;
    private bool configured;
    private GameObject hudRoot;
    private Image progressFill;
    private TMP_Text progressText;
    private TMP_Text mistakesText;
    private TMP_Text comboText;

    public void Configure(LevelFlowManager flow)
    {
        if (configured) return;
        configured = true;
        levelFlow = flow;
        MissionLevelData level = MissionLevelCatalog.Get(SceneManager.GetActiveScene().name);
        if (GameProgress.IsChallengeMode)
        {
            level.difficulty = "CHALLENGE";
            level.challenge = "NO ROOM FOR ERROR";
            level.stainTarget += 1;
            level.decoyItemCount = 99;
            level.maxWrongUses = 1;
        }
        CurrentLevel = level;

        ConfigureStains();
        ConfigureItems();
        CleaningTarget.WrongItemUsed += HandleWrongItemUsed;
        CleaningTarget.CorrectItemUsed += HandleCorrectItemUsed;
        BuildHud();
    }

    private void OnDestroy()
    {
        CleaningTarget.WrongItemUsed -= HandleWrongItemUsed;
        CleaningTarget.CorrectItemUsed -= HandleCorrectItemUsed;
        if (hudRoot != null) Destroy(hudRoot);
    }

    private void Update()
    {
        if (!configured || progressText == null) return;

        CleaningTarget[] stains = FindObjectsOfType<CleaningTarget>();
        int cleared = 0;
        foreach (CleaningTarget stain in stains)
            if (stain.isCleared) cleared++;

        progressText.text = "STAIN FILES  " + cleared + " / " + stains.Length;
        if (progressFill != null)
        {
            progressFill.rectTransform.anchorMax = new Vector2(stains.Length == 0 ? 0f : (float)cleared / stains.Length, 1f);
            progressFill.rectTransform.offsetMax = Vector2.zero;
        }

        int remaining = Mathf.Max(0, CurrentLevel.maxWrongUses - WrongUses);
        mistakesText.text = "ERRORS LEFT  " + remaining + " / " + CurrentLevel.maxWrongUses;
        mistakesText.color = remaining <= 1 ? new Color(1f, 0.55f, 0.34f, 1f) : new Color(0.60f, 0.87f, 0.98f, 1f);
        comboText.text = "CLEAN COMBO  x" + CurrentCombo + "    BEST x" + BestCombo;
    }

    public int GetClearRank()
    {
        if (WrongUses == 0) return 3;
        if (WrongUses == 1) return 2;
        return 1;
    }

    private void ConfigureStains()
    {
        CleaningTarget[] found = FindObjectsOfType<CleaningTarget>();
        var stains = new List<CleaningTarget>();
        Transform curatedRoot = GameObject.Find("GameplayStains_Level" + CurrentLevel.number.ToString("00"))?.transform;

        foreach (CleaningTarget target in found)
        {
            bool belongsToCuratedSet = curatedRoot != null && target.transform.IsChildOf(curatedRoot);
            if (curatedRoot != null && !belongsToCuratedSet)
            {
                // The old room stains remain in the scene for safety, but this new
                // level curve only uses the curated set placed for the mission.
                target.gameObject.SetActive(false);
                continue;
            }

            stains.Add(target);
        }
        stains.Sort((left, right) => string.Compare(left.stainName + left.name, right.stainName + right.name, System.StringComparison.Ordinal));

        int activeCount = Mathf.Min(CurrentLevel.stainTarget, stains.Count);
        for (int index = activeCount; index < stains.Count; index++)
            stains[index].gameObject.SetActive(false);
    }

    private void ConfigureItems()
    {
        var requiredNames = new HashSet<string>();
        foreach (CleaningTarget stain in FindObjectsOfType<CleaningTarget>())
        {
            if (!string.IsNullOrWhiteSpace(stain.requiredItemName))
                requiredNames.Add(stain.requiredItemName);
        }

        ItemData[] found = FindObjectsOfType<ItemData>();
        var decoys = new List<ItemData>();
        foreach (ItemData item in found)
        {
            if (!requiredNames.Contains(item.itemName))
                decoys.Add(item);
        }

        decoys.Sort((left, right) => string.Compare(left.itemName + left.name, right.itemName + right.name, System.StringComparison.Ordinal));
        int visibleDecoys = Mathf.Min(CurrentLevel.decoyItemCount, decoys.Count);
        for (int index = visibleDecoys; index < decoys.Count; index++)
            decoys[index].gameObject.SetActive(false);
    }

    private void HandleWrongItemUsed(CleaningTarget _)
    {
        CurrentCombo = 0;
        WrongUses++;
        if (WrongUses >= CurrentLevel.maxWrongUses && levelFlow != null)
            levelFlow.FailFromWrongUses(WrongUses, CurrentLevel.maxWrongUses);
    }

    private void HandleCorrectItemUsed(CleaningTarget _)
    {
        CurrentCombo++;
        BestCombo = Mathf.Max(BestCombo, CurrentCombo);
    }

    private void BuildHud()
    {
        hudRoot = new GameObject("MissionChallengeHUD", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = hudRoot.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 350;
        CanvasScaler scaler = hudRoot.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        GameObject panel = CreateUiObject("ChallengePanel", hudRoot.transform);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = panelRect.anchorMax = new Vector2(0.5f, 1f);
        panelRect.pivot = new Vector2(0.5f, 1f);
        panelRect.anchoredPosition = new Vector2(0f, -22f);
        panelRect.sizeDelta = new Vector2(590f, 104f);
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.015f, 0.105f, 0.17f, 0.94f);
        panelImage.raycastTarget = false;
        Outline outline = panel.AddComponent<Outline>();
        outline.effectColor = new Color(0.28f, 0.78f, 0.96f, 0.72f);
        outline.effectDistance = new Vector2(1f, -1f);

        TMP_FontAsset font = Resources.Load<TMP_FontAsset>("UI/Fonts/Sarabun-Bold SDF");
        if (font == null) font = TMP_Settings.defaultFontAsset;
        CreateText(panel.transform, "Level", "LEVEL " + CurrentLevel.number.ToString("00") + "  /  " + CurrentLevel.difficulty + "  /  " + CurrentLevel.challenge, 14f,
            new Color(0.41f, 0.88f, 1f, 1f), new Vector2(0f, 32f), new Vector2(540f, 22f), font, TextAlignmentOptions.Center);
        progressText = CreateText(panel.transform, "Progress", "", 17f, Color.white, new Vector2(-120f, 8f), new Vector2(260f, 24f), font, TextAlignmentOptions.Left);
        mistakesText = CreateText(panel.transform, "Mistakes", "", 14f, Color.white, new Vector2(153f, 8f), new Vector2(250f, 24f), font, TextAlignmentOptions.Right);
        comboText = CreateText(panel.transform, "Combo", "", 13f, new Color(0.60f, 0.87f, 0.98f, 1f), new Vector2(0f, -15f), new Vector2(500f, 20f), font, TextAlignmentOptions.Center);

        GameObject bar = CreateUiObject("ProgressBar", panel.transform);
        RectTransform barRect = bar.GetComponent<RectTransform>();
        barRect.anchorMin = barRect.anchorMax = new Vector2(0.5f, 0.5f);
        barRect.anchoredPosition = new Vector2(0f, -38f);
        barRect.sizeDelta = new Vector2(540f, 6f);
        Image barImage = bar.AddComponent<Image>();
        barImage.color = new Color(0.03f, 0.05f, 0.10f, 1f);
        barImage.raycastTarget = false;

        GameObject fill = CreateUiObject("Fill", bar.transform);
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = new Vector2(0f, 0f);
        fillRect.anchorMax = new Vector2(0f, 1f);
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        progressFill = fill.AddComponent<Image>();
        progressFill.color = new Color(0.32f, 0.86f, 1f, 1f);
        progressFill.raycastTarget = false;
    }

    private static GameObject CreateUiObject(string name, Transform parent)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform));
        obj.transform.SetParent(parent, false);
        return obj;
    }

    private static TMP_Text CreateText(Transform parent, string name, string value, float size, Color color, Vector2 position, Vector2 bounds, TMP_FontAsset font, TextAlignmentOptions alignment)
    {
        GameObject obj = CreateUiObject(name, parent);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = bounds;
        TextMeshProUGUI text = obj.AddComponent<TextMeshProUGUI>();
        text.font = font;
        text.text = value;
        text.fontSize = size;
        text.fontStyle = FontStyles.Bold;
        text.color = color;
        text.alignment = alignment;
        text.raycastTarget = false;
        return text;
    }
}
