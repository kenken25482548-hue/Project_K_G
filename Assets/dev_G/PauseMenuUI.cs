using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PauseMenuUI : MonoBehaviour
{
    public GameObject pauseOverlay;
    private bool isPaused = false;
    private TMP_FontAsset uiFont;
    private TMP_FontAsset guideFont;
    private GameObject pauseGuidePanel;
    private GameObject themedPausePanel;

    private readonly Color cyan = new Color(0.22f, 0.83f, 1f, 1f);
    private readonly Color navy = new Color(0.015f, 0.10f, 0.15f, 0.97f);
    private readonly Color white = new Color(0.92f, 0.97f, 1f, 1f);

    // Other gameplay scripts use this to ignore input while the pause overlay is open.
    public static bool IsPaused { get; private set; }

    void Start()
    {
        isPaused = false;
        IsPaused = false;
        Time.timeScale = 1f;

        if (pauseOverlay != null)
        {
            BuildThemedPauseMenu();
            pauseOverlay.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        if (isPaused && pauseGuidePanel != null && pauseGuidePanel.activeSelf)
            ClosePauseGuide();
        else
            TogglePause();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        IsPaused = isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        if (pauseOverlay != null)
            pauseOverlay.SetActive(isPaused);

        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        IsPaused = false;
        Time.timeScale = 1f;

        if (pauseOverlay != null)
            pauseOverlay.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RestartLevel()
    {
        isPaused = false;
        IsPaused = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        ResetPauseState();
        SceneManager.LoadScene("0Mainmenu0");
    }

    private void ResetPauseState()
    {
        isPaused = false;
        IsPaused = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Builds a single visual style in every gameplay scene, even if the old pause UI differs.
    private void BuildThemedPauseMenu()
    {
        LoadThaiPauseFont();

        Transform existingPanel = pauseOverlay.transform.Find("ThemedPausePanel");
        if (existingPanel != null)
        {
            themedPausePanel = existingPanel.gameObject;
            ApplyThaiFont(existingPanel);
            BuildPauseGuide();
            return;
        }

        for (int i = 0; i < pauseOverlay.transform.childCount; i++)
            pauseOverlay.transform.GetChild(i).gameObject.SetActive(false);

        Image overlayImage = pauseOverlay.GetComponent<Image>();
        if (overlayImage == null) overlayImage = pauseOverlay.AddComponent<Image>();
        overlayImage.color = new Color(0.002f, 0.012f, 0.025f, 0.80f);

        GameObject panel = UiObject("ThemedPausePanel", pauseOverlay.transform);
        themedPausePanel = panel;
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(660f, 660f);
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = navy;
        Outline outline = panel.AddComponent<Outline>();
        outline.effectColor = new Color(cyan.r, cyan.g, cyan.b, 0.78f);
        outline.effectDistance = new Vector2(2f, -2f);

        Accent(panel.transform, new Vector2(0f, 1f), new Vector2(1f, 1f), Vector2.zero, new Vector2(0f, 5f));
        Text(panel.transform, "Eyebrow", "SYSTEM MENU  /  PAUSED", 17f, cyan, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -47f), new Vector2(520f, 34f), TextAlignmentOptions.Center, FontStyles.Bold);
        Text(panel.transform, "Title", "GAME PAUSED", 42f, white, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -94f), new Vector2(560f, 60f), TextAlignmentOptions.Center, FontStyles.Bold);
        Text(panel.transform, "Hint", "PRESS ESC TO RESUME", 18f, new Color(0.66f, 0.77f, 0.85f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -137f), new Vector2(450f, 28f), TextAlignmentOptions.Center);

        ThemedButton(panel.transform, "Resume", "RESUME", new Vector2(0f, 94f), ResumeGame, true);
        ThemedButton(panel.transform, "Restart", "RESTART LEVEL", new Vector2(0f, 29f), RestartLevel, false);
        ThemedButton(panel.transform, "HowToPlay", "HOW TO PLAY", new Vector2(0f, -36f), OpenPauseGuide, false);
        ThemedButton(panel.transform, "MainMenu", "RETURN TO MAIN MENU", new Vector2(0f, -101f), ReturnToMainMenu, false);

        Text(panel.transform, "AudioLabel", "AUDIO SETTINGS", 16f, cyan, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 133f), new Vector2(450f, 30f), TextAlignmentOptions.Center, FontStyles.Bold, 2f);
        CreateSliderRow(panel.transform, "MUSIC", new Vector2(0f, 88f), true);
        CreateSliderRow(panel.transform, "SFX", new Vector2(0f, 48f), false);
        BuildPauseGuide();
    }

    public void OpenPauseGuide()
    {
        if (pauseGuidePanel == null) return;
        pauseGuidePanel.SetActive(true);
        if (themedPausePanel != null)
            themedPausePanel.SetActive(false);
    }

    public void ClosePauseGuide()
    {
        if (pauseGuidePanel != null)
            pauseGuidePanel.SetActive(false);
        if (themedPausePanel != null)
            themedPausePanel.SetActive(true);
    }

    private void BuildPauseGuide()
    {
        if (pauseOverlay == null || pauseGuidePanel != null) return;

        guideFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/MiPancake SDF");
        if (guideFont == null) guideFont = uiFont;

        pauseGuidePanel = UiObject("PauseQuickGuide", pauseOverlay.transform);
        RectTransform guideRect = pauseGuidePanel.GetComponent<RectTransform>();
        guideRect.anchorMin = guideRect.anchorMax = new Vector2(0.5f, 0.5f);
        guideRect.pivot = new Vector2(0.5f, 0.5f);
        guideRect.sizeDelta = new Vector2(720f, 530f);
        Image guideImage = pauseGuidePanel.AddComponent<Image>();
        guideImage.color = navy;
        Outline outline = pauseGuidePanel.AddComponent<Outline>();
        outline.effectColor = new Color(cyan.r, cyan.g, cyan.b, 0.82f);
        outline.effectDistance = new Vector2(2f, -2f);

        Accent(pauseGuidePanel.transform, new Vector2(0f, 1f), new Vector2(1f, 1f), Vector2.zero, new Vector2(0f, 5f));
        GuideText("Eyebrow", "QUICK GUIDE  /  วิธีเล่นแบบย่อ", 18f, cyan, new Vector2(0f, 197f), new Vector2(580f, 32f), TextAlignmentOptions.Center);
        GuideText("Title", "วิธีการเล่น", 38f, white, new Vector2(0f, 145f), new Vector2(580f, 58f), TextAlignmentOptions.Center);
        GuideText("Move", "เดินและเคลื่อนที่:  W A S D", 25f, white, new Vector2(0f, 70f), new Vector2(590f, 42f), TextAlignmentOptions.Left);
        GuideText("Inspect", "สำรวจคราบ/ดูข้อมูล:  E", 25f, white, new Vector2(0f, 18f), new Vector2(590f, 42f), TextAlignmentOptions.Left);
        GuideText("Use", "หยิบไอเทม/ใช้งาน:  F", 25f, white, new Vector2(0f, -34f), new Vector2(590f, 42f), TextAlignmentOptions.Left);
        GuideText("Select", "เลือกช่องไอเทม:  1 - 5", 25f, white, new Vector2(0f, -86f), new Vector2(590f, 42f), TextAlignmentOptions.Left);
        GuideText("Goal", "ตรวจสอบคราบให้ครบก่อน แล้วเลือกไอเทมให้ถูก\nใช้ไอเทมผิดจะเสียจำนวนการใช้ 1 ครั้ง", 22f, new Color(0.76f, 0.90f, 0.96f, 1f), new Vector2(0f, -151f), new Vector2(590f, 68f), TextAlignmentOptions.Center);
        ThemedButton(pauseGuidePanel.transform, "Back", "BACK TO PAUSE MENU", new Vector2(0f, -215f), ClosePauseGuide, true);
        pauseGuidePanel.SetActive(false);
    }

    private void GuideText(string name, string content, float size, Color color, Vector2 position, Vector2 dimensions, TextAlignmentOptions alignment)
    {
        GameObject item = UiObject(name, pauseGuidePanel.transform);
        RectTransform rect = item.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = dimensions;
        TextMeshProUGUI text = item.AddComponent<TextMeshProUGUI>();
        text.font = guideFont != null ? guideFont : TMP_Settings.defaultFontAsset;
        text.text = content;
        text.fontSize = size;
        text.fontStyle = FontStyles.Bold;
        text.color = color;
        text.alignment = alignment;
        text.lineSpacing = content.IndexOf('\n') >= 0 ? 4f : 0f;
        text.enableWordWrapping = true;
        text.raycastTarget = false;
    }

    private void LoadThaiPauseFont()
    {
        uiFont = Resources.Load<TMP_FontAsset>("UI/Fonts/ChakraPetch-SemiBold SDF");
        if (uiFont == null)
        {
            Font tahoma = Font.CreateDynamicFontFromOSFont("Tahoma", 90);
            if (tahoma != null)
                uiFont = TMP_FontAsset.CreateFontAsset(tahoma);
        }
        if (uiFont == null)
            uiFont = Resources.Load<TMP_FontAsset>("UI/Fonts/Kanit-SemiBold SDF");
    }

    private void ApplyThaiFont(Transform panel)
    {
        TextMeshProUGUI[] labels = panel.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (TextMeshProUGUI label in labels)
        {
            label.font = uiFont;
            label.fontStyle = FontStyles.Normal;
            label.characterSpacing = 0f;
        }
    }

    private void CreateSliderRow(Transform parent, string label, Vector2 position, bool isBgm)
    {
        Text(parent, label + "Label", label, 20f, white, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-190f, position.y), new Vector2(100f, 30f), TextAlignmentOptions.Left, FontStyles.Bold);
        GameObject sliderObject = UiObject(label + "Slider", parent);
        RectTransform rect = sliderObject.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(45f, position.y + 1f);
        rect.sizeDelta = new Vector2(290f, 22f);
        Slider slider = sliderObject.AddComponent<Slider>();
        slider.minValue = 0f; slider.maxValue = 1f;
        slider.value = GameSFXManager.Instance == null ? 1f : (isBgm ? GameSFXManager.Instance.GetBGMVolume() : GameSFXManager.Instance.GetSFXVolume());
        slider.onValueChanged.AddListener(value =>
        {
            if (GameSFXManager.Instance == null) return;
            if (isBgm) GameSFXManager.Instance.SetBGMVolume(value);
            else GameSFXManager.Instance.SetSFXVolume(value);
        });

        Image background = sliderObject.AddComponent<Image>();
        background.color = new Color(0.08f, 0.18f, 0.25f, 1f);
        GameObject fill = UiObject("Fill", sliderObject.transform);
        Image fillImage = fill.AddComponent<Image>(); fillImage.color = cyan;
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero; fillRect.anchorMax = Vector2.one; fillRect.offsetMin = new Vector2(3f, 3f); fillRect.offsetMax = new Vector2(-3f, -3f);
        GameObject handle = UiObject("Handle", sliderObject.transform);
        Image handleImage = handle.AddComponent<Image>(); handleImage.color = white;
        RectTransform handleRect = handle.GetComponent<RectTransform>(); handleRect.sizeDelta = new Vector2(20f, 28f);
        slider.fillRect = fillRect; slider.handleRect = handleRect; slider.targetGraphic = handleImage;
    }

    private void ThemedButton(Transform parent, string name, string label, Vector2 position, UnityEngine.Events.UnityAction action, bool primary)
    {
        GameObject buttonObject = UiObject(name, parent);
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position; rect.sizeDelta = new Vector2(450f, 54f);
        Image image = buttonObject.AddComponent<Image>();
        image.color = primary ? new Color(0.03f, 0.31f, 0.43f, 1f) : new Color(0.025f, 0.15f, 0.22f, 1f);
        Button button = buttonObject.AddComponent<Button>(); button.targetGraphic = image; button.onClick.AddListener(action);
        ColorBlock colors = button.colors; colors.normalColor = Color.white; colors.highlightedColor = new Color(0.48f, 0.86f, 1f, 1f); colors.pressedColor = new Color(0.20f, 0.55f, 0.72f, 1f); colors.colorMultiplier = 1f; button.colors = colors;
        Accent(buttonObject.transform, Vector2.zero, new Vector2(0f, 1f), Vector2.zero, new Vector2(4f, 0f));
        Text(buttonObject.transform, "Label", label, 25f, white, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, TextAlignmentOptions.Center, FontStyles.Bold);
        MenuButtonHoverMotion hover = buttonObject.AddComponent<MenuButtonHoverMotion>(); hover.Configure(new Vector2(8f, 0f), 1.02f, 14f);
    }

    private void Accent(Transform parent, Vector2 min, Vector2 max, Vector2 position, Vector2 size)
    {
        GameObject accent = UiObject("Accent", parent); RectTransform rect = accent.GetComponent<RectTransform>();
        rect.anchorMin = min; rect.anchorMax = max; rect.pivot = new Vector2(0f, 0.5f); rect.anchoredPosition = position; rect.sizeDelta = size;
        Image image = accent.AddComponent<Image>(); image.color = cyan; image.raycastTarget = false;
    }

    private TextMeshProUGUI Text(Transform parent, string name, string content, float size, Color color, Vector2 min, Vector2 max, Vector2 position, Vector2 dimensions, TextAlignmentOptions alignment, FontStyles style = FontStyles.Normal, float spacing = 0f)
    {
        GameObject item = UiObject(name, parent); RectTransform rect = item.GetComponent<RectTransform>();
        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.pivot = new Vector2(0.5f, 0.5f);
        if (min == max)
        {
            rect.anchoredPosition = position;
            rect.sizeDelta = dimensions;
        }
        else
        {
            rect.offsetMin = position;
            rect.offsetMax = dimensions;
        }
        TextMeshProUGUI text = item.AddComponent<TextMeshProUGUI>(); text.font = uiFont; text.text = content; text.fontSize = size; text.color = color; text.fontStyle = FontStyles.Normal; text.alignment = alignment; text.characterSpacing = spacing; text.raycastTarget = false; return text;
    }

    private GameObject UiObject(string name, Transform parent)
    {
        GameObject item = new GameObject(name, typeof(RectTransform)); item.layer = LayerMask.NameToLayer("UI"); item.transform.SetParent(parent, false); return item;
    }
}
