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
    // Minimal menu uses a clean navy backdrop instead of the old character illustration.
    private const string BackgroundResourcePath = "";
    private const string LevelAtlasResourcePath = "UI/LevelSelect_RoomAtlas";
    private const string BackgroundVideoResourcePath = "UI/MainMenu_AAA_Background_Animated";
    private const bool UseAnimatedBackgroundVideo = false;
    private const string FallbackFontResourcePath = "Fonts & Materials/MiPancake SDF";
    private const string UiFontResourcePath = "Fonts & Materials/MiPancake SDF";
    private const string TitleFontResourcePath = "Fonts & Materials/MiPancake SDF";

    private readonly Color cyan = new Color(0.49f, 0.90f, 1f, 1f);
    private readonly Color warmWhite = new Color(0.94f, 0.99f, 1f, 1f);
    private readonly Color mutedWhite = new Color(0.76f, 0.90f, 0.96f, 1f);
    private readonly Color panelNavy = new Color(0.025f, 0.16f, 0.25f, 0.96f);

    private AudioSource audioSource;
    private TMP_FontAsset uiFont;
    private TMP_FontAsset titleFont;
    private GameObject premiumRoot;
    private GameObject howToPlayPanel;
    private GameObject levelSelectPanel;
    private GameObject settingsPanel;
    private GameObject caseFilesPanel;
    private Button startButton;
    private Button howToButton;
    private Button closeHowToButton;
    private Button closeSettingsButton;
    private Button resetSaveButton;
    private Button closeCaseFilesButton;
    private Button challengeModeButton;
    private bool isStartingGame;
    private bool waitingForResetConfirmation;
    private float nextHoverSoundTime;
    private const float HoverSoundCooldown = 0.18f;
    private readonly Button[] levelButtons = new Button[4];
    private readonly RawImage[] levelThumbnails = new RawImage[4];
    private readonly string[] levelSceneNames = { "1bathroom1", "2Kitchen2", "3iving room3", "4bedroom4" };
    private readonly string[] levelLabels = { "BATHROOM", "KITCHEN", "LIVING ROOM", "BEDROOM" };

    void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        audioSource = GetComponent<AudioSource>();
        LoadFonts();

        AudioClip sharedHoverClip = Resources.Load<AudioClip>("Audio/MenuHover");
        if (sharedHoverClip != null)
            hoverSound = sharedHoverClip;

        if (bgmClip != null && audioSource != null)
        {
            audioSource.clip = bgmClip;
            audioSource.loop = true;
            audioSource.volume = PlayerPrefs.GetFloat("BGMVolume", 0.25f);
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

        if (levelSelectPanel != null && levelSelectPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            CloseLevelSelect();

        if (settingsPanel != null && settingsPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            CloseSettings();

        if (caseFilesPanel != null && caseFilesPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            CloseCaseFiles();
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
            Destroy(existingRoot.gameObject);

        HideLegacyMenuChildren(targetCanvas.transform, null);

        premiumRoot = CreateUiObject(PremiumRootName, targetCanvas.transform);
        Stretch(premiumRoot.GetComponent<RectTransform>());

        BuildBackground(premiumRoot.transform);
        BuildMainPanel(premiumRoot.transform);
        BuildHowToPlayPanel(premiumRoot.transform);
        BuildLevelSelectPanel(premiumRoot.transform);
        BuildSettingsPanel(premiumRoot.transform);
        BuildCaseFilesPanel(premiumRoot.transform);
    }

    void HideLegacyMenuChildren(Transform canvasRoot, Transform keepRoot)
    {
        for (int i = 0; i < canvasRoot.childCount; i++)
        {
            Transform child = canvasRoot.GetChild(i);
            if (child != keepRoot)
                child.gameObject.SetActive(false);
        }
    }

    Canvas FindMainCanvas()
    {
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            if (canvas.gameObject.scene == gameObject.scene &&
                canvas.isRootCanvas && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                return canvas;
        }

        foreach (Canvas canvas in canvases)
            if (canvas.gameObject.scene == gameObject.scene)
                return canvas;

        return canvases.Length > 0 ? canvases[0] : null;
    }

    void LoadFonts()
    {
        if (uiFont != null && titleFont != null) return;

        TMP_FontAsset fallbackFont = Resources.Load<TMP_FontAsset>(FallbackFontResourcePath);
        uiFont = Resources.Load<TMP_FontAsset>(UiFontResourcePath);
        titleFont = Resources.Load<TMP_FontAsset>(TitleFontResourcePath);

        if (uiFont == null)
            uiFont = fallbackFont;

        if (titleFont == null)
            titleFont = uiFont;
    }

    void BuildBackground(Transform parent)
    {
        GameObject backgroundObject = CreateUiObject("CinematicBackground", parent);
        Stretch(backgroundObject.GetComponent<RectTransform>());

        RawImage background = backgroundObject.AddComponent<RawImage>();
        background.raycastTarget = false;
        background.color = Color.white;

        Texture2D backgroundTexture = string.IsNullOrEmpty(BackgroundResourcePath)
            ? null
            : Resources.Load<Texture2D>(BackgroundResourcePath);
        if (backgroundTexture != null)
        {
            background.texture = backgroundTexture;

            AspectRatioFitter fitter = backgroundObject.AddComponent<AspectRatioFitter>();
            fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            fitter.aspectRatio = (float)backgroundTexture.width / backgroundTexture.height;
        }
        else
        {
            background.color = new Color(0.08f, 0.36f, 0.54f, 1f);
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

        MainMenuBackgroundMotion backgroundMotion =
            motionObject.AddComponent<MainMenuBackgroundMotion>();
        backgroundMotion.Configure(background, null);

    }

    void BuildMainPanel(Transform parent)
    {
        GameObject leftPanel = CreateUiObject("CommandPanel", parent);
        RectTransform panelRect = leftPanel.GetComponent<RectTransform>();
        Stretch(panelRect);

        TextMeshProUGUI operationLabel = CreateText(
            leftPanel.transform,
            "OperationLabel",
            "CLEANING OPERATIONS  /  01",
            22f,
            cyan,
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            new Vector2(0f, 245f),
            new Vector2(670f, 42f),
            TextAlignmentOptions.Center,
            FontStyles.Bold,
            3f);
        operationLabel.font = titleFont;

        TextMeshProUGUI title = CreateText(
            leftPanel.transform,
            "GameTitle",
            "CLEAN & LEARN",
            72f,
            warmWhite,
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            new Vector2(0f, 165f),
            new Vector2(900f, 105f),
            TextAlignmentOptions.Center,
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
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            new Vector2(0f, 85f),
            new Vector2(730f, 45f),
            TextAlignmentOptions.Center);

        startButton = CreateMenuButton(
            leftPanel.transform,
            "StartGameButton",
            "START MISSION",
            "01",
            new Vector2(0f, 5f),
            StartGame);

        howToButton = CreateMenuButton(
            leftPanel.transform,
            "HowToPlayButton",
            "HOW TO PLAY",
            "02",
            new Vector2(0f, -68f),
            OpenHowToPlay);

        CreateMenuButton(
            leftPanel.transform,
            "CaseFilesButton",
            "CASE FILES",
            "03",
            new Vector2(0f, -141f),
            OpenCaseFiles);

        CreateMenuButton(
            leftPanel.transform,
            "SettingsButton",
            "SETTINGS",
            "04",
            new Vector2(0f, -214f),
            OpenSettings);

        CreateMenuButton(
            leftPanel.transform,
            "QuitGameButton",
            "QUIT GAME",
            "05",
            new Vector2(0f, -287f),
            QuitGame);

        CreateText(
            leftPanel.transform,
            "FooterStatus",
            "READY FOR DEPLOYMENT",
            17f,
            new Color(mutedWhite.r, mutedWhite.g, mutedWhite.b, 0.68f),
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            new Vector2(14f, -373f),
            new Vector2(460f, 34f),
            TextAlignmentOptions.Center,
            FontStyles.Bold,
            2f);

        GameObject statusDot = CreateUiObject("StatusDot", leftPanel.transform);
        RectTransform dotRect = statusDot.GetComponent<RectTransform>();
        dotRect.anchorMin = dotRect.anchorMax = new Vector2(0.5f, 0.5f);
        dotRect.pivot = new Vector2(0.5f, 0.5f);
        dotRect.anchoredPosition = new Vector2(-204f, -373f);
        dotRect.sizeDelta = new Vector2(10f, 10f);
        Image dotImage = statusDot.AddComponent<Image>();
        dotImage.color = cyan;
        dotImage.raycastTarget = false;

        CreateText(
            parent,
            "BuildLabel",
            "CLEAN & LEARN  •  DEV BUILD",
            16f,
            new Color(0.82f, 0.88f, 0.93f, 0f),
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
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.pivot = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = anchoredPosition;
        buttonRect.sizeDelta = new Vector2(440f, 58f);

        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = new Color(0.025f, 0.17f, 0.27f, 0.88f);

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
            new Vector2(760f, 44f),
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
            new Vector2(800f, 78f),
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
            new Vector2(390f, 54f),
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
            new Vector2(450f, 54f),
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
        missionRect.sizeDelta = new Vector2(-108f, 154f);
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
            new Vector2(24f, 16f),
            new Vector2(-204f, -16f),
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

    void BuildSettingsPanel(Transform parent)
    {
        settingsPanel = CreateUiObject("SettingsModal", parent);
        Stretch(settingsPanel.GetComponent<RectTransform>());
        Image dimmer = settingsPanel.AddComponent<Image>();
        dimmer.color = new Color(0.02f, 0.16f, 0.24f, 0.76f);

        GameObject modal = CreateUiObject("ModalWindow", settingsPanel.transform);
        RectTransform modalRect = modal.GetComponent<RectTransform>();
        modalRect.anchorMin = modalRect.anchorMax = new Vector2(0.5f, 0.5f);
        modalRect.pivot = new Vector2(0.5f, 0.5f);
        modalRect.sizeDelta = new Vector2(650f, 420f);
        Image modalImage = modal.AddComponent<Image>();
        modalImage.color = new Color(0.12f, 0.46f, 0.62f, 0.98f);
        Outline outline = modal.AddComponent<Outline>();
        outline.effectColor = cyan;
        outline.effectDistance = new Vector2(1.2f, -1.2f);

        TextMeshProUGUI title = CreateText(modal.transform, "Title", "SETTINGS", 40f, cyan,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -68f),
            new Vector2(500f, 55f), TextAlignmentOptions.Center, FontStyles.Bold, 4f);
        title.font = titleFont;
        CreateText(modal.transform, "Subtitle", "AUDIO CONTROLS", 16f, mutedWhite,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -118f),
            new Vector2(440f, 28f), TextAlignmentOptions.Center, FontStyles.Bold, 2f).font = titleFont;

        CreateSettingsSlider(modal.transform, "Music", "MUSIC", -18f, "BGMVolume", true);
        CreateSettingsSlider(modal.transform, "Sfx", "SFX", -94f, "SFXVolume", false);
        closeSettingsButton = CreateCompactButton(modal.transform, "Close", "CLOSE", new Vector2(1f, 1f),
            new Vector2(-56f, -36f), new Vector2(108f, 42f), CloseSettings);
        settingsPanel.SetActive(false);
    }

    void CreateSettingsSlider(Transform parent, string objectName, string label, float y, string key, bool affectsBgm)
    {
        CreateText(parent, objectName + "Label", label, 19f, warmWhite,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-230f, y),
            new Vector2(130f, 34f), TextAlignmentOptions.Left, FontStyles.Bold).font = titleFont;

        GameObject sliderObject = CreateUiObject(objectName + "Slider", parent);
        RectTransform rect = sliderObject.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(85f, y);
        rect.sizeDelta = new Vector2(350f, 26f);
        Slider slider = sliderObject.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = PlayerPrefs.GetFloat(key, affectsBgm ? 0.25f : 0.8f);

        GameObject background = CreateUiObject("Background", sliderObject.transform);
        Stretch(background.GetComponent<RectTransform>());
        Image backgroundImage = background.AddComponent<Image>();
        backgroundImage.color = new Color(0.06f, 0.13f, 0.20f, 1f);
        slider.targetGraphic = backgroundImage;

        GameObject fillArea = CreateUiObject("Fill Area", sliderObject.transform);
        Stretch(fillArea.GetComponent<RectTransform>());
        RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
        fillAreaRect.offsetMin = new Vector2(8f, 6f);
        fillAreaRect.offsetMax = new Vector2(-8f, -6f);
        GameObject fill = CreateUiObject("Fill", fillArea.transform);
        Stretch(fill.GetComponent<RectTransform>());
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = cyan;
        slider.fillRect = fill.GetComponent<RectTransform>();

        GameObject handle = CreateUiObject("Handle", sliderObject.transform);
        RectTransform handleRect = handle.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(18f, 36f);
        Image handleImage = handle.AddComponent<Image>();
        handleImage.color = warmWhite;
        slider.handleRect = handleRect;
        slider.direction = Slider.Direction.LeftToRight;
        slider.onValueChanged.AddListener(value =>
        {
            PlayerPrefs.SetFloat(key, value);
            if (affectsBgm && audioSource != null)
                audioSource.volume = value;
        });
    }

    void BuildLevelSelectPanel(Transform parent)
    {
        levelSelectPanel = CreateUiObject("LevelSelectPanel", parent);
        Stretch(levelSelectPanel.GetComponent<RectTransform>());

        Image shade = levelSelectPanel.AddComponent<Image>();
        shade.color = new Color(0.004f, 0.014f, 0.030f, 0.97f);

        CreateText(levelSelectPanel.transform, "LevelSelectTitle", "เลือกด่าน", 58f, cyan,
            new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(78f, -86f),
            new Vector2(500f, 80f), TextAlignmentOptions.Left, FontStyles.Bold).font = uiFont;
        CreateText(levelSelectPanel.transform, "LevelSelectSub", "SELECT A MISSION", 20f, mutedWhite,
            new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(84f, 52f),
            new Vector2(400f, 38f), TextAlignmentOptions.Left, FontStyles.Bold, 3f).font = titleFont;

        for (int i = 0; i < levelButtons.Length; i++)
            levelButtons[i] = CreateLevelCard(levelSelectPanel.transform, i);

        CreateCompactButton(levelSelectPanel.transform, "BackToMenu", "BACK", new Vector2(1f, 0f),
            new Vector2(-90f, 58f), new Vector2(130f, 48f), CloseLevelSelect);

        resetSaveButton = CreateCompactButton(levelSelectPanel.transform, "ResetSaveButton", "RESET SAVE", new Vector2(1f, 0f),
            new Vector2(-116f, 122f), new Vector2(190f, 48f), RequestResetSave);

        challengeModeButton = CreateCompactButton(levelSelectPanel.transform, "ChallengeModeButton", "CHALLENGE LOCKED", new Vector2(0f, 0f),
            new Vector2(142f, 58f), new Vector2(280f, 48f), ToggleChallengeMode);

        levelSelectPanel.SetActive(false);
    }

    void BuildCaseFilesPanel(Transform parent)
    {
        caseFilesPanel = CreateUiObject("CaseFilesPanel", parent);
        Stretch(caseFilesPanel.GetComponent<RectTransform>());
        Image shade = caseFilesPanel.AddComponent<Image>();
        shade.color = new Color(0.004f, 0.014f, 0.030f, 0.90f);

        GameObject window = CreateUiObject("Window", caseFilesPanel.transform);
        RectTransform windowRect = window.GetComponent<RectTransform>();
        windowRect.anchorMin = windowRect.anchorMax = new Vector2(0.5f, 0.5f);
        windowRect.pivot = new Vector2(0.5f, 0.5f);
        windowRect.sizeDelta = new Vector2(920f, 650f);
        Image windowImage = window.AddComponent<Image>();
        windowImage.color = panelNavy;
        Outline windowOutline = window.AddComponent<Outline>();
        windowOutline.effectColor = cyan;
        windowOutline.effectDistance = new Vector2(1.2f, -1.2f);

        CreateText(window.transform, "Eyebrow", "EVIDENCE ARCHIVE  /  RECOVERED CASE FILES", 17f, cyan,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -48f),
            new Vector2(760f, 32f), TextAlignmentOptions.Center, FontStyles.Bold, 2f).font = titleFont;
        CreateText(window.transform, "Title", "CASE FILES", 42f, warmWhite,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -82f),
            new Vector2(720f, 54f), TextAlignmentOptions.Center, FontStyles.Bold).font = titleFont;
        CreateText(window.transform, "Hint", "CLEAR A MISSION TO RECOVER ITS NOTE", 14f, mutedWhite,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -145f),
            new Vector2(720f, 25f), TextAlignmentOptions.Center, FontStyles.Bold, 1.5f);
        closeCaseFilesButton = CreateCompactButton(window.transform, "CloseButton", "BACK", new Vector2(0.5f, 0f),
            new Vector2(0f, 38f), new Vector2(160f, 46f), CloseCaseFiles);

        string[] notes =
        {
            "FIRST TRACE RECOVERED. The system recorded a careful first cleanup.",
            "AFTER HOURS RECOVERED. The kitchen pattern confirms deliberate choices.",
            "QUIET ROOM RECOVERED. The missing record points to a repeated test.",
            "FINAL ROOM RECOVERED. Every case is now connected and closed."
        };
        for (int i = 0; i < 4; i++)
            CreateEvidenceEntry(window.transform, i, notes[i]);

        caseFilesPanel.SetActive(false);
    }

    void CreateEvidenceEntry(Transform parent, int levelIndex, string note)
    {
        GameObject entry = CreateUiObject("Evidence_" + (levelIndex + 1), parent);
        RectTransform rect = entry.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -190f - levelIndex * 82f);
        rect.sizeDelta = new Vector2(770f, 78f);
        Image image = entry.AddComponent<Image>();
        bool recovered = GameProgress.HasEvidence(levelIndex);
        image.color = recovered
            ? new Color(0.035f, 0.20f, 0.29f, 0.96f)
            : new Color(0.025f, 0.07f, 0.11f, 0.96f);

        MissionLevelData level = MissionLevelCatalog.GetByIndex(levelIndex);
        CreateText(entry.transform, "Number", "0" + level.number, 23f, recovered ? cyan : mutedWhite,
            new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(28f, 0f),
            new Vector2(70f, 38f), TextAlignmentOptions.Left, FontStyles.Bold).font = titleFont;
        CreateText(entry.transform, "Name", levelLabels[levelIndex], 18f, warmWhite,
            new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(108f, 16f),
            new Vector2(220f, 30f), TextAlignmentOptions.Left, FontStyles.Bold).font = titleFont;
        CreateText(entry.transform, "Status", recovered ? "EVIDENCE RECOVERED" : "DATA LOCKED", 13f, recovered ? new Color(0.37f, 1f, 0.75f, 1f) : mutedWhite,
            new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(108f, -15f),
            new Vector2(210f, 25f), TextAlignmentOptions.Left, FontStyles.Bold, 1f).font = titleFont;
        CreateText(entry.transform, "Note", recovered ? note : "Complete this mission to recover the hidden case note.", 13f,
            recovered ? new Color(0.78f, 0.91f, 0.97f, 1f) : new Color(0.50f, 0.62f, 0.70f, 1f),
            new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(355f, 0f),
            new Vector2(375f, 52f), TextAlignmentOptions.Left, FontStyles.Normal);
    }

    Button CreateLevelCard(Transform parent, int levelIndex)
    {
        GameObject card = CreateUiObject("MissionCard_" + (levelIndex + 1), parent);
        RectTransform rect = card.GetComponent<RectTransform>();
        float cardWidth = 0.205f;
        float gap = 0.025f;
        float left = 0.06f + levelIndex * (cardWidth + gap);
        rect.anchorMin = new Vector2(left, 0.25f);
        rect.anchorMax = new Vector2(left + cardWidth, 0.73f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image image = card.AddComponent<Image>();
        image.color = new Color(0.025f, 0.095f, 0.15f, 0.97f);
        Outline outline = card.AddComponent<Outline>();
        outline.effectColor = new Color(cyan.r, cyan.g, cyan.b, 0.72f);
        outline.effectDistance = new Vector2(1.4f, -1.4f);

        Button button = card.AddComponent<Button>();
        button.targetGraphic = image;
        int selectedIndex = levelIndex;
        button.onClick.AddListener(() => SelectLevel(selectedIndex));
        MenuButtonHoverMotion hover = card.AddComponent<MenuButtonHoverMotion>();
        hover.Configure(new Vector2(0f, 8f), 1.025f, 14f);

        GameObject thumbnailObject = CreateUiObject("Thumbnail", card.transform);
        RectTransform thumbnailRect = thumbnailObject.GetComponent<RectTransform>();
        thumbnailRect.anchorMin = new Vector2(0f, 0.20f);
        thumbnailRect.anchorMax = new Vector2(1f, 1f);
        thumbnailRect.offsetMin = new Vector2(8f, 0f);
        thumbnailRect.offsetMax = new Vector2(-8f, -8f);
        RawImage thumbnail = thumbnailObject.AddComponent<RawImage>();
        thumbnail.texture = Resources.Load<Texture2D>(LevelAtlasResourcePath);
        thumbnail.uvRect = GetLevelThumbnailUv(levelIndex);
        thumbnail.color = Color.white;
        thumbnail.raycastTarget = false;
        thumbnail.transform.SetAsFirstSibling();
        levelThumbnails[levelIndex] = thumbnail;

        CreateText(card.transform, "Number", "0" + (levelIndex + 1), 39f, cyan,
            new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(24f, 27f),
            new Vector2(88f, 58f), TextAlignmentOptions.Left, FontStyles.Bold).font = titleFont;
        CreateText(card.transform, "Name", levelLabels[levelIndex], 23f, warmWhite,
            new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(104f, 26f),
            new Vector2(-20f, 66f), TextAlignmentOptions.Left, FontStyles.Bold).font = titleFont;
        MissionLevelData level = MissionLevelCatalog.GetByIndex(levelIndex);
        CreateText(card.transform, "Difficulty", level.difficulty + "  //  " + level.challenge, 12f, mutedWhite,
            new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(104f, 3f),
            new Vector2(-20f, 28f), TextAlignmentOptions.Left, FontStyles.Bold, 1f).font = titleFont;
        TextMeshProUGUI state = CreateText(card.transform, "State", "", 20f, cyan,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero,
            new Vector2(240f, 54f), TextAlignmentOptions.Center, FontStyles.Bold);
        state.font = uiFont;

        return button;
    }

    static Rect GetLevelThumbnailUv(int levelIndex)
    {
        switch (levelIndex)
        {
            case 0: return new Rect(0f, 0.5f, 0.5f, 0.5f); // bathroom, top-left
            case 1: return new Rect(0.5f, 0.5f, 0.5f, 0.5f); // kitchen, top-right
            case 2: return new Rect(0f, 0f, 0.5f, 0.5f); // living room, bottom-left
            default: return new Rect(0.5f, 0f, 0.5f, 0.5f); // bedroom, bottom-right
        }
    }

    void OpenLevelSelect()
    {
        if (levelSelectPanel == null) return;
        PlayClick();
        waitingForResetConfirmation = false;
        SetResetSaveLabel("RESET SAVE");
        UpdateLevelCards();
        UpdateChallengeModeButton();
        levelSelectPanel.SetActive(true);
        if (EventSystem.current != null && levelButtons[0] != null)
            EventSystem.current.SetSelectedGameObject(levelButtons[0].gameObject);
    }

    void CloseLevelSelect()
    {
        if (levelSelectPanel == null) return;
        waitingForResetConfirmation = false;
        SetResetSaveLabel("RESET SAVE");
        PlayClick();
        levelSelectPanel.SetActive(false);
        if (EventSystem.current != null && startButton != null)
            EventSystem.current.SetSelectedGameObject(startButton.gameObject);
    }

    void RequestResetSave()
    {
        if (!waitingForResetConfirmation)
        {
            waitingForResetConfirmation = true;
            SetResetSaveLabel("CONFIRM RESET?");
            return;
        }

        GameProgress.ResetProgress();
        waitingForResetConfirmation = false;
        SetResetSaveLabel("SAVE RESET!");
        UpdateLevelCards();
        UpdateChallengeModeButton();
    }

    void ToggleChallengeMode()
    {
        if (!GameProgress.CanUseChallengeMode()) return;
        GameProgress.SetChallengeMode(!GameProgress.IsChallengeMode);
        PlayClick();
        UpdateChallengeModeButton();
        UpdateLevelCards();
    }

    void UpdateChallengeModeButton()
    {
        if (challengeModeButton == null) return;
        bool available = GameProgress.CanUseChallengeMode();
        challengeModeButton.interactable = available;
        TMP_Text label = challengeModeButton.transform.Find("Label")?.GetComponent<TMP_Text>();
        if (label != null)
        {
            label.text = available
                ? (GameProgress.IsChallengeMode ? "CHALLENGE MODE: ON" : "CHALLENGE MODE: OFF")
                : "CHALLENGE LOCKED";
            label.color = available && GameProgress.IsChallengeMode
                ? new Color(1f, 0.67f, 0.34f, 1f)
                : warmWhite;
        }
    }

    void SetResetSaveLabel(string label)
    {
        if (resetSaveButton == null) return;
        TMP_Text text = resetSaveButton.transform.Find("Label")?.GetComponent<TMP_Text>();
        if (text != null) text.text = label;
    }

    void UpdateLevelCards()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            Button button = levelButtons[i];
            if (button == null) continue;
            bool unlocked = GameProgress.IsUnlocked(i);
            bool completed = GameProgress.IsCompleted(i);
            button.interactable = unlocked;
            Image image = button.GetComponent<Image>();
            if (image != null) image.color = unlocked
                ? new Color(0.025f, 0.13f, 0.20f, 0.70f)
                : new Color(0.016f, 0.04f, 0.07f, 0.82f);
            if (levelThumbnails[i] != null)
                levelThumbnails[i].color = unlocked
                    ? new Color(1f, 1f, 1f, 1f)
                    : new Color(0.24f, 0.31f, 0.40f, 0.68f);
            TMP_Text state = button.transform.Find("State").GetComponent<TMP_Text>();
            if (state != null)
            {
                state.text = completed
                    ? "CLEARED\nRANK " + RankLabel(GameProgress.GetBestRank(i))
                    : (unlocked ? "READY" : "LOCKED");
                state.color = completed
                    ? new Color(0.49f, 0.90f, 1f, 1f)
                    : (unlocked ? new Color(0.35f, 1f, 0.73f, 1f) : mutedWhite);
            }
            TMP_Text difficulty = button.transform.Find("Difficulty")?.GetComponent<TMP_Text>();
            if (difficulty != null)
            {
                MissionLevelData level = MissionLevelCatalog.GetByIndex(i);
                difficulty.text = GameProgress.IsChallengeMode
                    ? "CHALLENGE  //  NO ROOM FOR ERROR"
                    : level.difficulty + "  //  " + level.challenge;
                difficulty.color = GameProgress.IsChallengeMode
                    ? new Color(1f, 0.67f, 0.34f, 1f)
                    : mutedWhite;
            }
        }
    }

    static string RankLabel(int rank)
    {
        switch (rank)
        {
            case 3: return "S";
            case 2: return "A";
            default: return "B";
        }
    }

    void OpenCaseFiles()
    {
        if (caseFilesPanel == null) return;
        PlayClick();
        caseFilesPanel.SetActive(true);
        if (EventSystem.current != null && closeCaseFilesButton != null)
            EventSystem.current.SetSelectedGameObject(closeCaseFilesButton.gameObject);
    }

    void CloseCaseFiles()
    {
        if (caseFilesPanel == null || !caseFilesPanel.activeSelf) return;
        PlayClick();
        caseFilesPanel.SetActive(false);
        if (EventSystem.current != null && startButton != null)
            EventSystem.current.SetSelectedGameObject(startButton.gameObject);
    }

    void SelectLevel(int levelIndex)
    {
        if (!GameProgress.IsUnlocked(levelIndex) || isStartingGame) return;
        isStartingGame = true;
        PlayClick();
        int sceneIndex = FindSceneBuildIndex(levelSceneNames[levelIndex]);
        int loadingIndex = FindSceneBuildIndex(loadingSceneName);
        if (sceneIndex < 0 || loadingIndex < 0)
        {
            isStartingGame = false;
            Debug.LogError("Level Select: missing gameplay or loading scene in Build Settings.");
            return;
        }
        PlayerPrefs.SetInt("NextScene", sceneIndex);
        PlayerPrefs.Save();
        SceneManager.LoadScene(loadingIndex);
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
        keyRect.sizeDelta = new Vector2(keyWidth, 52f);

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
            new Vector2(330f, 56f),
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
        text.extraPadding = false;
        text.lineSpacing = content.IndexOf('\n') >= 0 ? 4f : 0f;
        text.textWrappingMode = TextWrappingModes.Normal;
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
        if (audioSource == null || hoverSound == null)
            return;

        // Prevent noisy repeats while the mouse passes across multiple buttons.
        if (Time.unscaledTime < nextHoverSoundTime)
            return;

        nextHoverSoundTime = Time.unscaledTime + HoverSoundCooldown;
        audioSource.PlayOneShot(hoverSound, 0.5f);
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

    public void OpenSettings()
    {
        if (settingsPanel == null) return;
        PlayClick();
        settingsPanel.SetActive(true);
        if (EventSystem.current != null && closeSettingsButton != null)
            EventSystem.current.SetSelectedGameObject(closeSettingsButton.gameObject);
    }

    public void CloseSettings()
    {
        if (settingsPanel == null || !settingsPanel.activeSelf) return;
        PlayerPrefs.Save();
        PlayClick();
        settingsPanel.SetActive(false);
        if (EventSystem.current != null && startButton != null)
            EventSystem.current.SetSelectedGameObject(startButton.gameObject);
    }

    public void OpenCredits()
    {
        PlayClick();
        SceneManager.LoadScene("Credits");
    }

    public void StartGame()
    {
        OpenLevelSelect();
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
