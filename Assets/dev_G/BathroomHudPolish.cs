using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Applies only at runtime in 1bathroom1; the shared UI prefab is never edited.
[DisallowMultipleComponent]
public class BathroomHudPolish : MonoBehaviour
{
    static readonly Color Cyan = new Color(0.20f, 0.82f, 1f, 1f);
    static readonly Color Panel = new Color(0.015f, 0.075f, 0.13f, 0.90f);
    static readonly Color SlotNormal = new Color(0.02f, 0.10f, 0.17f, 0.90f);
    static readonly Color SlotSelected = new Color(0.035f, 0.24f, 0.36f, 0.98f);

    Sprite roundedSprite;
    Texture2D roundedTexture;

    void Start()
    {
        roundedSprite = CreateRoundedSprite();
        StyleObjectivePanel();
        StyleInventory();
    }

    void StyleObjectivePanel()
    {
        ObjectivePanelUI objectives = GetComponent<ObjectivePanelUI>();
        if (objectives == null) return;

        foreach (Image oldImage in GetComponentsInChildren<Image>(true))
            oldImage.enabled = false;

        Image background = CreateImage("HudBackground", transform, Panel);
        Stretch(background.rectTransform, -14f);
        background.sprite = roundedSprite;
        background.type = Image.Type.Sliced;
        background.transform.SetAsFirstSibling();

        Image accent = CreateImage("HudLeftAccent", transform, Cyan);
        RectTransform accentRect = accent.rectTransform;
        accentRect.anchorMin = new Vector2(0f, 0f);
        accentRect.anchorMax = new Vector2(0f, 1f);
        accentRect.pivot = new Vector2(0f, 0.5f);
        accentRect.anchoredPosition = new Vector2(12f, 0f);
        accentRect.sizeDelta = new Vector2(5f, -28f);

        Image topRule = CreateImage("HudTopRule", transform, new Color(0.20f, 0.82f, 1f, 0.38f));
        RectTransform ruleRect = topRule.rectTransform;
        ruleRect.anchorMin = new Vector2(0f, 1f);
        ruleRect.anchorMax = new Vector2(1f, 1f);
        ruleRect.pivot = new Vector2(0.5f, 1f);
        ruleRect.anchoredPosition = new Vector2(0f, -16f);
        ruleRect.sizeDelta = new Vector2(-46f, 2f);

        objectives.titleColor = Cyan;
        objectives.normalColor = new Color(0.90f, 0.96f, 1f, 1f);
        objectives.lockedColor = new Color(1f, 0.73f, 0.30f, 1f);
        objectives.readyColor = new Color(0.35f, 1f, 0.73f, 1f);

        LayoutObjectiveText(objectives.titleText, new Vector2(36f, -25f), new Vector2(330f, 34f));
        LayoutObjectiveText(objectives.inspectObjectiveText, new Vector2(36f, -78f), new Vector2(360f, 32f));
        LayoutObjectiveText(objectives.cleanObjectiveText, new Vector2(36f, -120f), new Vector2(360f, 32f));
        LayoutObjectiveText(objectives.unlockObjectiveText, new Vector2(36f, -170f), new Vector2(370f, 30f));

        StyleText(objectives.titleText, 21f, Cyan);
        StyleText(objectives.inspectObjectiveText, 19f, objectives.normalColor);
        StyleText(objectives.cleanObjectiveText, 19f, objectives.normalColor);
        StyleText(objectives.unlockObjectiveText, 16f, objectives.lockedColor);
    }

    void StyleInventory()
    {
        InventoryUI inventory = FindFirstObjectByType<InventoryUI>();
        if (inventory == null || inventory.slotBackgrounds == null) return;

        inventory.normalColor = SlotNormal;
        inventory.selectedColor = SlotSelected;
        inventory.normalScale = Vector3.one;
        inventory.selectedScale = new Vector3(1.075f, 1.075f, 1f);

        for (int i = 0; i < inventory.slotBackgrounds.Length; i++)
        {
            Image slot = inventory.slotBackgrounds[i];
            if (slot == null) continue;

            slot.enabled = true;
            slot.sprite = roundedSprite;
            slot.type = Image.Type.Sliced;
            slot.raycastTarget = false;

            Outline outline = slot.GetComponent<Outline>();
            if (outline == null) outline = slot.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.20f, 0.78f, 1f, 0.42f);
            outline.effectDistance = new Vector2(1.2f, -1.2f);

            BathroomHudSlotGlow glow = slot.GetComponent<BathroomHudSlotGlow>();
            if (glow == null) glow = slot.gameObject.AddComponent<BathroomHudSlotGlow>();
            glow.Configure(outline);

            CreateSlotNumber(slot.transform, i + 1);
            if (inventory.slotUseTexts != null && i < inventory.slotUseTexts.Length)
                StyleText(inventory.slotUseTexts[i], 16f, Cyan);
        }

