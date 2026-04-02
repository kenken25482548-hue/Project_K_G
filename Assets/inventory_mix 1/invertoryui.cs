using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Slot Objects")]
    public GameObject[] slots;

    [Header("Slot Images")]
    public Image[] slotImages;

    [Header("Colors")]
    public Color normalColor = new Color(0.7f, 0.85f, 1f, 0.35f);
    public Color selectedColor = new Color(0.45f, 0.75f, 1f, 0.95f);

    private int currentIndex = 0;

    void Start()
    {
        UpdateSlotVisual();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) Select(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) Select(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) Select(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) Select(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) Select(4);
    }

    void Select(int index)
    {
        if (index < 0 || index >= slotImages.Length) return;

        currentIndex = index;
        Debug.Log("Selected slot: " + (index + 1));
        UpdateSlotVisual();
    }

    void UpdateSlotVisual()
    {
        for (int i = 0; i < slotImages.Length; i++)
        {
            if (slotImages[i] != null)
            {
                slotImages[i].color = (i == currentIndex) ? selectedColor : normalColor;
            }
        }
    }
}