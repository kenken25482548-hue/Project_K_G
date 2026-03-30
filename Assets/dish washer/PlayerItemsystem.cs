using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerItemSystem : MonoBehaviour
{
    [Header("Player")]
    public Transform player;
    public float interactDistance = 3f;

    [Header("UI - Interact")]
    public GameObject interactUI;
    public TMP_Text interactText;

    [Header("UI - Info")]
    public GameObject infoPanel;
    public TMP_Text infoText;
    public TMP_Text itemNameText;
    public Image itemImage;

    [Header("Dirty Target")]
    public CleaningTarget cleaningTarget;   // ลาก Cube ที่มี CleaningTarget มาใส่

    private ItemData focusedItem;
    private ItemData carriedItem;
    private bool infoOpen = false;

    void Start()
    {
        if (interactUI) interactUI.SetActive(false);
        if (infoPanel) infoPanel.SetActive(false);

        if (cleaningTarget == null)
        {
            cleaningTarget = FindObjectOfType<CleaningTarget>();

            if (cleaningTarget == null)
                Debug.LogError("❌ ไม่พบ CleaningTarget ในฉาก");
            else
                Debug.Log("✅ เจอ CleaningTarget: " + cleaningTarget.name);
        }
    }

    void Update()
    {
        if (infoOpen)
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape))
                CloseInfo();
            return;
        }

        if (carriedItem == null)
        {
            FindNearestItem();
            UpdatePickupUI();
            HandlePickupInput();
        }
        else
        {
            UpdateUseUI();
            HandleUseInput();
        }
    }

    void FindNearestItem()
    {
        ItemData[] allItems = FindObjectsByType<ItemData>(FindObjectsSortMode.None);

        float closestDistance = Mathf.Infinity;
        ItemData nearest = null;

        foreach (ItemData item in allItems)
        {
            if (item == null || item.isPicked || item.isUsed) continue;

            float distance = Vector3.Distance(player.position, item.transform.position);

            if (distance <= interactDistance && distance < closestDistance)
            {
                closestDistance = distance;
                nearest = item;
            }
        }

        focusedItem = nearest;
    }

    void UpdatePickupUI()
    {
        if (interactUI == null || interactText == null) return;

        if (focusedItem != null)
        {
            interactUI.SetActive(true);
            interactText.text = "[F] หยิบไอเทม\n[E] ดูข้อมูล";
        }
        else
        {
            interactUI.SetActive(false);
        }
    }

    void HandlePickupInput()
    {
        if (focusedItem == null) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            OpenInfo(focusedItem);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            carriedItem = focusedItem;
            carriedItem.Pick();
            focusedItem = null;

            if (interactUI) interactUI.SetActive(false);
            CloseInfo();
        }
    }

    void UpdateUseUI()
    {
        if (interactUI == null || interactText == null)
            return;

        if (cleaningTarget == null || carriedItem == null || cleaningTarget.isCleared)
        {
            interactUI.SetActive(false);
            return;
        }

        float distance = Vector3.Distance(player.position, cleaningTarget.transform.position);

        if (distance <= interactDistance)
        {
            interactUI.SetActive(true);
            interactText.text = "[F] ใช้ " + carriedItem.itemName;
        }
        else
        {
            interactUI.SetActive(false);
        }
    }

    void HandleUseInput()
    {
        if (cleaningTarget == null || carriedItem == null || cleaningTarget.isCleared)
            return;

        float distance = Vector3.Distance(player.position, cleaningTarget.transform.position);

        if (distance <= interactDistance && Input.GetKeyDown(KeyCode.F))
        {
            cleaningTarget.TryUseItem(carriedItem);
            carriedItem = null;

            if (interactUI) interactUI.SetActive(false);
        }
    }

    void OpenInfo(ItemData item)
    {
        if (item == null) return;

        infoOpen = true;

        if (infoPanel) infoPanel.SetActive(true);
        if (infoText) infoText.text = item.itemDescription;
        if (itemNameText) itemNameText.text = item.itemName;

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

        if (interactUI) interactUI.SetActive(false);
    }

    public void CloseInfo()
    {
        infoOpen = false;

        if (infoPanel != null)
            infoPanel.SetActive(false);

        if (itemImage != null)
        {
            itemImage.sprite = null;
            itemImage.enabled = false;
        }
    }
}