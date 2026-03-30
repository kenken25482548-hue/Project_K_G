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
    public Image itemImage;

    private ItemData focusedItem;
    private ItemData carriedItem;
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
            HandleInfoInput();
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
            UpdateDropUI();
            HandleDropInput();
        }
    }

    void FindNearestItem()
    {
        ItemData[] allItems = FindObjectsOfType<ItemData>();
        float closestDistance = Mathf.Infinity;
        ItemData nearest = null;

        foreach (ItemData item in allItems)
        {
            if (item == null || item.isPicked) continue;

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
        if (focusedItem != null)
        {
            if (interactUI) interactUI.SetActive(true);
            if (interactText) interactText.text = "[F] หยิบไอเทม\n[E] ดูข้อมูล";
        }
        else
        {
            if (interactUI) interactUI.SetActive(false);
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
            carriedItem.HideItem();
            focusedItem = null;

            if (interactUI) interactUI.SetActive(false);
            if (infoPanel) infoPanel.SetActive(false);
            infoOpen = false;
        }
    }

    void UpdateDropUI()
    {
        if (carriedItem == null || carriedItem.dropPoint == null)
        {
            if (interactUI) interactUI.SetActive(false);
            return;
        }

        float distanceToDrop = Vector3.Distance(player.position, carriedItem.dropPoint.position);

        if (distanceToDrop <= interactDistance)
        {
            if (interactUI) interactUI.SetActive(true);
            if (interactText) interactText.text = "[F] วางไอเทม";
        }
        else
        {
            if (interactUI) interactUI.SetActive(false);
        }
    }

    void HandleDropInput()
    {
        if (carriedItem == null || carriedItem.dropPoint == null) return;

        float distanceToDrop = Vector3.Distance(player.position, carriedItem.dropPoint.position);

        if (distanceToDrop <= interactDistance && Input.GetKeyDown(KeyCode.F))
        {
            carriedItem.ShowAtDropPoint();
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

        if (itemImage != null)
        {
            if (item.itemSprite != null)
            {
                itemImage.sprite = item.itemSprite;
                itemImage.enabled = true;
            }
            else
            {
                itemImage.enabled = false;
            }
        }

        if (interactUI) interactUI.SetActive(false);
    }

    void HandleInfoInput()
    {
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape))
        {
            CloseInfo();
        }
    }

    public void CloseInfo()
    {
        infoOpen = false;
        if (infoPanel) infoPanel.SetActive(false);
    }
}
