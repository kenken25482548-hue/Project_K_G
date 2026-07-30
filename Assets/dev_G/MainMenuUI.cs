using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class MainMenuUI : MonoBehaviour
{
    [Header("Scene")]
    public string firstSceneName = "1bathroom1";
    public string loadingSceneName = "9Loadingscene9";

    [Header("Sounds")]
    public AudioClip hoverSound;
    public AudioClip clickSound;
    public AudioClip bgmClip;

    private const string PremiumRootName = "AAA_MainMenu";
    private const string BackgroundResourcePath = "UI/MainMenu_Character_Background";
    private const string BackgroundVideoResourcePath = "UI/MainMenu_AAA_Background_Animated";
    private const bool UseAnimatedBackgroundVideo = false;
    private const string FallbackFontResourcePath = "Fonts & Materials/MiPancake SDF";
    private const string UiFontResourcePath = "UI/Fonts/Kanit-SemiBold SDF";
    private const string TitleFontResourcePath = "UI/Fonts/ChakraPetch-Bold SDF";

    private readonly Color cyan = new Color(0.25f, 0.80f, 1f, 1f);
    private readonly Color warmWhite = new Color(0.93f, 0.97f, 1f, 1f);
    private readonly Color mutedWhite = new Color(0.69f, 0.76f, 0.82f, 1f);
    private readonly Color panelNavy = new Color(0.012f, 0.028f, 0.052f, 0.96f);

    private AudioSource audioSource;
    private TMP_FontAsset uiFont;
    private TMP_FontAsset titleFont;
    private GameObject premiumRoot;
    private GameObject howToPlayPanel;
    private Button startButton;
    private Button howToButton;
    private Button closeHowToButton;
    private bool isStartingGame;

    void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        audioSource = GetComponent<AudioSource>();
        TMP_FontAsset fallbackFont = Resources.Load<TMP_FontAsset>(FallbackFontResourcePath);
        uiFont = Resources.Load<TMP_FontAsset>(UiFontResourcePath);
        titleFont = Resources.Load<TMP_FontAsset>(TitleFontResourcePath);

        if (uiFont == null)
            uiFont = fallbackFont;

        if (titleFont == null)
            titleFont = uiFont;

        if (bgmClip != null && audioSource != null)
        {
            audioSource.clip = bgmClip;
            audioSource.loop = true;
            audioSource.volume = 0.5f;
            audioSource.Play();
        }

        BuildPremiumMenu();

        if (premiumRoot != null)
        {
            Button[] buttons = premiumRoot.GetComponentsInChildren<Button>(true);
            foreach (Button button in buttons)
                AddHoverSound(button);
        }

        if (EventSystem.current != null && startButton != null)
            EventSystem.current.SetSelectedGameObject(startButton.gameObject);
    }

    void Update()
    {
        if (howToPlayPanel != null &&
            howToPlayPanel.activeSelf &&
            Input.GetKeyDown(KeyCode.Escape))
        {
            CloseHowToPlay();
        }
    }

    void BuildPremiumMenu()
    {
        Canvas targetCanvas = FindMainCanvas();
        if (targetCanvas == null)
        {
            Debug.LogError("MainMenuUI: ไม่พบ Canvas สำหรับสร้างหน้าเมนู");
            return;
        }

        Transform existingRoot = targetCanvas.transform.Find(PremiumRootName);
        if (existingRoot != null)
        {
            premiumRoot = existingRoot.gameObject;
            return;
        }

        for (int i = 0; i < targetCanvas.transform.childCount; i++)
            targetCanvas.transform.GetChild(i).gameObject.SetActive(false);

        premiumRoot = CreateUiObject(PremiumRootName, targetCanvas.transform);
        Stretch(premiumRoot.GetComponent<RectTransform>());

        BuildBackground(premiumRoot.transform);
        BuildMainPanel(premiumRoot.transform);
        BuildHowToPlayPanel(premiumRoot.transform);
    }

    Canvas FindMainCanvas()
    {
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            if (canvas.isRootCanvas && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                return canvas;
        }

        return canvases.Length > 0 ? canvases[0] : null;
    }

    void BuildBackground(Transform parent)
    {
        GameObject backgroundObject = CreateUiObject("CinematicBackground", parent);
        Stretch(backgroundObject.GetComponent<RectTransform>());

        RawImage background = backgroundObject.AddComponent<RawImage>();
        background.raycastTarget = false;
        background.color = Color.white;

        Texture2D backgroundTexture = Resources.Load<Texture2D>(BackgroundResourcePath);
        if (backgroundTexture != null)
        {
            background.texture = backgroundTexture;

            AspectRatioFitter fitter = backgroundObject.AddComponent<AspectRatioFitter>();
            fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            fitter.aspectRatio = (float)backgroundTexture.width / backgroundTexture.height;
        }
        else
        {
            background.color = new Color(0.015f, 0.035f, 0.065f, 1f);
            Debug.LogWarning($"MainMenuUI: ไม่พบภาพ Resources/{BackgroundResourcePath}");
        }

        VideoClip animatedBackground = UseAnimatedBackgroundVideo
            ? Resources.Load<VideoClip>(BackgroundVideoResourcePath)
            : null;
        if (animatedBackground != null)
        {
            MainMenuBackgroundVideo videoBackground =
                backgroundObject.AddComponent<MainMenuBackgroundVideo>();
            videoBackground.Configure(background, animatedBackground);
        }
        else if (UseAnimatedBackgroundVideo)
        {
            Debug.LogWarning(
                $"MainMenuUI: ไม่พบวิดีโอ Resources/{BackgroundVideoResourcePath}");
        }

        GameObject shadeObject = CreateUiObject("CinematicShade", parent);
        Stretch(shadeObject.GetComponent<RectTransform>());
        Image shade = shadeObject.AddComponent<Image>();
        shade.color = new Color(0.005f, 0.012f, 0.025f, 0.16f);
        shade.raycastTarget = false;

        GameObject motionObject = CreateUiObject("CinematicMotion", parent);
        Stretch(motionObject.GetComponent<RectTransform>());

        GameObject sweepObject = CreateUiObject("MovingLightSweep", motionObject.transform);
        RectTransform sweepRect = sweepObject.GetComponent<RectTransform>();
        sweepRect.anchorMin = new Vector2(0f, 0.5f);
        sweepRect.anchorMax = new Vector2(0f, 0.5f);
        sweepRect.pivot = new Vector2(0.5f, 0.5f);
        sweepRect.anchoredPosition = new Vector2(-420f, 0f);
        sweepRect.sizeDelta = new Vector2(330f, 1450f);
        sweepRect.localRotation = Quaternion.Euler(0f, 0f, -12f);

        RawImage sweepImage = sweepObject.AddComponent<RawImage>();
        sweepImage.color = new Color(0.18f, 0.76f, 1f, 0.10f);
        sweepImage.raycastTarget = false;

        MainMenuBackgroundMotion backgroundMotion =
            motionObject.AddComponent<MainMenuBackgroundMotion>();
        backgroundMotion.Configure(background, sweepImage);
    }

    void BuildMainPanel(Transform parent)
    {
        GameObject leftPanel = CreateUiObject("CommandPanel", parent);
        RectTransform panelRect = leftPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = new Vector2(0.48f, 1f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        TextMeshProUGUI operationLabel = CreateText(
            leftPanel.transform,
            "OperationLabel",
            "CLEANING OPERATIONS  /  01",
            22f,
            cyan,
            new Vector2(0f, 1f),
            new Vector2(0f, 1f),
            new Vector2(104f, -122f),
            new Vector2(670f, 42f),
            TextAlignmentOptions.Left,
            FontStyles.Bold,
            3f);
        operationLabel.font = titleFont;

        TextMeshProUGUI title = CreateText(
            leftPanel.transform,
            "GameTitle",
            "CLEAN & LEARN",
            72f,
            warmWhite,
            new Vector2(0f, 1f),
            new Vector2(0f, 1f),
            new Vector2(98f, -172f),
            new Vector2(740f, 105f),
            TextAlignmentOptions.TopLeft,
            FontStyles.Bold,
            1.5f);
        title.font = titleFont;
        title.enableWordWrapping = false;

        CreateText(
            leftPanel.transform,
            "Tagline",
            "สำรวจคราบ  เลือกให้ถูก  แล้วจัดการทุกจุด",
            23f,
            mutedWhite,
            new Vector2(0f, 1f),
            new Vector2(0f, 1f),
            new Vector2(104f, -292f),
            new Vector2(730f, 45f),
            TextAlignmentOptions.Left);

        startButton = CreateMenuButton(
            leftPanel.transform,
            "StartGameButton",
            "START MISSION",
            "01",
            new Vector2(104f, -375f),
            StartGame);

        howToButton = CreateMenuButton(
            leftPanel.transform,
            "HowToPlayButton",
            "HOW TO PLAY",
            "02",
            new Vector2(104f, -460f),
            OpenHowToPlay);

        CreateMenuButton(
            leftPanel.transform,
            "QuitGameButton",
            "QUIT GAME",
            "03",
            new Vector2(104f, -545f),
            QuitGame);

        CreateText(
            leftPanel.transform,
            "FooterStatus",
            "READY FOR DEPLOYMENT",
            17f,
            new Color(mutedWhite.r, mutedWhite.g, mutedWhite.b, 0.68f),
            new Vector2(0f, 0f),
            new Vector2(0f, 0f),
            new Vector2(104f, 48f),
            new Vector2(460f, 34f),
            TextAlignmentOptions.Left,
            FontStyles.Bold,
            2f);

        GameObject statusDot = CreateUiObject("StatusDot", leftPanel.transform);
        RectTransform dotRect = statusDot.GetComponent<RectTransform>();
        dotRect.anchorMin = Vector2.zero;
        dotRect.anchorMax = Vector2.zero;
        dotRect.pivot = Vector2.zero;
        dotRect.anchoredPosition = new Vector2(78f, 59f);
        dotRect.sizeDelta = new Vector2(10f, 10f);
        Image dotImage = statusDot.AddComponent<Image>();
        dotImage.color = cyan;
        dotImage.raycastTarget = false;

        CreateText(
            parent,
            "BuildLabel",
            "CLEAN & LEARN  •  DEV BUILD",
            16f,
            new Color(0.82f, 0.88f, 0.93f, 0.58f),
            new Vector2(1f, 0f),
            new Vector2(1f, 0f),
            new Vector2(-42f, 34f),
            new Vector2(420f, 30f),
            TextAlignmentOptions.Right);
    }

    Button CreateMenuButton(
        Transform parent,
        string objectName,
        string label,
        string number,
        Vector2 anchoredPosition,
        UnityAction onClick)
    {
        GameObject buttonObject = CreateUiObject(objectName, parent);
        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0f, 1f);
        buttonRect.anchorMax = new Vector2(0f, 1f);
        buttonRect.pivot = new Vector2(0f, 1f);
        buttonRect.anchoredPosition = anchoredPosition;
        buttonRect.sizeDelta = new Vector2(470f, 66f);

        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = new Color(0.025f, 0.065f, 0.105f, 0.78f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonImage;
        button.transition = Selectable.Transition.ColorTint;

        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.48f, 0.80f, 1f, 1f);
        colors.selectedColor = new Color(0.32f, 0.70f, 0.96f, 1f);
        colors.pressedColor = new Color(0.22f, 0.55f, 0.78f, 1f);
        colors.disabledColor = new Color(0.35f, 0.39f, 0.43f, 0.5f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.12f;
        button.colors = colors;
        button.onClick.AddListener(onClick);

        GameObject accent = CreateUiObject("Accent", buttonObject.transform);
        RectTransform accentRect = accent.GetComponent<RectTransform>();
        accentRect.anchorMin = new Vector2(0f, 0f);
        accentRect.anchorMax = new Vector2(0f, 1f);
        accentRect.pivot = new Vector2(0f, 0.5f);
        accentRect.anchoredPosition = Vector2.zero;
        accentRect.sizeDelta = new Vector2(4f, 0f);
        Image accentImage = accent.AddComponent<Image>();
        accentImage.color = cyan;
        accentImage.raycastTarget = false;

        CreateText(
            buttonObject.transform,
            "Number",
            number,
            15f,
            new Color(cyan.r, cyan.g, cyan.b, 0.82f),
            new Vector2(0f, 0f),
            new Vector2(0f, 1f),
            new Vector2(22f, 0f),
            new Vector2(46f, 0f),
            TextAlignmentOptions.Center,
            FontStyles.Bold);

        TextMeshProUGUI buttonLabel = CreateText(
            buttonObject.transform,
            "Label",
            label,
            28f,
            warmWhite,
            Vector2.zero,
            Vector2.one,
            new Vector2(73f, 0f),
            new Vector2(-26f, 0f),
            TextAlignmentOptions.Left,
            FontStyles.Bold);
        buttonLabel.raycastTarget = false;

        MenuButtonHoverMotion hoverMotion = buttonObject.AddComponent<MenuButtonHoverMotion>();
        hoverMotion.Configure(new Vector2(16f, 0f), 1.025f, 15f);

        return button;
    }

    void BuildHowToPlayPanel(Transform parent)
    {
        howToPlayPanel = CreateUiObject("HowToPlayModal", parent);
        Stretch(howToPlayPanel.GetComponent<RectTransform>());

        Image dimmer = howToPlayPanel.AddComponent<Image>();
        dimmer.color = new Color(0.002f, 0.008f, 0.018f, 0.86f);

        GameObject modal = CreateUiObject("ModalWindow", howToPlayPanel.transform);
        RectTransform modalRect = modal.GetComponent<RectTransform>();
        modalRect.anchorMin = new Vector2(0.5f, 0.5f);
        modalRect.anchorMax = new Vector2(0.5f, 0.5f);
        modalRect.pivot = new Vector2(0.5f, 0.5f);
        modalRect.anchoredPosition = Vector2.zero;
        modalRect.sizeDelta = new Vector2(1080f, 720f);

        Image modalImage = modal.AddComponent<Image>();
        modalImage.color = panelNavy;

        GameObject topAccent = CreateUiObject("TopAccent", modal.transform);
        RectTransform topAccentRect = topAccent.GetComponent<RectTransform>();
        topAccentRect.anchorMin = new Vector2(0f, 1f);
        topAccentRect.anchorMax = new Vector2(1f, 1f);
        topAccentRect.pivot = new Vector2(0.5f, 1f);
        topAccentRect.anchoredPosition = Vector2.zero;
        topAccentRect.sizeDelta = new Vector2(0f, 5f);
        Image topAccentImage = topAccent.AddComponent<Image>();
        topAccentImage.color = cyan;
        topAccentImage.raycastTarget = false;

        CreateText(
            modal.transform,
            "ModalEyebrow",
            "FIELD MANUAL  /  วิธีการเล่น",
            20f,
            cyan,
            new Vector2(0f, 1f),
            new Vector2(0f, 1f),
            new Vector2(58f, -48f),
            new Vector2(760f, 34f),
            TextAlignmentOptions.Left,
            FontStyles.Bold,
            2f);

        CreateText(
            modal.transform,
            "ModalTitle",
            "เตรียมพร้อมก่อนเริ่มภารกิจ",
            42f,
            warmWhite,
            new Vector2(0f, 1f),
            new Vector2(0f, 1f),
            new Vector2(54f, -91f),
            new Vector2(800f, 60f),
            TextAlignmentOptions.Left,
            FontStyles.Bold);

        closeHowToButton = CreateCompactButton(
            modal.transform,
            "CloseButton",
            "X",
            new Vector2(1f, 1f),
            new Vector2(-56f, -56f),
            new Vector2(54f, 54f),
            CloseHowToPlay);

        GameObject divider = CreateUiObject("HeaderDivider", modal.transform);
        RectTransform dividerRect = divider.GetComponent<RectTransform>();
        dividerRect.anchorMin = new Vector2(0f, 1f);
        dividerRect.anchorMax = new Vector2(1f, 1f);
        dividerRect.pivot = new Vector2(0.5f, 1f);
        dividerRect.anchoredPosition = new Vector2(0f, -167f);
        dividerRect.sizeDelta = new Vector2(-108f, 1f);
        Image dividerImage = divider.AddComponent<Image>();
        dividerImage.color = new Color(0.30f, 0.45f, 0.58f, 0.38f);
        dividerImage.raycastTarget = false;

        CreateText(
            modal.transform,
            "MovementTitle",
            "การเคลื่อนที่",
            25f,
            cyan,
            new Vector2(0f, 1f),
            new Vector2(0f, 1f),
            new Vector2(58f, -199f),
            new Vector2(390f, 40f),
            TextAlignmentOptions.Left,
            FontStyles.Bold);

        CreateInstructionRow(modal.transform, new Vector2(58f, -253f), "W A S D", "เดินและเคลื่อนที่", 132f);
        CreateInstructionRow(modal.transform, new Vector2(58f, -313f), "เมาส์", "หมุนมุมกล้อง", 132f);
        CreateInstructionRow(modal.transform, new Vector2(58f, -373f), "SPACE", "กระโดด", 132f);
        CreateInstructionRow(modal.transform, new Vector2(58f, -433f), "ESC", "หยุดเกม / ปิดหน้าต่าง", 132f);

        CreateText(
            modal.transform,
            "ActionTitle",
            "การสำรวจและทำความสะอาด",
            25f,
            cyan,
            new Vector2(0f, 1f),
            new Vector2(0f, 1f),
            new Vector2(570f, -199f),
            new Vector2(450f, 40f),
            TextAlignmentOptions.Left,
            FontStyles.Bold);

        CreateInstructionRow(modal.transform, new Vector2(570f, -253f), "E", "ตรวจข้อมูลคราบ / ไอเทม", 76f);
        CreateInstructionRow(modal.transform, new Vector2(570f, -313f), "F", "หยิบไอเทม / ใช้ทำความสะอาด", 76f);
        CreateInstructionRow(modal.transform, new Vector2(570f, -373f), "1 - 5", "เลือกช่องไอเทม", 76f);

        GameObject missionBox = CreateUiObject("MissionBrief", modal.transform);
        RectTransform missionRect = missionBox.GetComponent<RectTransform>();
        missionRect.anchorMin = new Vector2(0f, 0f);
        missionRect.anchorMax = new Vector2(1f, 0f);
        missionRect.pivot = new Vector2(0.5f, 0f);
        missionRect.anchoredPosition = new Vector2(0f, 80f);
        missionRect.sizeDelta = new Vector2(-108f, 132f);
        Image missionImage = missionBox.AddComponent<Image>();
        missionImage.color = new Color(0.035f, 0.10f, 0.16f, 0.82f);

        CreateText(
            missionBox.transform,
            "MissionText",
            "<color=#55D4FF><b>เป้าหมายภารกิจ</b></color>\nสำรวจคราบให้ครบก่อนหยิบไอเทม จากนั้นเลือกน้ำยาที่เหมาะและทำความสะอาดทุกจุด\n<size=19><color=#A9B8C6>ระวัง: ใช้ไอเทมผิดจะเสียจำนวนการใช้งาน 1 ครั้ง</color></size>",
            23f,
            warmWhite,
            Vector2.zero,
            Vector2.one,
            new Vector2(24f, 12f),
            new Vector2(-204f, -12f),
            TextAlignmentOptions.Left);

        Button understoodButton = CreateCompactButton(
            missionBox.transform,
            "UnderstoodButton",
            "เข้าใจแล้ว",
            new Vector2(1f, 0.5f),
            new Vector2(-82f, 0f),
            new Vector2(142f, 52f),
            CloseHowToPlay);
        closeHowToButton = understoodButton;

        howToPlayPanel.SetActive(false);
    }

    void CreateInstructionRow(
        Transform parent,
        Vector2 anchoredPosition,
        string key,
        string description,
        float keyWidth)
    {
        GameObject keyObject = CreateUiObject($"Key_{key}", parent);
        RectTransform keyRect = keyObject.GetComponent<RectTransform>();
        keyRect.anchorMin = new Vector2(0f, 1f);
        keyRect.anchorMax = new Vector2(0f, 1f);
        keyRect.pivot = new Vector2(0f, 1f);
        keyRect.anchoredPosition = anchoredPosition;
        keyRect.sizeDelta = new Vector2(keyWidth, 44f);

        Image keyImage = keyObject.AddComponent<Image>();
        keyImage.color = new Color(0.09f, 0.18f, 0.26f, 0.94f);
        keyImage.raycastTarget = false;

        CreateText(
            keyObject.transform,
            "KeyLabel",
            key,
            18f,
            cyan,
            Vector2.zero,
            Vector2.one,
            Vector2.zero,
            Vector2.zero,
            TextAlignmentOptions.Center,
            FontStyles.Bold);

        CreateText(
            parent,
            $"Description_{key}",
            description,
            22f,
            warmWhite,
            new Vector2(0f, 1f),
            new Vector2(0f, 1f),
            anchoredPosition + new Vector2(keyWidth + 18f, 0f),
            new Vector2(330f, 44f),
            TextAlignmentOptions.Left);
    }

    Button CreateCompactButton(
        Transform parent,
        string objectName,
        string label,
        Vector2 anchor,
        Vector2 anchoredPosition,
        Vector2 size,
        UnityAction onClick)
    {
        GameObject buttonObject = CreateUiObject(objectName, parent);
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.07f, 0.20f, 0.29f, 0.96f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;

        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.55f, 0.87f, 1f, 1f);
        colors.selectedColor = new Color(0.45f, 0.80f, 1f, 1f);
        colors.pressedColor = new Color(0.28f, 0.62f, 0.82f, 1f);
        button.colors = colors;
        button.onClick.AddListener(onClick);

        CreateText(
            buttonObject.transform,
            "Label",
            label,
            20f,
            warmWhite,
            Vector2.zero,
            Vector2.one,
            Vector2.zero,
            Vector2.zero,
            TextAlignmentOptions.Center,
            FontStyles.Bold);

        MenuButtonHoverMotion hoverMotion = buttonObject.AddComponent<MenuButtonHoverMotion>();
        hoverMotion.Configure(new Vector2(6f, 0f), 1.035f, 16f);

        return button;
    }

    TextMeshProUGUI CreateText(
        Transform parent,
        string objectName,
        string content,
        float fontSize,
        Color color,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 anchoredPosition,
        Vector2 sizeDelta,
        TextAlignmentOptions alignment,
        FontStyles style = FontStyles.Normal,
        float characterSpacing = 0f)
    {
        GameObject textObject = CreateUiObject(objectName, parent);
        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = new Vector2(anchorMin.x, anchorMax.y);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;

        if (Mathf.Approximately(anchorMin.x, anchorMax.x))
        {
            Vector2 position = rect.anchoredPosition;
            Vector2 size = rect.sizeDelta;
            position.x = anchoredPosition.x;
            size.x = sizeDelta.x;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }
        else
        {
            Vector2 minimum = rect.offsetMin;
            Vector2 maximum = rect.offsetMax;
            minimum.x = anchoredPosition.x;
            maximum.x = sizeDelta.x;
            rect.offsetMin = minimum;
            rect.offsetMax = maximum;
        }

        if (Mathf.Approximately(anchorMin.y, anchorMax.y))
        {
            Vector2 position = rect.anchoredPosition;
            Vector2 size = rect.sizeDelta;
            position.y = anchoredPosition.y;
            size.y = sizeDelta.y;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }
        else
        {
            Vector2 minimum = rect.offsetMin;
            Vector2 maximum = rect.offsetMax;
            minimum.y = anchoredPosition.y;
            maximum.y = sizeDelta.y;
            rect.offsetMin = minimum;
            rect.offsetMax = maximum;
        }

        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        if (uiFont != null)
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

    GameObject CreateUiObject(string objectName, Transform parent)
    {
        GameObject uiObject = new GameObject(objectName, typeof(RectTransform));
        uiObject.layer = LayerMask.NameToLayer("UI");
        uiObject.transform.SetParent(parent, false);
        uiObject.transform.localScale = Vector3.one;
        return uiObject;
    }

    static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.pivot = new Vector2(0.5f, 0.5f);
    }

    void AddHoverSound(Button button)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = button.gameObject.AddComponent<EventTrigger>();

        if (trigger.triggers == null)
            trigger.triggers = new System.Collections.Generic.List<EventTrigger.Entry>();

        EventTrigger.Entry hoverEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        hoverEntry.callback.AddListener((data) => PlayHover());
        trigger.triggers.Add(hoverEntry);
    }

    public void PlayHover()
    {
        if (audioSource != null && hoverSound != null)
            audioSource.PlayOneShot(hoverSound, 0.7f);
    }

    public void OpenHowToPlay()
    {
        if (howToPlayPanel == null)
            return;

        PlayClick();
        howToPlayPanel.SetActive(true);

        if (EventSystem.current != null && closeHowToButton != null)
            EventSystem.current.SetSelectedGameObject(closeHowToButton.gameObject);
    }

    public void CloseHowToPlay()
    {
        if (howToPlayPanel == null || !howToPlayPanel.activeSelf)
            return;

        PlayClick();
        howToPlayPanel.SetActive(false);

        if (EventSystem.current != null && howToButton != null)
            EventSystem.current.SetSelectedGameObject(howToButton.gameObject);
    }

    public void StartGame()
    {
        if (isStartingGame)
            return;

        isStartingGame = true;
        PlayClick();

        if (startButton != null)
            startButton.interactable = false;

        Invoke(nameof(LoadScene), 0.2f);
    }

    public void QuitGame()
    {
        PlayClick();
        Invoke(nameof(DoQuit), 0.2f);
    }

    void PlayClick()
    {
        if (audioSource != null && clickSound != null)
            audioSource.PlayOneShot(clickSound, 1f);
    }

    void LoadScene()
    {
        int firstSceneIndex = FindSceneBuildIndex(firstSceneName);
        int loadingSceneIndex = FindSceneBuildIndex(loadingSceneName);

        if (firstSceneIndex < 0)
        {
            isStartingGame = false;
            if (startButton != null)
                startButton.interactable = true;

            Debug.LogError($"ไม่พบด่านเริ่มต้น '{firstSceneName}' ใน Build Settings");
            return;
        }

        if (loadingSceneIndex < 0)
        {
            isStartingGame = false;
            if (startButton != null)
                startButton.interactable = true;

            Debug.LogError($"ไม่พบหน้าโหลด '{loadingSceneName}' ใน Build Settings");
            return;
        }

        PlayerPrefs.SetInt("NextScene", firstSceneIndex);
        PlayerPrefs.Save();
        SceneManager.LoadScene(loadingSceneIndex);
    }

    int FindSceneBuildIndex(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneFileName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if (string.Equals(sceneFileName, sceneName, StringComparison.OrdinalIgnoreCase))
                return i;
        }

        return -1;
    }

    void DoQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
