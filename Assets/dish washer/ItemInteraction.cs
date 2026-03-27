using UnityEngine;

public class ItemInteraction : MonoBehaviour
{
    [Header("UI")]
    public GameObject interactUI;
    public GameObject infoPanel;

    [Header("Drop Point")]
    public Transform dropPoint;

    private bool playerInRange = false;
    private bool isPicked = false;

    void Start()
    {
        // ซ่อน UI ตอนเริ่ม
        if (interactUI != null) interactUI.SetActive(false);
        if (infoPanel != null) infoPanel.SetActive(false);
    }

    void Update()
    {
        // รับปุ่มเฉพาะตอนผู้เล่นอยู่ในระยะ
        if (!playerInRange) return;

        // เปิด/ปิดข้อมูล
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E pressed");

            if (infoPanel != null)
                infoPanel.SetActive(!infoPanel.activeSelf);
        }

        // เก็บหรือวางไอเทม
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("F pressed");

            if (!isPicked)
            {
                PickupItem();
            }
            else
            {
                DropItem();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered");

            playerInRange = true;

            if (interactUI != null)
                interactUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited");

            playerInRange = false;

            // ปิด UI ทั้งหมดเมื่อออกระยะ
            if (interactUI != null)
                interactUI.SetActive(false);

            if (infoPanel != null)
                infoPanel.SetActive(false);
        }
    }

    void PickupItem()
    {
        Debug.Log("Item picked");

        isPicked = true;

        // ซ่อนไอเทมจากฉาก
        gameObject.SetActive(false);
    }

    void DropItem()
    {
        Debug.Log("Item dropped");

        isPicked = false;

        // วางกลับที่ตำแหน่งที่กำหนด
        if (dropPoint != null)
        {
            transform.position = dropPoint.position;
        }

        gameObject.SetActive(true);
    }
}