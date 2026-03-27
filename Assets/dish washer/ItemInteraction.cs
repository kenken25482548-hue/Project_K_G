using UnityEngine;
using TMPro;

public class ItemInteraction : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Hold Position")]
    public Transform holdPoint;

    [Header("Distance Settings")]
    public float interactDistance = 3f;

    [Header("UI Choice Panel")]
    public GameObject choiceUI;

    [Header("Info Panel")]
    public GameObject infoPanel;
    public TMP_Text infoText;

    [Header("Item Description")]
    [TextArea(3, 5)]
    public string itemDescription;

    [Header("Drop System")]
    public Transform dropPoint;

    private bool isNear = false;
    private bool isCarrying = false;

    void Start()
    {
        if (choiceUI != null)
            choiceUI.SetActive(false);

        if (infoPanel != null)
            infoPanel.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        // เข้าใกล้ไอเทม
        if (distance <= interactDistance && !isCarrying)
        {
            isNear = true;

            if (choiceUI != null)
                choiceUI.SetActive(true);
        }
        else
        {
            isNear = false;

            if (choiceUI != null)
                choiceUI.SetActive(false);
        }

        // กด E = เปิด / ปิด Info
        if (isNear && Input.GetKeyDown(KeyCode.E))
        {
            ToggleInfo();
        }

        // กด F = เก็บไอเทม
        if (isNear && !isCarrying && Input.GetKeyDown(KeyCode.F))
        {
            PickupItem();
        }

        // ไปถึงจุดวางแล้วกด F
        if (isCarrying && dropPoint != null)
        {
            float dropDistance = Vector3.Distance(player.position, dropPoint.position);

            if (dropDistance <= interactDistance && Input.GetKeyDown(KeyCode.F))
            {
                DropItem();
            }
        }
    }

    void ToggleInfo()
    {
        if (infoPanel == null) return;

        bool isOpen = infoPanel.activeSelf;
        infoPanel.SetActive(!isOpen);

        if (infoText != null)
            infoText.text = itemDescription;
    }

    void PickupItem()
    {
        isCarrying = true;

        if (choiceUI != null)
            choiceUI.SetActive(false);

        // เอาไอเทมไปติดกับผู้เล่น
        if (holdPoint != null)
        {
            transform.SetParent(holdPoint);
            transform.localPosition = Vector3.zero;
        }
    }

    void DropItem()
    {
        isCarrying = false;

        // ปล่อยจาก player
        transform.SetParent(null);

        // วางที่ drop point
        transform.position = dropPoint.position;
    }
}