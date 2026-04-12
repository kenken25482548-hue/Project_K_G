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

    void Start()
    {
        inventory = new ItemData[inventorySize];

        if (raycastCamera == null)
            raycastCamera = Camera.main;

        interactUI.SetActive(false);
        infoPanel.SetActive(false);
        stainInfoPanel.SetActive(false);

        inventoryUI.SetSelectedSlot(currentSlot);
        inventoryUI.RefreshAll(inventory);

        UpdateUsesUI();
    }

    void Update()
    {
        HandleSlotInput();

        // 🔥 กัน popup ซ้อนทั้งหมด
        if (IsAnyPopupOpen())
        {
            interactUI.SetActive(false);
            return;
        }

        DetectObject();
        UpdateUsesUI();

        if (focusedCleaningTarget != null && focusedCleaningTarget.IsPopupRecentlyClosed())
        {
            ShowStainPromptOnly();
            return;
        }

        if (itemInfoOpen || stainInfoOpen)
        {
            interactUI.SetActive(false);

            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape))
                CloseAllPanels();

            return;
        }

        UpdateInteractUI();
        HandleInput();
    }

    // 🔥 ตรวจ popup ทุกตัวในฉาก
    bool IsAnyPopupOpen()
    {
        CleaningTarget[] all = FindObjectsByType<CleaningTarget>(FindObjectsSortMode.None);

        foreach (var t in all)
        {
            if (t != null && t.IsWrongPopupOpen)
                return true;
        }

        return false;
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
        currentSlot = index;
        inventoryUI.SetSelectedSlot(index);
        UpdateUsesUI();
    }

    void DetectObject()
    {
        focusedItem = null;
        focusedCleaningTarget = null;

        Ray ray = raycastCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit[] hits = Physics.SphereCastAll(ray, detectRadius, interactDistance);

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (var hit in hits)
        {
            if (hit.collider.transform.root == transform.root) continue;

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

    void UpdateInteractUI()
    {
        if (focusedItem != null)
        {
            interactUI.SetActive(true);

            if (CleaningTarget.inspectedStains < CleaningTarget.totalStains)
                interactText.text = "ต้องอ่านคราบให้ครบก่อน";
            else
                interactText.text = "[F] หยิบ\n[E] ดูข้อมูล";

            return;
        }

        if (focusedCleaningTarget != null)
        {
            interactUI.SetActive(true);

            if (!focusedCleaningTarget.isDiscovered)
                interactText.text = "[E] อ่านข้อมูลคราบ";
            else
                interactText.text = "[F] ใช้ไอเทม\n[E] อ่านข้อมูลคราบ";

            return;
        }

        interactUI.SetActive(false);
    }

    void ShowStainPromptOnly()
    {
        if (focusedCleaningTarget != null)
        {
            interactUI.SetActive(true);
            interactText.text = "[E] อ่านข้อมูลคราบ";
        }
    }

    void UpdateUsesUI()
    {
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
            if (CleaningTarget.inspectedStains < CleaningTarget.totalStains)
                return;

            if (Input.GetKeyDown(KeyCode.E))
                OpenItemInfo(focusedItem);

            if (Input.GetKeyDown(KeyCode.F))
                PickupItem();
        }

        if (focusedCleaningTarget != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
                OpenStainInfo(focusedCleaningTarget);

            if (Input.GetKeyDown(KeyCode.F))
                UseItem();
        }
    }

    void PickupItem()
    {
        int slot = GetEmptySlot();
        if (slot == -1) return;

        inventory[slot] = focusedItem;
        focusedItem.Pick();

        inventoryUI.SetSlot(slot, focusedItem);
        UpdateUsesUI();
    }

    void UseItem()
    {
        ItemData item = GetSelectedItem();
        if (item == null) return;

        bool used = focusedCleaningTarget.TryUseItem(item);
        if (!used) return;

        inventory[currentSlot] = null;
        inventoryUI.ClearSlot(currentSlot);
        UpdateUsesUI();
    }

    void OpenItemInfo(ItemData item)
    {
        itemInfoOpen = true;
        infoPanel.SetActive(true);

        itemNameText.text = item.itemName;
        infoText.text = item.itemDescription + "\nใช้ได้: " + item.usesLeft;

        if (item.itemSprite != null)
        {
            itemImage.sprite = item.itemSprite;
            itemImage.enabled = true;
        }
    }

    void OpenStainInfo(CleaningTarget stain)
    {
        stainInfoOpen = true;
        stainInfoPanel.SetActive(true);

        stain.Inspect();

        stainNameText.text = stain.stainName;
        stainDescriptionText.text = stain.stainDescription;
        stainStateText.text = stain.isCleared ? "เสร็จแล้ว" : "ตรวจสอบแล้ว";
    }

    public void CloseAllPanels()
    {
        itemInfoOpen = false;
        stainInfoOpen = false;

        infoPanel.SetActive(false);
        stainInfoPanel.SetActive(false);
    }

    ItemData GetSelectedItem()
    {
        return inventory[currentSlot];
    }

    int GetEmptySlot()
    {
        for (int i = 0; i < inventory.Length; i++)
            if (inventory[i] == null)
                return i;

        return -1;
    }
}