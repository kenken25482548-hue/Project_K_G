using UnityEngine;
using TMPro;

public class ItemInteraction : MonoBehaviour
{
    public static ItemInteraction currentItem;

    [Header("Player")]
    public Transform player;
    public Transform holdPoint;

    [Header("Distance")]
    public float interactDistance = 3f;

    [Header("UI")]
    public GameObject choiceUI;
    public GameObject infoPanel;
    public TMP_Text infoText;

    [Header("Item Info")]
    [TextArea(3, 5)]
    public string itemDescription;

    bool isNear = false;
    bool isCarrying = false;

    void Start()
    {
        if (choiceUI) choiceUI.SetActive(false);
        if (infoPanel) infoPanel.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactDistance && !isCarrying)
        {
            isNear = true;
            currentItem = this;

            if (choiceUI) choiceUI.SetActive(true);
        }
        else
        {
            isNear = false;

            if (choiceUI) choiceUI.SetActive(false);
        }

        if (currentItem == this)
        {
            if (isNear && Input.GetKeyDown(KeyCode.E))
            {
                ToggleInfo();
            }

            if (isNear && !isCarrying && Input.GetKeyDown(KeyCode.F))
            {
                PickupItem();
            }
        }

        // วางของ
        if (isCarrying && Input.GetKeyDown(KeyCode.F))
        {
            DropItem();
        }
    }

    void ToggleInfo()
    {
        if (infoPanel == null) return;

        bool open = infoPanel.activeSelf;
        infoPanel.SetActive(!open);

        if (infoText)
            infoText.text = itemDescription;
    }

    void PickupItem()
    {
        isCarrying = true;

        if (choiceUI) choiceUI.SetActive(false);

        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
    }

    void DropItem()
    {
        isCarrying = false;

        transform.SetParent(null);

        // วางตรงหน้าผู้เล่น
        Vector3 dropPos = player.position + player.forward * 1.5f;

        transform.position = dropPos;
    }
}