using UnityEngine;
using TMPro;

public class ItemInteraction : MonoBehaviour
{
    [Header("UI")]
    public GameObject interactUI;
    public GameObject infoPanel;
    public TextMeshProUGUI infoText;

    [Header("Item Setting")]
    [TextArea]
    public string itemInfo = "ข้อมูลไอเทม";

    [Header("Pickup Setting")]
    public Transform holdPoint;
    public Transform dropPoint;

    private bool isNear = false;
    private bool isHolding = false;

    // 👉 เก็บตำแหน่งเริ่มต้น
    private Vector3 startPos;
    private Quaternion startRot;

    void Start()
    {
        // 🔥 จำตำแหน่งเริ่มต้น
        startPos = transform.position;
        startRot = transform.rotation;

        // 🔥 บังคับสถานะเริ่มต้น
        isHolding = false;

        // 🔥 ป้องกันติด parent ตั้งแต่แรก
        transform.SetParent(null);

        // 🔥 เปิด collider
        GetComponent<Collider>().enabled = true;

        // 🔥 ปิด UI
        interactUI.SetActive(false);
        infoPanel.SetActive(false);

        // 🔥 ใส่ข้อความ
        if (infoText != null)
            infoText.text = itemInfo;
    }

    void Update()
    {
        if (!isNear) return;

        // กด E = เปิด/ปิดข้อมูล
        if (Input.GetKeyDown(KeyCode.E))
        {
            infoPanel.SetActive(!infoPanel.activeSelf);
        }

        // กด F = หยิบ / วาง
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!isHolding)
                PickUp();
            else
                Drop();
        }
    }

    void PickUp()
    {
        if (holdPoint == null) return;

        isHolding = true;

        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        GetComponent<Collider>().enabled = false;
    }

    void Drop()
    {
        isHolding = false;

        transform.SetParent(null);

        if (dropPoint != null)
        {
            transform.position = dropPoint.position;
        }
        else
        {
            // 👉 ถ้าไม่มี dropPoint กลับที่เดิม
            transform.position = startPos;
            transform.rotation = startRot;
        }

        GetComponent<Collider>().enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNear = true;
            interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNear = false;

            interactUI.SetActive(false);
            infoPanel.SetActive(false);
        }
    }
}