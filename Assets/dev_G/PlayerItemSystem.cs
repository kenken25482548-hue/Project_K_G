using NUnit.Framework.Interfaces;
using TMPro;
using UnityEngine;
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

    private ItemData focusedItem;
    private ItemData carriedItem;
    private CleaningTarget focusedCleaningTarget;
    private bool infoOpen = false;

    void Start()
    {
        if (interactUI) interactUI.SetActive(false);
        if (infoPanel) infoPanel.SetActive(false);
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
            FindNearestCleaningTarget();
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

    void FindNearestCleaningTarget()
    {
        CleaningTarget[] allTargets = FindObjectsByType<CleaningTarget>(FindObjectsSortMode.None);

        float closestDistance = Mathf.Infinity;
        CleaningTarget nearest = null;

        foreach (CleaningTarget target in allTargets)
        {
            if (target == null || target.isCleared) continue;

            float distance = Vector3.Distance(player.position, target.transform.position);

            if (distance <= interactDistance && distance < closestDistance)
            {
                closestDistance = distance;
                nearest = target;
            }
        }

        focusedCleaningTarget = nearest;
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

        if (carriedItem == null || focusedCleaningTarget == null)
        {
            interactUI.SetActive(false);
            return;
        }

        interactUI.SetActive(true);
        interactText.text = "[F] ใช้ " + carriedItem.itemName;
    }

    void HandleUseInput()
    {
        if (carriedItem == null || focusedCleaningTarget == null)
            return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            focusedCleaningTarget.TryUseItem(carriedItem);
            carriedItem = null;

            if (interactUI) interactUI.SetActive(false);
        }
    }

    void OpenInfo(ItemData item)
    {
        if (item == null) return;

        infoOpen = true;

        if (infoPanel) infoPanel.SetActive(true);

        if (itemNameText)
            itemNameText.text = item.itemName;

        if (infoText)
            infoText.text = item.itemDescription;

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