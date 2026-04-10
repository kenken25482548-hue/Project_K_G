using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItemSystem : MonoBehaviour
{
    [Header("Raycast")]
    public Camera raycastCamera;
    public float interactDistance = 3f;

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

    [Header("Inventory")]
    public InventoryUI inventoryUI;
    public int inventorySize = 5;

    private ItemData[] inventory;
    private int currentSlot = 0;

    private ItemData focusedItem;
    private CleaningTarget focusedCleaningTarget;

    private bool itemInfoOpen = false;
    private bool stainInfoOpen = false;

    void Start()
    {
        inventory = new ItemData[inventorySize];

        if (interactUI != null) interactUI.SetActive(false);
        if (infoPanel != null) infoPanel.SetActive(false);
        if (stainInfoPanel != null) stainInfoPanel.SetActive(false);

        if (inventoryUI != null)
        {
            inventoryUI.SetSelectedSlot(currentSlot);
            inventoryUI.RefreshAll(inventory);
        }

        if (raycastCamera == null)
            raycastCamera = Camera.main;
    }

    void Update()
    {
        HandleSlotInput();

        if (itemInfoOpen || stainInfoOpen)
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape))
                CloseAllPanels();
            return;
        }

        DetectObject();
        HandleInteractionInput();
    }

    void HandleSlotInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetCurrentSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetCurrentSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetCurrentSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetCurrentSlot(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SetCurrentSlot(4);
    }

    void SetCurrentSlot(int index)
    {
        if (inventory == null) return;
        if (index < 0 || index >= inventory.Length) return;

        currentSlot = index;

        if (inventoryUI != null)
            inventoryUI.SetSelectedSlot(currentSlot);
    }

    void DetectObject()
    {
        focusedItem = null;
        focusedCleaningTarget = null;

        if (raycastCamera == null)
        {
            if (interactUI != null) interactUI.SetActive(false);
            return;
        }

        Ray ray = new Ray(raycastCamera.transform.position, raycastCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            focusedItem = hit.collider.GetComponentInParent<ItemData>();
            focusedCleaningTarget = hit.collider.GetComponentInParent<CleaningTarget>();

            UpdateInteractUI();
        }
        else
        {
            if (interactUI != null)
                interactUI.SetActive(false);
        }
    }

    void UpdateInteractUI()
    {
        if (interactUI == null || interactText == null)
            return;

        ItemData selectedItem = GetSelectedItem();

        if (focusedItem != null && !focusedItem.isPicked && !focusedItem.isUsed)
        {
            interactUI.SetActive(true);
            interactText.text = "[F] หยิบไอเทม\n[E] ดูข้อมูล";
            return;
        }

        if (focusedCleaningTarget != null && !focusedCleaningTarget.isCleared)
        {
            interactUI.SetActive(true);

            if (!focusedCleaningTarget.isDiscovered)
            {
                interactText.text = "[E] ตรวจสอบคราบ";
            }
            else if (selectedItem != null)
            {
                interactText.text = "[F] ใช้ " + selectedItem.itemName + "\n[E] อ่านข้อมูลคราบ";
            }
            else
            {
                interactText.text = "[E] อ่านข้อมูลคราบ";
            }

            return;
        }

        interactUI.SetActive(false);
    }

    void HandleInteractionInput()
    {
        if (focusedItem != null && !focusedItem.isPicked && !focusedItem.isUsed)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                OpenItemInfo(focusedItem);
                return;
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                PickupFocusedItem();
                return;
            }
        }

        if (focusedCleaningTarget != null && !focusedCleaningTarget.isCleared)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                OpenStainInfo(focusedCleaningTarget);
                return;
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                TryUseSelectedItemOnFocusedStain();
                return;
            }
        }
    }

    void PickupFocusedItem()
    {
        if (focusedItem == null) return;

        int emptyIndex = GetFirstEmptySlot();
        if (emptyIndex == -1)
        {
            Debug.Log("Inventory เต็ม");
            return;
        }

        inventory[emptyIndex] = focusedItem;
        focusedItem.Pick();

        if (inventoryUI != null)
            inventoryUI.SetSlot(emptyIndex, focusedItem);

        focusedItem = null;

        if (interactUI != null)
            interactUI.SetActive(false);
    }

    void TryUseSelectedItemOnFocusedStain()
    {
        if (focusedCleaningTarget == null) return;
        if (!focusedCleaningTarget.isDiscovered) return;

        ItemData selectedItem = GetSelectedItem();
        if (selectedItem == null) return;

        bool useWasCounted = focusedCleaningTarget.TryUseItem(selectedItem);

        if (useWasCounted)
        {
            inventory[currentSlot] = null;

            if (inventoryUI != null)
                inventoryUI.ClearSlot(currentSlot);
        }

        if (interactUI != null)
            interactUI.SetActive(false);
    }

    void OpenItemInfo(ItemData item)
    {
        if (item == null) return;

        itemInfoOpen = true;

        if (infoPanel != null)
            infoPanel.SetActive(true);

        if (itemNameText != null)
            itemNameText.text = item.itemName;

        if (infoText != null)
            infoText.text = item.itemDescription + "\n\nจำนวนใช้ได้: " + item.usesLeft;

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

        if (interactUI != null)
            interactUI.SetActive(false);
    }

    void OpenStainInfo(CleaningTarget target)
    {
        if (target == null) return;

        target.Inspect();
        stainInfoOpen = true;

        if (stainInfoPanel != null)
            stainInfoPanel.SetActive(true);

        if (stainNameText != null)
            stainNameText.text = target.stainName;

        if (stainDescriptionText != null)
            stainDescriptionText.text = target.stainDescription;

        if (stainStateText != null)
            stainStateText.text = target.isCleared ? "สถานะ: ทำความสะอาดแล้ว" : "สถานะ: ตรวจสอบแล้ว";

        if (interactUI != null)
            interactUI.SetActive(false);
    }

    public void CloseAllPanels()
    {
        itemInfoOpen = false;
        stainInfoOpen = false;

        if (infoPanel != null)
            infoPanel.SetActive(false);

        if (stainInfoPanel != null)
            stainInfoPanel.SetActive(false);

        if (itemImage != null)
        {
            itemImage.sprite = null;
            itemImage.enabled = false;
        }
    }

    // เผื่อปุ่ม Close เดิมของมึงยังเรียกเมธอดนี้อยู่
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

    int GetFirstEmptySlot()
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