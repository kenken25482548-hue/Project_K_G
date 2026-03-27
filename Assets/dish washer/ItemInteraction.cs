using UnityEngine;
using TMPro;

public class ItemInteraction : MonoBehaviour
{
    public GameObject interactUI;
    public GameObject infoPanel;
    public TextMeshProUGUI infoText;

    public string itemInfo = "ข้อมูลไอเทม";

    public Transform holdPoint;
    public Transform dropPoint;

    private bool playerInRange = false;
    private bool isHolding = false;

    void Start()
    {
        interactUI.SetActive(false);
        infoPanel.SetActive(false);
    }

    void Update()
    {
        if (playerInRange)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                ToggleInfo();
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                PickupItem();
            }
        }
    }

    void ToggleInfo()
    {
        infoPanel.SetActive(!infoPanel.activeSelf);
        infoText.text = itemInfo;
    }

    void PickupItem()
    {
        if (!isHolding)
        {
            transform.position = holdPoint.position;
            transform.parent = holdPoint;
            isHolding = true;
        }
        else
        {
            transform.position = dropPoint.position;
            transform.parent = null;
            isHolding = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            interactUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            interactUI.SetActive(false);
            infoPanel.SetActive(false);
        }
    }
}