        ArrangeSlots(inventory);
        StyleUsesCounter(inventory);
        inventory.SetSelectedSlot(0);
    }

    void CreateSlotNumber(Transform slot, int number)
    {
        if (slot.Find("HudSlotNumber") != null) return;
        GameObject numberObject = new GameObject("HudSlotNumber", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        numberObject.layer = LayerMask.NameToLayer("UI");
        numberObject.transform.SetParent(slot, false);
        RectTransform rect = numberObject.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(0f, 9f);
        rect.sizeDelta = new Vector2(34f, 26f);
        TextMeshProUGUI text = numberObject.GetComponent<TextMeshProUGUI>();
        text.font = Resources.Load<TMP_FontAsset>("UI/Fonts/ChakraPetch-Bold SDF");
        text.text = number.ToString();
        text.fontSize = 17f;
        text.fontStyle = FontStyles.Bold;
        text.color = Cyan;
        text.alignment = TextAlignmentOptions.Center;
        text.raycastTarget = false;
    }

    static void StyleText(TMP_Text text, float size, Color color)
    {
        if (text == null) return;
        text.fontSize = size;
        text.fontStyle = FontStyles.Bold;
        text.color = color;
        text.outlineWidth = 0f;
    }

    static void LayoutObjectiveText(TMP_Text text, Vector2 position, Vector2 size)
    {
        if (text == null) return;
        RectTransform rect = text.rectTransform;
        rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        text.alignment = TextAlignmentOptions.Left;
        text.enableWordWrapping = false;
        text.overflowMode = TextOverflowModes.Overflow;
    }

    static void ArrangeSlots(InventoryUI inventory)
    {
        if (inventory.slotBackgrounds.Length == 0) return;

        float width = inventory.slotBackgrounds[0].rectTransform.rect.width;
        float averageY = 0f;
        for (int i = 0; i < inventory.slotBackgrounds.Length; i++)
            averageY += inventory.slotBackgrounds[i].rectTransform.anchoredPosition.y;
        averageY /= inventory.slotBackgrounds.Length;

        const float gap = 18f;
        float totalWidth = inventory.slotBackgrounds.Length * width +
                           (inventory.slotBackgrounds.Length - 1) * gap;
        float startX = -totalWidth * 0.5f + width * 0.5f;

        for (int i = 0; i < inventory.slotBackgrounds.Length; i++)
        {
            RectTransform rect = inventory.slotBackgrounds[i].rectTransform;
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(startX + i * (width + gap), averageY);
        }
    }

    void StyleUsesCounter(InventoryUI inventory)
    {
        PlayerItemSystem playerItems = FindFirstObjectByType<PlayerItemSystem>();
        if (playerItems == null || playerItems.usesText == null) return;

        Transform existing = inventory.transform.Find("UsesBadge");
        GameObject badgeObject;
        if (existing == null)
        {
            badgeObject = new GameObject("UsesBadge", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            badgeObject.layer = LayerMask.NameToLayer("UI");
            badgeObject.transform.SetParent(inventory.transform, false);
            Image badgeImage = badgeObject.GetComponent<Image>();
            badgeImage.sprite = roundedSprite;
            badgeImage.type = Image.Type.Sliced;
            badgeImage.color = new Color(0.015f, 0.10f, 0.17f, 0.96f);
            badgeImage.raycastTarget = false;
        }
        else
        {
            badgeObject = existing.gameObject;
        }

        RectTransform badgeRect = badgeObject.GetComponent<RectTransform>();
        badgeRect.anchorMin = badgeRect.anchorMax = new Vector2(1f, 1f);
        badgeRect.pivot = new Vector2(1f, 0f);
        badgeRect.anchoredPosition = new Vector2(18f, 13f);
        badgeRect.sizeDelta = new Vector2(150f, 34f);

        RectTransform usesRect = playerItems.usesText.rectTransform;
        usesRect.SetParent(badgeObject.transform, false);
        Stretch(usesRect, 0f);
        playerItems.usesText.alignment = TextAlignmentOptions.Center;
        playerItems.usesText.enableWordWrapping = false;
        StyleText(playerItems.usesText, 16f, Cyan);
    }

    static Image CreateImage(string objectName, Transform parent, Color color)
    {
        GameObject imageObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        imageObject.layer = LayerMask.NameToLayer("UI");
        imageObject.transform.SetParent(parent, false);
        Image image = imageObject.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    static void Stretch(RectTransform rect, float inset)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(inset, inset);
        rect.offsetMax = new Vector2(-inset, -inset);
    }

    Sprite CreateRoundedSprite()
    {
        const int size = 64;
        const float radius = 15f;
        roundedTexture = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            name = "Bathroom HUD Rounded Panel",
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float dx = Mathf.Max(radius - x, 0f, x - (size - 1f - radius));
            float dy = Mathf.Max(radius - y, 0f, y - (size - 1f - radius));
            float alpha = Mathf.Clamp01((radius - Mathf.Sqrt(dx * dx + dy * dy)) + 1f);
            roundedTexture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
        }
        roundedTexture.Apply(false, true);
        Sprite sprite = Sprite.Create(roundedTexture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, new Vector4(radius, radius, radius, radius));
        sprite.name = "Bathroom HUD Rounded Sprite";
        return sprite;
    }

    void OnDestroy()
    {
        if (roundedSprite != null) Destroy(roundedSprite);
        if (roundedTexture != null) Destroy(roundedTexture);
    }
}

public class BathroomHudSlotGlow : MonoBehaviour
{
    Outline outline;
    public void Configure(Outline targetOutline) => outline = targetOutline;
    void Update()
    {
        if (outline == null) return;
        bool selected = transform.localScale.x > 1.02f;
        outline.effectColor = selected ? new Color(0.28f, 0.92f, 1f, 1f) : new Color(0.20f, 0.78f, 1f, 0.42f);
        outline.effectDistance = selected ? new Vector2(2.2f, -2.2f) : new Vector2(1.2f, -1.2f);
    }
}
