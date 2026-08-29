using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItemSystem : MonoBehaviour
{
    [Header("Raycast")]
    public Camera raycastCamera;
    public float interactDistance = 4f;
    public float detectRadius = 0.4f;

    [Header("UI - Interact")]
    public GameObject interactUI;
    public TMP_Text interactText;

    [Header("UI - Item Info")]
    public GameObject infoPanel;
    public TMP_Text infoText;
    public TMP_Text itemNameText;
    public Image itemImage;

    [Header("UI - Stain Info")]
    public GameObject stainInfoPanel;
    public TMP_Text stainNameText;
    public TMP_Text stainDescriptionText;
    public TMP_Text stainStateText;

    [Header("UI - Uses")]
    public TMP_Text usesText;

    [Header("Inventory")]
    public InventoryUI inventoryUI;
    public int inventorySize = 5;

    private ItemData[] inventory;
    private int currentSlot = 0;

    private ItemData focusedItem;
    private CleaningTarget focusedCleaningTarget;

    private bool itemInfoOpen = false;
    private bool stainInfoOpen = false;
    private bool keepItemInfoOpenWithoutFocus;

    void Start()
    {
        inventory = new ItemData[inventorySize];

        if (raycastCamera == null)
            raycastCamera = Camera.main;

        if (interactUI != null) interactUI.SetActive(false);
        if (infoPanel != null) infoPanel.SetActive(false);
        if (stainInfoPanel != null) stainInfoPanel.SetActive(false);

        if (inventoryUI != null)
        {
            inventoryUI.SetSelectedSlot(currentSlot);
            inventoryUI.RefreshAll(inventory);
        }

        UpdateUsesUI();
    }

    void Update()
    {
        if (PauseMenuUI.IsPaused)
            return;

        HandleSlotInput();
        DetectObject();
        UpdateUsesUI();

        if ((itemInfoOpen || stainInfoOpen) && focusedItem == null && focusedCleaningTarget == null &&
            !keepItemInfoOpenWithoutFocus)
        {
            CloseAllPanels();
        }

        if (focusedCleaningTarget != null && focusedCleaningTarget.IsWrongPopupOpen)
        {
            if (interactUI != null)
                interactUI.SetActive(false);
            return;
        }

        if (focusedCleaningTarget != null && focusedCleaningTarget.IsPopupRecentlyClosed())
        {
            ShowStainPromptOnly();
            return;
        }

        // Reopen the description of the currently selected inventory item anywhere in the level.
        if (!itemInfoOpen && !stainInfoOpen && Input.GetKeyDown(KeyCode.Q))
        {
            ItemData selectedItem = GetSelectedItem();
            if (selectedItem != null)
            {
                OpenItemInfo(selectedItem, true);
                return;
            }
        }

        if (itemInfoOpen || stainInfoOpen)
        {
            if (interactUI != null)
                interactUI.SetActive(false);

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Q))
                CloseAllPanels();

            return;
        }

        UpdateInteractUI();
        HandleInput();
    }

    void HandleSlotInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetSlot(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SetSlot(4);
    }

    void SetSlot(int index)
    {
        if (inventory == null) return;
        if (index < 0 || index >= inventory.Length) return;
        if (currentSlot == index) return;

        currentSlot = index;

        if (inventoryUI != null)
            inventoryUI.SetSelectedSlot(index);

        GameSFXManager.PlaySfx(GameSFXManager.Instance != null ? GameSFXManager.Instance.slotChangeSfx : null, 0.9f);
        UpdateUsesUI();
    }

    void DetectObject()
    {
        focusedItem = null;
        focusedCleaningTarget = null;

        if (FocusNearestInteractionZone())
            return;

        if (raycastCamera == null)
            return;

        Ray ray = raycastCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit[] hits = Physics.SphereCastAll(ray, detectRadius, interactDistance);

        if (hits == null || hits.Length == 0)
            return;

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (var hit in hits)
        {
            if (hit.collider == null) continue;

            if (hit.collider.transform.root == transform.root)
                continue;

            InteractionZone interactionZone = hit.collider.GetComponentInParent<InteractionZone>();
            if (interactionZone != null && !interactionZone.IsPositionInside(transform.position))
                continue;

            ItemData item = hit.collider.GetComponentInParent<ItemData>();
            if (item != null && !item.isPicked && !item.isUsed)
            {
                focusedItem = item;
                return;
            }

            CleaningTarget stain = hit.collider.GetComponentInParent<CleaningTarget>();
            if (stain != null && !stain.isCleared)
            {
                focusedCleaningTarget = stain;
                return;
            }
        }
    }

    bool FocusNearestInteractionZone()
    {
        InteractionZone[] zones = FindObjectsOfType<InteractionZone>();
        float closestDistance = float.MaxValue;

        for (int i = 0; i < zones.Length; i++)
        {
            InteractionZone zone = zones[i];
            if (zone == null || !zone.IsPositionInside(transform.position)) continue;

            float distance = Vector3.SqrMagnitude(zone.transform.position - transform.position);
            if (distance >= closestDistance) continue;

            ItemData item = zone.GetComponentInParent<ItemData>();
            if (item != null && !item.isPicked && !item.isUsed)
            {
                closestDistance = distance;
                focusedItem = item;
                focusedCleaningTarget = null;
                continue;
            }

            CleaningTarget stain = zone.GetComponentInParent<CleaningTarget>();
            if (stain != null && !stain.isCleared)
            {
                closestDistance = distance;
                focusedCleaningTarget = stain;
                focusedItem = null;
            }
        }

        return focusedItem != null || focusedCleaningTarget != null;
    }

    void UpdateInteractUI()
    {
        if (interactUI == null || interactText == null)
            return;

        if (focusedItem != null)
        {
            interactUI.SetActive(true);

            if (CleaningTarget.inspectedStains < CleaningTarget.totalStains)
                interactText.text = "สำรวจสิ่งสกปรกให้ครบก่อน!";
            else
                interactText.text = "[F] หยิบไอเทม\n[E] ดูข้อมูล";

            return;
        }

        if (focusedCleaningTarget != null)
        {
            interactUI.SetActive(true);

            ItemData item = GetSelectedItem();

            if (!focusedCleaningTarget.isDiscovered)
            {
                interactText.text = "[E] สังเกตุคราบสกปรก";
            }
            else
            {
                if (item != null)
                    interactText.text = "[F] ใช้ " + item.itemName + "\n[E] สังเกตุคราบสกปรก";
                else
                    interactText.text = "[E] สังเกตุคราบสกปรก";
            }

            return;
        }

        interactUI.SetActive(false);
    }

    void ShowStainPromptOnly()
    {
        if (interactUI == null || interactText == null) return;

        if (focusedCleaningTarget != null && !focusedCleaningTarget.isCleared)
        {
            interactUI.SetActive(true);
            interactText.text = "[E] สังเกตุคราบสกปรก";
        }
        else
        {
            interactUI.SetActive(false);
        }
    }

    void UpdateUsesUI()
    {
        if (usesText == null) return;

        ItemData item = GetSelectedItem();

        if (item != null)
            usesText.text = "ใช้ได้อีก: " + item.usesLeft;
        else
            usesText.text = "";
    }

    void HandleInput()
    {
        if (focusedItem != null)
        {
            bool allStainsInspected = CleaningTarget.inspectedStains >= CleaningTarget.totalStains;

            if (!allStainsInspected)
            {
                if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.F))
                {
                    if (interactUI != null)
                        interactUI.SetActive(true);

                    if (interactText != null)
                        interactText.text = "ตรวจคราบให้ครบก่อน!";
                }

                return;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                OpenItemInfo(focusedItem);
                return;
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                PickupItem();
                return;
            }
        }

        if (focusedCleaningTarget != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                OpenStainInfo(focusedCleaningTarget);
                return;
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                TryUseSelectedItemOnStain();
                return;
            }
        }
    }

    void PickupItem()
    {
        if (focusedItem == null) return;

        if (CleaningTarget.inspectedStains < CleaningTarget.totalStains)
        {
            if (interactText != null)
                interactText.text = "ตรวจคราบให้ครบก่อน!";
            return;
        }

        int slot = GetEmptySlot();
        if (slot == -1)
        {
            Debug.Log("Inventory เต็ม");
            return;
        }

        inventory[slot] = focusedItem;
        focusedItem.Pick();

        if (inventoryUI != null)
            inventoryUI.SetSlot(slot, focusedItem);

        GameSFXManager.PlaySfx(GameSFXManager.Instance != null ? GameSFXManager.Instance.pickupSfx : null, 1f);

        focusedItem = null;
        UpdateUsesUI();
    }

    void TryUseSelectedItemOnStain()
    {
        if (focusedCleaningTarget == null) return;
        if (!focusedCleaningTarget.isDiscovered) return;

        ItemData item = GetSelectedItem();
        if (item == null) return;

        CloseAllPanels();

        bool used = focusedCleaningTarget.TryUseItem(item);
        if (!used) return;

        // Keep an item in the inventory until all of its mission-configured charges are spent.
        if (item.isUsed)
        {
            inventory[currentSlot] = null;

            if (inventoryUI != null)
                inventoryUI.ClearSlot(currentSlot);
        }

        UpdateUsesUI();
    }

    void OpenItemInfo(ItemData item, bool keepOpenWithoutFocus = false)
    {
        if (item == null) return;

        CloseAllPanels();

        if (interactUI != null)
            interactUI.SetActive(false);

        itemInfoOpen = true;
        keepItemInfoOpenWithoutFocus = keepOpenWithoutFocus;

        if (infoPanel != null)
            infoPanel.SetActive(true);

        if (itemNameText != null)
            itemNameText.text = item.itemName;

        if (infoText != null)
            infoText.text = item.itemDescription + "\nจำนวนใช้ได้: " + item.usesLeft;

        if (itemImage != null)
        {
            if (item.itemSprite != null)
            {
                itemImage.sprite = item.itemSprite;
                itemImage.enabled = true;
                itemImage.preserveAspect = true;
                itemImage.color = Color.white;
            }
            else
            {
                itemImage.sprite = null;
                itemImage.enabled = false;
            }
        }

        GameSFXManager.PlaySfx(GameSFXManager.Instance != null ? GameSFXManager.Instance.openInfoSfx : null, 1f);
    }

    void OpenStainInfo(CleaningTarget stain)
    {
        if (stain == null) return;

        CloseAllPanels();

        if (interactUI != null)
            interactUI.SetActive(false);

        stain.Inspect();
        stainInfoOpen = true;

        if (stainInfoPanel != null)
            stainInfoPanel.SetActive(true);

        if (stainNameText != null)
            stainNameText.text = stain.stainName;

        if (stainDescriptionText != null)
            stainDescriptionText.text = stain.stainDescription;

        if (stainStateText != null)
            stainStateText.text = stain.isCleared ? "สถานะ: ทำความสะอาดแล้ว" : "สถานะ: ตรวจสอบแล้ว";

        GameSFXManager.PlaySfx(GameSFXManager.Instance != null ? GameSFXManager.Instance.openInfoSfx : null, 1f);
    }

    public void CloseAllPanels()
    {
        itemInfoOpen = false;
        stainInfoOpen = false;
        keepItemInfoOpenWithoutFocus = false;

        if (infoPanel != null) infoPanel.SetActive(false);
        if (stainInfoPanel != null) stainInfoPanel.SetActive(false);

        if (itemImage != null)
        {
            itemImage.sprite = null;
            itemImage.enabled = false;
        }
    }

    public void CloseInfo()
    {
        CloseAllPanels();
    }

    public void CloseStainInfo()
    {
        CloseAllPanels();
    }

    ItemData GetSelectedItem()
    {
        if (inventory == null) return null;
        if (currentSlot < 0 || currentSlot >= inventory.Length) return null;
        return inventory[currentSlot];
    }

    int GetEmptySlot()
    {
        if (inventory == null) return -1;

        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == null)
                return i;
        }

        return -1;
    }
}
