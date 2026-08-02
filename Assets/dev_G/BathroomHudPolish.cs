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
        StyleInteractionUI();
        StyleRemainingPanels();
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

        LayoutObjectiveText(objectives.titleText, new Vector2(36f, -25f), new Vector2(340f, 38f));
        LayoutObjectiveText(objectives.inspectObjectiveText, new Vector2(36f, -80f), new Vector2(370f, 38f));
        LayoutObjectiveText(objectives.cleanObjectiveText, new Vector2(36f, -126f), new Vector2(370f, 38f));
        LayoutObjectiveText(objectives.unlockObjectiveText, new Vector2(36f, -177f), new Vector2(380f, 34f));

        StyleText(objectives.titleText, 26f, Cyan);
        StyleText(objectives.inspectObjectiveText, 23f, objectives.normalColor);
        StyleText(objectives.cleanObjectiveText, 23f, objectives.normalColor);
        StyleText(objectives.unlockObjectiveText, 19f, objectives.lockedColor);
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

            RectTransform slotRoot = slot.rectTransform.parent as RectTransform;
            if (slotRoot != null)
                CreateSlotNumber(slotRoot, i + 1);
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

        RectTransform firstRoot = inventory.slotBackgrounds[0].rectTransform.parent as RectTransform;
        if (firstRoot == null) return;

        float width = firstRoot.rect.width;
        float averageY = 0f;
        for (int i = 0; i < inventory.slotBackgrounds.Length; i++)
        {
            RectTransform root = inventory.slotBackgrounds[i].rectTransform.parent as RectTransform;
            if (root == null || root.parent != firstRoot.parent) return;
            averageY += root.anchoredPosition.y;
        }
        averageY /= inventory.slotBackgrounds.Length;

        const float gap = 8f;
        float totalWidth = inventory.slotBackgrounds.Length * width +
                           (inventory.slotBackgrounds.Length - 1) * gap;
        float startX = -totalWidth * 0.5f + width * 0.5f;

        for (int i = 0; i < inventory.slotBackgrounds.Length; i++)
        {
            RectTransform slotRoot = inventory.slotBackgrounds[i].rectTransform.parent as RectTransform;
            slotRoot.anchorMin = slotRoot.anchorMax = new Vector2(0.5f, 0.5f);
            slotRoot.pivot = new Vector2(0.5f, 0.5f);
            // Lower only the inventory row, leaving the original interaction prompt intact.
            slotRoot.anchoredPosition = new Vector2(startX + i * (width + gap), averageY - 55f);

            // The selected background is a child of Slot_#; keep it centered so the icon
            // and the new frame always travel together with the slot root.
            RectTransform backgroundRect = inventory.slotBackgrounds[i].rectTransform;
            backgroundRect.anchorMin = backgroundRect.anchorMax = new Vector2(0.5f, 0.5f);
            backgroundRect.pivot = new Vector2(0.5f, 0.5f);
            backgroundRect.anchoredPosition = Vector2.zero;
            backgroundRect.sizeDelta = new Vector2(width, slotRoot.rect.height);
        }
    }

    void StyleUsesCounter(InventoryUI inventory)
    {
        PlayerItemSystem playerItems = FindFirstObjectByType<PlayerItemSystem>();
        if (playerItems == null || playerItems.usesText == null) return;

        // The original text lives outside InventoryBar and was the old overlapping label.
        // Keep it as a data source for PlayerItemSystem, but hide its legacy visual.
        playerItems.usesText.enabled = false;

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
        RectTransform lastSlotRoot = inventory.slotBackgrounds[inventory.slotBackgrounds.Length - 1].rectTransform.parent as RectTransform;
        if (lastSlotRoot != null && lastSlotRoot.parent != null)
        {
            // The remaining-use badge belongs at the end of the item row, not above it.
            badgeRect.SetParent(lastSlotRoot.parent, false);
            badgeRect.anchorMin = badgeRect.anchorMax = new Vector2(0.5f, 0.5f);
            badgeRect.pivot = new Vector2(0.5f, 0.5f);
            badgeRect.sizeDelta = new Vector2(178f, 58f);
            float gapAfterLastSlot = 12f;
            badgeRect.anchoredPosition = lastSlotRoot.anchoredPosition +
                                         new Vector2(lastSlotRoot.rect.width * 0.5f + gapAfterLastSlot + badgeRect.sizeDelta.x * 0.5f, 0f);
        }

        Transform newTextTransform = badgeObject.transform.Find("UsesText");
        TextMeshProUGUI newText;
        if (newTextTransform == null)
        {
            GameObject textObject = new GameObject(
                "UsesText",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(TextMeshProUGUI));
            textObject.layer = LayerMask.NameToLayer("UI");
            textObject.transform.SetParent(badgeObject.transform, false);
            newText = textObject.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            newText = newTextTransform.GetComponent<TextMeshProUGUI>();
        }

        Stretch(newText.rectTransform, 0f);
        newText.alignment = TextAlignmentOptions.Center;
        newText.enableWordWrapping = false;
        // Reuse the font already configured by the user on the legacy count text.
        newText.font = playerItems.usesText.font;
        StyleText(newText, 22f, Cyan);

        BathroomHudUsesDisplay display = badgeObject.GetComponent<BathroomHudUsesDisplay>();
        if (display == null) display = badgeObject.AddComponent<BathroomHudUsesDisplay>();
        display.Configure(playerItems.usesText, newText);

        CreateItemInfoBadge(badgeRect, playerItems.usesText.font);
    }

    void CreateItemInfoBadge(RectTransform usesBadgeRect, TMP_FontAsset font)
    {
        if (usesBadgeRect == null) return;

        Transform existing = usesBadgeRect.parent.Find("ItemInfoBadge");
        GameObject badgeObject;
        if (existing == null)
        {
            badgeObject = new GameObject("ItemInfoBadge", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            badgeObject.layer = LayerMask.NameToLayer("UI");
            badgeObject.transform.SetParent(usesBadgeRect.parent, false);
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
        badgeRect.anchorMin = badgeRect.anchorMax = new Vector2(0.5f, 0.5f);
        badgeRect.pivot = new Vector2(0.5f, 0.5f);
        badgeRect.sizeDelta = new Vector2(148f, 58f);
        badgeRect.anchoredPosition = usesBadgeRect.anchoredPosition +
                                     new Vector2(usesBadgeRect.sizeDelta.x * 0.5f + 10f + badgeRect.sizeDelta.x * 0.5f, 0f);

        Transform textTransform = badgeObject.transform.Find("InfoText");
        TextMeshProUGUI text;
        if (textTransform == null)
        {
            GameObject textObject = new GameObject("InfoText", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            textObject.layer = LayerMask.NameToLayer("UI");
            textObject.transform.SetParent(badgeObject.transform, false);
            text = textObject.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            text = textTransform.GetComponent<TextMeshProUGUI>();
        }

        Stretch(text.rectTransform, 0f);
        text.font = font;
        text.text = "[Q]  INFO";
        text.alignment = TextAlignmentOptions.Center;
        text.enableWordWrapping = false;
        StyleText(text, 18f, Cyan);
    }

    void StyleInteractionUI()
    {
        PlayerItemSystem playerItems = FindFirstObjectByType<PlayerItemSystem>();
        if (playerItems == null || playerItems.interactUI == null) return;
        ThemePanel(playerItems.interactUI, "HudInteractAccent");

        RectTransform interactRect = playerItems.interactUI.GetComponent<RectTransform>();
        if (interactRect != null)
            interactRect.anchoredPosition += new Vector2(0f, 40f);

        if (playerItems.interactText != null)
        {
            // Keep the font selected in the prefab, just match its color to the new HUD.
            playerItems.interactText.fontSize = 34f;
            playerItems.interactText.color = new Color(0.90f, 0.96f, 1f, 1f);
            playerItems.interactText.outlineWidth = 0f;
        }
    }

    void StyleRemainingPanels()
    {
        PlayerItemSystem items = FindFirstObjectByType<PlayerItemSystem>();
        if (items != null)
        {
            bool isKitchenLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "2Kitchen2";
            bool isLivingRoomLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "3iving room3";
            ThemePanel(items.infoPanel, "HudItemInfoAccent");
            ThemePanel(items.stainInfoPanel, "HudStainInfoAccent");

            // Keep the complete item card at its original centred prefab position.
            // Only the description/remaining-use text is offset inside the card.
            if (isLivingRoomLevel)
                LayoutLivingRoomItemInfo(items);
            else
            {
                RectTransform itemDescriptionRect = items.infoText != null
                    ? items.infoText.rectTransform
                    : null;
                if (itemDescriptionRect != null)
                    itemDescriptionRect.anchoredPosition += new Vector2(80f, 0f);
            }

            StyleText(items.itemNameText, isKitchenLevel ? 28f : 32f, Cyan);
            StyleText(items.infoText, isLivingRoomLevel ? 24f : 27f, new Color(0.90f, 0.96f, 1f, 1f));
            StyleText(items.stainNameText, 32f, Cyan);
            StyleText(items.stainDescriptionText, 27f, new Color(0.90f, 0.96f, 1f, 1f));
            StyleText(items.stainStateText, 25f, new Color(0.35f, 1f, 0.73f, 1f));
        }

        CleaningTarget[] stains = FindObjectsOfType<CleaningTarget>(true);
        for (int i = 0; i < stains.Length; i++)
        {
            if (stains[i] == null) continue;
            ThemePanel(stains[i].wrongPopup, "HudWrongItemAccent");
            StyleText(stains[i].wrongPopupText, 28f, new Color(0.90f, 0.96f, 1f, 1f));
        }

        LevelFlowManager[] flows = FindObjectsOfType<LevelFlowManager>(true);
        for (int i = 0; i < flows.Length; i++)
        {
            if (flows[i] == null) continue;
            ThemePanel(flows[i].levelCompletePanel, "HudCompleteAccent");
            ThemePanel(flows[i].levelFailPanel, "HudFailAccent");
            StyleAllPanelText(flows[i].levelCompletePanel, new Color(0.35f, 1f, 0.73f, 1f));
            StyleAllPanelText(flows[i].levelFailPanel, new Color(1f, 0.73f, 0.30f, 1f));
        }
    }

    static void LayoutLivingRoomItemInfo(PlayerItemSystem items)
    {
        if (items.infoPanel != null)
        {
            RectTransform panelRect = items.infoPanel.GetComponent<RectTransform>();
            if (panelRect != null) panelRect.sizeDelta = new Vector2(640f, 240f);
        }

        if (items.itemNameText != null)
        {
            RectTransform titleRect = items.itemNameText.rectTransform;
            titleRect.anchorMin = titleRect.anchorMax = new Vector2(0f, 1f);
            titleRect.pivot = new Vector2(0f, 1f);
            titleRect.anchoredPosition = new Vector2(54f, -30f);
            titleRect.sizeDelta = new Vector2(350f, 46f);
            items.itemNameText.alignment = TextAlignmentOptions.Left;
        }

        if (items.infoText != null)
        {
            RectTransform descriptionRect = items.infoText.rectTransform;
            descriptionRect.anchorMin = descriptionRect.anchorMax = new Vector2(0f, 0.5f);
            descriptionRect.pivot = new Vector2(0f, 0.5f);
            descriptionRect.anchoredPosition = new Vector2(54f, -24f);
            descriptionRect.sizeDelta = new Vector2(350f, 138f);
            items.infoText.alignment = TextAlignmentOptions.MidlineLeft;
            items.infoText.enableWordWrapping = true;
            items.infoText.overflowMode = TextOverflowModes.Ellipsis;
            items.infoText.fontSize = 24f;
        }

        if (items.itemImage != null)
        {
            RectTransform imageRect = items.itemImage.rectTransform;
            imageRect.anchorMin = imageRect.anchorMax = new Vector2(1f, 0.5f);
            imageRect.pivot = new Vector2(0.5f, 0.5f);
            imageRect.anchoredPosition = new Vector2(-100f, 0f);
            imageRect.sizeDelta = new Vector2(140f, 140f);
        }
    }

    void ThemePanel(GameObject panelObject, string accentName)
    {
        if (panelObject == null) return;

        Image panelImage = panelObject.GetComponent<Image>();
        if (panelImage != null)
        {
            panelImage.sprite = roundedSprite;
            panelImage.type = Image.Type.Sliced;
            panelImage.color = new Color(0.015f, 0.075f, 0.13f, 0.94f);
            panelImage.raycastTarget = false;

            Outline outline = panelImage.GetComponent<Outline>();
            if (outline == null) outline = panelImage.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(Cyan.r, Cyan.g, Cyan.b, 0.62f);
            outline.effectDistance = new Vector2(1.5f, -1.5f);
        }

        if (panelObject.transform.Find(accentName) != null) return;
        Image accent = CreateImage(accentName, panelObject.transform, Cyan);
        RectTransform accentRect = accent.rectTransform;
        accentRect.anchorMin = new Vector2(0f, 0f);
        accentRect.anchorMax = new Vector2(0f, 1f);
        accentRect.pivot = new Vector2(0f, 0.5f);
        accentRect.anchoredPosition = new Vector2(12f, 0f);
        accentRect.sizeDelta = new Vector2(4f, -22f);
    }

    static void StyleAllPanelText(GameObject panelObject, Color color)
    {
        if (panelObject == null) return;
        TMP_Text[] texts = panelObject.GetComponentsInChildren<TMP_Text>(true);
        for (int i = 0; i < texts.Length; i++)
            StyleText(texts[i], 30f, color);
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

public class BathroomHudUsesDisplay : MonoBehaviour
{
    TMP_Text source;
    TMP_Text target;

    public void Configure(TMP_Text sourceText, TMP_Text targetText)
    {
        source = sourceText;
        target = targetText;
    }

    void LateUpdate()
    {
        if (source != null && target != null)
            target.text = source.text.Replace("ใช้ได้อีก:", "คงเหลือ:");
    }
}
