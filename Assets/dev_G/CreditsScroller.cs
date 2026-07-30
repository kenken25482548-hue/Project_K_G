using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class CreditsScroller : MonoBehaviour
{
    [Header("Legacy UI (kept so the current scene stays compatible)")]
    public RectTransform creditsText;
    public Button backButton;

    [Header("Legacy Settings")]
    public float scrollSpeed = 80f;
    public float endY = 1200f;

    private const string PremiumRootName = "AAA_Credits";
    private const string BackgroundResourcePath = "UI/Credits_Character_Background";
    private const string BackgroundVideoResourcePath = "UI/MainMenu_AAA_Background_Animated";
    private const bool UseAnimatedBackgroundVideo = false;
    private const string UiFontResourcePath = "UI/Fonts/Kanit-SemiBold SDF";
    private const string TitleFontResourcePath = "UI/Fonts/ChakraPetch-Bold SDF";

    private static readonly Color Cyan = new Color(0.20f, 0.80f, 1f, 1f);
    private static readonly Color SoftWhite = new Color(0.91f, 0.95f, 0.98f, 1f);
    private static readonly Color Muted = new Color(0.55f, 0.65f, 0.73f, 1f);

    private sealed class FloatingElement
    {
        public RectTransform rect;
        public Vector2 restPosition;
        public float amplitude;
        public float speed;
        public float phase;
    }

    private readonly List<FloatingElement> floatingElements = new List<FloatingElement>();

    private TMP_FontAsset uiFont;
    private TMP_FontAsset titleFont;
    private CanvasGroup screenGroup;
    private RectTransform contentRoot;
    private RectTransform outerRing;
    private RectTransform innerRing;
    private RectTransform scanLine;
    private Sprite ringSprite;
    private Texture2D ringTexture;
    private float entranceTime;
    private bool isLeaving;

    void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        uiFont = Resources.Load<TMP_FontAsset>(UiFontResourcePath);
        titleFont = Resources.Load<TMP_FontAsset>(TitleFontResourcePath);

        if (uiFont == null)
            uiFont = TMP_Settings.defaultFontAsset;
        if (titleFont == null)
            titleFont = uiFont;

        BuildPremiumCredits();
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            GoToMainMenu();
        }

        AnimateEntrance();
        AnimateInterface();
    }

    void BuildPremiumCredits()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("CreditsScroller: ไม่พบ Canvas สำหรับสร้างหน้าเครดิต");
            return;
        }

        Transform existing = canvas.transform.Find(PremiumRootName);
        if (existing != null)
        {
            if (Application.isPlaying)
                Destroy(existing.gameObject);
            else
                DestroyImmediate(existing.gameObject);
        }

        for (int i = 0; i < canvas.transform.childCount; i++)
            canvas.transform.GetChild(i).gameObject.SetActive(false);

        GameObject rootObject = CreateUiObject(PremiumRootName, canvas.transform);
        rootObject.SetActive(true);
        RectTransform root = rootObject.GetComponent<RectTransform>();
        Stretch(root);

        screenGroup = rootObject.AddComponent<CanvasGroup>();
        screenGroup.alpha = 0f;

        BuildBackground(root);

        GameObject veilObject = CreateUiObject("CinematicVeil", root);
        RectTransform veilRect = veilObject.GetComponent<RectTransform>();
        Stretch(veilRect);
        Image veil = veilObject.AddComponent<Image>();
        veil.color = new Color(0.005f, 0.015f, 0.04f, 0.72f);
        veil.raycastTarget = false;

        contentRoot = CreateUiObject("CreditsContent", root).GetComponent<RectTransform>();
        Stretch(contentRoot);
        contentRoot.anchoredPosition = new Vector2(-54f, 0f);

        BuildHeader(contentRoot);
        BuildTeamPanel(contentRoot);
        BuildMissionSeal(contentRoot);
        BuildFooter(contentRoot);
    }

    void BuildBackground(RectTransform root)
    {
        GameObject backgroundObject = CreateUiObject("CinematicBackground", root);
        RectTransform backgroundRect = backgroundObject.GetComponent<RectTransform>();
        Stretch(backgroundRect);

        RawImage background = backgroundObject.AddComponent<RawImage>();
        background.texture = Resources.Load<Texture2D>(BackgroundResourcePath);
        background.color = background.texture != null
            ? Color.white
            : new Color(0.015f, 0.055f, 0.10f, 1f);
        background.raycastTarget = false;

        VideoClip animatedBackground = UseAnimatedBackgroundVideo
            ? Resources.Load<VideoClip>(BackgroundVideoResourcePath)
            : null;
        if (animatedBackground != null)
        {
            MainMenuBackgroundVideo videoBackground =
                backgroundObject.AddComponent<MainMenuBackgroundVideo>();
            videoBackground.Configure(background, animatedBackground);
        }

        GameObject motionObject = CreateUiObject("CinematicMotion", root);
        RectTransform motionRect = motionObject.GetComponent<RectTransform>();
        Stretch(motionRect);

        GameObject sweepObject = CreateUiObject("LightSweep", motionObject.transform);
        RectTransform sweepRect = sweepObject.GetComponent<RectTransform>();
        sweepRect.anchorMin = sweepRect.anchorMax = new Vector2(0.5f, 0.5f);
        sweepRect.pivot = new Vector2(0.5f, 0.5f);
        sweepRect.sizeDelta = new Vector2(760f, 1500f);
        RawImage sweep = sweepObject.AddComponent<RawImage>();
        sweep.color = new Color(0.18f, 0.76f, 1f, 0.08f);
        sweep.raycastTarget = false;

        MainMenuBackgroundMotion motion =
            motionObject.AddComponent<MainMenuBackgroundMotion>();
        motion.Configure(background, sweep);
    }

    void BuildHeader(Transform parent)
    {
        CreatePanel(
            parent,
            "LeftRail",
            new Vector2(72f, -72f),
            new Vector2(5f, 936f),
            Cyan);

        TextMeshProUGUI eyebrow = CreateText(
            parent,
            "ArchiveLabel",
            "PROJECT ARCHIVE  /  FINAL REPORT  /  2026",
            21f,
            Cyan,
            new Vector2(112f, -92f),
            new Vector2(900f, 36f),
            TextAlignmentOptions.Left,
            FontStyles.Bold,
            3f);
        eyebrow.font = titleFont;

        TextMeshProUGUI title = CreateText(
            parent,
            "CreditsTitle",
            "MISSION COMPLETE",
            72f,
            SoftWhite,
            new Vector2(108f, -142f),
            new Vector2(1180f, 104f),
            TextAlignmentOptions.Left,
            FontStyles.Bold,
            1.5f);
        title.font = titleFont;
        title.enableWordWrapping = false;

        TextMeshProUGUI gameTitle = CreateText(
            parent,
            "GameTitle",
            "CLEAN & LEARN",
            30f,
            Cyan,
            new Vector2(112f, -250f),
            new Vector2(640f, 48f),
            TextAlignmentOptions.Left,
            FontStyles.Bold,
            4f);
        gameTitle.font = titleFont;

        CreateText(
            parent,
            "ThankYou",
            "ขอบคุณที่ร่วมทำทุกภารกิจจนสำเร็จ",
            25f,
            new Color(0.75f, 0.82f, 0.88f, 1f),
            new Vector2(112f, -304f),
            new Vector2(760f, 44f),
            TextAlignmentOptions.Left);

        CreatePanel(
            parent,
            "HeaderRule",
            new Vector2(112f, -354f),
            new Vector2(1310f, 2f),
            new Color(0.23f, 0.55f, 0.72f, 0.35f));
    }

    void BuildTeamPanel(Transform parent)
    {
        GameObject panelObject = CreatePanel(
            parent,
            "TeamPanel",
            new Vector2(112f, -388f),
            new Vector2(1310f, 488f),
            new Color(0.015f, 0.06f, 0.105f, 0.88f));
        AddFloatingElement(panelObject.GetComponent<RectTransform>(), 2.5f, 0.52f, 0f);

        CreatePanel(
            panelObject.transform,
            "TopAccent",
            Vector2.zero,
            new Vector2(1310f, 4f),
            Cyan);

        TextMeshProUGUI section = CreateText(
            panelObject.transform,
            "TeamLabel",
            "DEVELOPMENT TEAM",
            18f,
            Cyan,
            new Vector2(30f, -31f),
            new Vector2(520f, 34f),
            TextAlignmentOptions.Left,
            FontStyles.Bold,
            3f);
        section.font = titleFont;

        CreateDeveloperCard(
            panelObject.transform,
            "Developer01",
            "01",
            "GAME DEVELOPER",
            "นายรัตนนิล พูนพวง",
            new Vector2(30f, -86f));

        CreateDeveloperCard(
            panelObject.transform,
            "Developer02",
            "02",
            "GAME DEVELOPER",
            "นายกรวิช แพงชาลี",
            new Vector2(650f, -86f));

        CreatePanel(
            panelObject.transform,
            "InfoRule",
            new Vector2(30f, -344f),
            new Vector2(1250f, 1f),
            new Color(0.24f, 0.42f, 0.54f, 0.42f));

        CreateInfoBlock(
            panelObject.transform,
            "ProjectInfo",
            "PROJECT",
            "เรียนรู้ผ่านการทำความสะอาด",
            new Vector2(30f, -372f),
            new Vector2(450f, 84f));

        CreateInfoBlock(
            panelObject.transform,
            "EngineInfo",
            "DEVELOPED WITH",
            "UNITY",
            new Vector2(500f, -372f),
            new Vector2(320f, 84f));

        CreateInfoBlock(
            panelObject.transform,
            "ReleaseInfo",
            "RELEASE",
            "© 2026  CLEAN & LEARN",
            new Vector2(840f, -372f),
            new Vector2(420f, 84f));

        GameObject scanObject = CreatePanel(
            panelObject.transform,
            "ScanningLine",
            new Vector2(0f, -8f),
            new Vector2(1310f, 2f),
            new Color(0.30f, 0.85f, 1f, 0.24f));
        scanLine = scanObject.GetComponent<RectTransform>();
    }

    void CreateDeveloperCard(
        Transform parent,
        string objectName,
        string number,
        string role,
        string developerName,
        Vector2 position)
    {
        GameObject cardObject = CreatePanel(
            parent,
            objectName,
            position,
            new Vector2(600f, 228f),
            new Color(0.025f, 0.105f, 0.17f, 0.96f));
        RectTransform cardRect = cardObject.GetComponent<RectTransform>();
        AddFloatingElement(
            cardRect,
            4f,
            objectName.EndsWith("01") ? 0.72f : 0.64f,
            objectName.EndsWith("01") ? 0f : 1.8f);

        CreatePanel(
            cardObject.transform,
            "Accent",
            Vector2.zero,
            new Vector2(5f, 228f),
            Cyan);

        TextMeshProUGUI numberLabel = CreateText(
            cardObject.transform,
            "Number",
            number,
            62f,
            new Color(0.22f, 0.80f, 1f, 0.16f),
            new Vector2(34f, -25f),
            new Vector2(125f, 88f),
            TextAlignmentOptions.Left,
            FontStyles.Bold);
        numberLabel.font = titleFont;

        TextMeshProUGUI roleLabel = CreateText(
            cardObject.transform,
            "Role",
            role,
            16f,
            Cyan,
            new Vector2(158f, -43f),
            new Vector2(390f, 32f),
            TextAlignmentOptions.Left,
            FontStyles.Bold,
            2.5f);
        roleLabel.font = titleFont;

        CreateText(
            cardObject.transform,
            "Name",
            developerName,
            31f,
            SoftWhite,
            new Vector2(158f, -88f),
            new Vector2(405f, 58f),
            TextAlignmentOptions.Left,
            FontStyles.Bold);

        CreateText(
            cardObject.transform,
            "Contribution",
            "GAMEPLAY  •  SYSTEM  •  LEVEL DESIGN",
            13f,
            Muted,
            new Vector2(158f, -150f),
            new Vector2(410f, 30f),
            TextAlignmentOptions.Left,
            FontStyles.Bold,
            1.5f);

        CreatePanel(
            cardObject.transform,
            "StatusDot",
            new Vector2(158f, -191f),
            new Vector2(9f, 9f),
            new Color(0.30f, 1f, 0.70f, 1f));

        CreateText(
            cardObject.transform,
            "Status",
            "MISSION VERIFIED",
            12f,
            new Color(0.54f, 0.72f, 0.80f, 1f),
            new Vector2(179f, -196f),
            new Vector2(260f, 24f),
            TextAlignmentOptions.Left,
            FontStyles.Bold,
            2f);
    }

    void CreateInfoBlock(
        Transform parent,
        string objectName,
        string label,
        string value,
        Vector2 position,
        Vector2 size)
    {
        GameObject block = CreateUiObject(objectName, parent);
        RectTransform rect = block.GetComponent<RectTransform>();
        SetTopLeft(rect, position, size);

        TextMeshProUGUI labelText = CreateText(
            block.transform,
            "Label",
            label,
            12f,
            Muted,
            Vector2.zero,
            new Vector2(size.x, 23f),
            TextAlignmentOptions.Left,
            FontStyles.Bold,
            2f);
        labelText.font = titleFont;

        CreateText(
            block.transform,
            "Value",
            value,
            20f,
            SoftWhite,
            new Vector2(0f, -29f),
            new Vector2(size.x, 42f),
            TextAlignmentOptions.Left,
            FontStyles.Bold);
    }

    void BuildMissionSeal(Transform parent)
    {
        ringSprite = CreateRingSprite();

        GameObject sealObject = CreateUiObject("MissionSeal", parent);
        RectTransform sealRect = sealObject.GetComponent<RectTransform>();
        SetTopLeft(sealRect, new Vector2(1510f, -305f), new Vector2(310f, 310f));
        AddFloatingElement(sealRect, 7f, 0.45f, 0.7f);

        outerRing = CreateRing(
            sealObject.transform,
            "OuterRing",
            new Vector2(310f, 310f),
            new Color(0.20f, 0.80f, 1f, 0.42f));
        innerRing = CreateRing(
            sealObject.transform,
            "InnerRing",
            new Vector2(230f, 230f),
            new Color(0.36f, 0.89f, 1f, 0.70f));

        TextMeshProUGUI percent = CreateCenteredText(
            sealObject.transform,
            "Percent",
            "100%",
            62f,
            SoftWhite,
            new Vector2(250f, 82f),
            new Vector2(0f, 18f),
            FontStyles.Bold);
        percent.font = titleFont;

        TextMeshProUGUI complete = CreateCenteredText(
            sealObject.transform,
            "CompleteLabel",
            "MISSION\nCOMPLETE",
            15f,
            Cyan,
            new Vector2(220f, 58f),
            new Vector2(0f, -64f),
            FontStyles.Bold);
        complete.font = titleFont;
        complete.characterSpacing = 3f;

        CreateText(
            parent,
            "SealCaption",
            "ALL OBJECTIVES CLEARED",
            13f,
            Muted,
            new Vector2(1510f, -634f),
            new Vector2(310f, 28f),
            TextAlignmentOptions.Center,
            FontStyles.Bold,
            2.2f);
    }

    void BuildFooter(Transform parent)
    {
        CreatePanel(
            parent,
            "FooterDot",
            new Vector2(112f, -1009f),
            new Vector2(10f, 10f),
            Cyan);

        TextMeshProUGUI footer = CreateText(
            parent,
            "FooterStatus",
            "PRESS ANY KEY TO RETURN TO MAIN MENU",
            16f,
            SoftWhite,
            new Vector2(143f, -1014f),
            new Vector2(620f, 28f),
            TextAlignmentOptions.Left,
            FontStyles.Bold,
            2f);
        footer.font = titleFont;

    }

    Button CreateReturnButton(Transform parent, Vector2 position, Vector2 size)
    {
        GameObject buttonObject = CreateUiObject("ReturnButton", parent);
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        SetTopLeft(rect, position, size);

        Image background = buttonObject.AddComponent<Image>();
        background.color = new Color(0.025f, 0.105f, 0.17f, 0.96f);

        Button button = buttonObject.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.76f, 0.95f, 1f, 1f);
        colors.pressedColor = new Color(0.55f, 0.86f, 1f, 1f);
        colors.selectedColor = colors.highlightedColor;
        colors.fadeDuration = 0.12f;
        button.colors = colors;

        CreatePanel(
            buttonObject.transform,
            "Accent",
            Vector2.zero,
            new Vector2(5f, size.y),
            Cyan);

        TextMeshProUGUI label = CreateCenteredText(
            buttonObject.transform,
            "Label",
            "กลับสู่เมนูหลัก   ESC",
            22f,
            SoftWhite,
            new Vector2(size.x - 26f, size.y),
            new Vector2(10f, 0f),
            FontStyles.Bold);
        label.raycastTarget = false;

        MenuButtonHoverMotion hover = buttonObject.AddComponent<MenuButtonHoverMotion>();
        hover.Configure(new Vector2(-10f, 0f), 1.035f, 16f);
        return button;
    }

    void AnimateEntrance()
    {
        if (screenGroup == null || contentRoot == null)
            return;

        entranceTime += Time.unscaledDeltaTime;
        float normalized = Mathf.Clamp01(entranceTime / 1.05f);
        float eased = 1f - Mathf.Pow(1f - normalized, 3f);
        screenGroup.alpha = eased;
        contentRoot.anchoredPosition = Vector2.Lerp(
            new Vector2(-54f, 0f),
            Vector2.zero,
            eased);
    }

    void AnimateInterface()
    {
        float time = Time.unscaledTime;

        if (outerRing != null)
            outerRing.localRotation = Quaternion.Euler(0f, 0f, -time * 13f);
        if (innerRing != null)
            innerRing.localRotation = Quaternion.Euler(0f, 0f, time * 21f);
        if (scanLine != null)
        {
            Vector2 position = scanLine.anchoredPosition;
            position.y = -10f - Mathf.PingPong(time * 36f, 455f);
            scanLine.anchoredPosition = position;
        }

        for (int i = 0; i < floatingElements.Count; i++)
        {
            FloatingElement element = floatingElements[i];
            if (element.rect == null)
                continue;

            Vector2 position = element.restPosition;
            position.y += Mathf.Sin(time * element.speed + element.phase) * element.amplitude;
            element.rect.anchoredPosition = position;
        }
    }

    public void GoToMainMenu()
    {
        if (isLeaving)
            return;

        isLeaving = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("0Mainmenu0");
    }

    GameObject CreatePanel(
        Transform parent,
        string objectName,
        Vector2 position,
        Vector2 size,
        Color color)
    {
        GameObject panelObject = CreateUiObject(objectName, parent);
        RectTransform rect = panelObject.GetComponent<RectTransform>();
        SetTopLeft(rect, position, size);
        Image image = panelObject.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return panelObject;
    }

    TextMeshProUGUI CreateText(
        Transform parent,
        string objectName,
        string content,
        float fontSize,
        Color color,
        Vector2 position,
        Vector2 size,
        TextAlignmentOptions alignment,
        FontStyles style = FontStyles.Normal,
        float characterSpacing = 0f)
    {
        GameObject textObject = CreateUiObject(objectName, parent);
        RectTransform rect = textObject.GetComponent<RectTransform>();
        SetTopLeft(rect, position, size);

        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.font = uiFont;
        text.text = content;
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.color = color;
        text.alignment = alignment;
        text.characterSpacing = characterSpacing;
        text.enableWordWrapping = true;
        text.overflowMode = TextOverflowModes.Overflow;
        text.raycastTarget = false;
        return text;
    }

    TextMeshProUGUI CreateCenteredText(
        Transform parent,
        string objectName,
        string content,
        float fontSize,
        Color color,
        Vector2 size,
        Vector2 position,
        FontStyles style)
    {
        GameObject textObject = CreateUiObject(objectName, parent);
        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = position;

        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.font = uiFont;
        text.text = content;
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.color = color;
        text.alignment = TextAlignmentOptions.Center;
        text.enableWordWrapping = true;
        text.overflowMode = TextOverflowModes.Overflow;
        text.raycastTarget = false;
        return text;
    }

    RectTransform CreateRing(
        Transform parent,
        string objectName,
        Vector2 size,
        Color color)
    {
        GameObject ringObject = CreateUiObject(objectName, parent);
        RectTransform rect = ringObject.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;

        Image image = ringObject.AddComponent<Image>();
        image.sprite = ringSprite;
        image.color = color;
        image.raycastTarget = false;
        return rect;
    }

    Sprite CreateRingSprite()
    {
        const int size = 256;
        ringTexture = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            name = "Credits Mission Ring",
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };

        Vector2 center = new Vector2((size - 1f) * 0.5f, (size - 1f) * 0.5f);
        float radius = size * 0.455f;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 point = new Vector2(x, y);
                float distance = Vector2.Distance(point, center);
                float ring = 1f - Mathf.Clamp01(Mathf.Abs(distance - radius) / 2.8f);
                float angle = Mathf.Atan2(y - center.y, x - center.x);
                float dash = Mathf.Sin(angle * 18f) > -0.55f ? 1f : 0.12f;
                float alpha = ring * dash;
                ringTexture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        ringTexture.Apply(false, true);
        Sprite sprite = Sprite.Create(
            ringTexture,
            new Rect(0f, 0f, size, size),
            new Vector2(0.5f, 0.5f),
            size);
        sprite.name = "Credits Mission Ring Sprite";
        return sprite;
    }

    void AddFloatingElement(RectTransform rect, float amplitude, float speed, float phase)
    {
        floatingElements.Add(new FloatingElement
        {
            rect = rect,
            restPosition = rect.anchoredPosition,
            amplitude = amplitude,
            speed = speed,
            phase = phase
        });
    }

    GameObject CreateUiObject(string objectName, Transform parent)
    {
        GameObject uiObject = new GameObject(objectName, typeof(RectTransform));
        uiObject.layer = LayerMask.NameToLayer("UI");
        uiObject.transform.SetParent(parent, false);
        uiObject.transform.localScale = Vector3.one;
        return uiObject;
    }

    static void SetTopLeft(RectTransform rect, Vector2 position, Vector2 size)
    {
        rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }

    static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.pivot = new Vector2(0.5f, 0.5f);
    }

    void OnDestroy()
    {
        ReleaseGeneratedObject(ringSprite);
        ReleaseGeneratedObject(ringTexture);
    }

    static void ReleaseGeneratedObject(Object generatedObject)
    {
        if (generatedObject == null)
            return;

        if (Application.isPlaying)
            Destroy(generatedObject);
        else
            DestroyImmediate(generatedObject);
    }
}
