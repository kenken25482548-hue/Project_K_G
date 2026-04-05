using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Slot Backgrounds")]
    public Image[] backgrounds;

    [Header("Slot Icons")]
    public Image[] icons;

    [Header("Background Colors")]
    public Color normalBackgroundColor = new Color(0.15f, 0.35f, 0.55f, 0.35f);
    public Color selectedBackgroundColor = new Color(0.20f, 0.55f, 0.95f, 0.85f);

    [Header("Icon Colors")]
    public Color normalIconColor = new Color(1f, 1f, 1f, 0.45f);
    public Color selectedIconColor = Color.white;

    [Header("Scale")]
    public Vector3 normalScale = Vector3.one;
    public Vector3 selectedScale = new Vector3(1.15f, 1.15f, 1f);

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
        if (backgrounds == null || backgrounds.Length == 0) return;
        if (index < 0 || index >= backgrounds.Length) return;

        currentIndex = index;
        Debug.Log("Selected slot: " + (index + 1));
        UpdateSlotVisual();
    }

    void UpdateSlotVisual()
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            bool isSelected = (i == currentIndex);

            if (backgrounds[i] != null)
            {
                backgrounds[i].color = isSelected ? selectedBackgroundColor : normalBackgroundColor;
            }

            if (icons != null && i < icons.Length && icons[i] != null)
            {
                icons[i].color = isSelected ? selectedIconColor : normalIconColor;
            }

            if (backgrounds[i] != null)
            {
                Transform slotTransform = backgrounds[i].transform.parent;
                if (slotTransform != null)
                {
                    slotTransform.localScale = isSelected ? selectedScale : normalScale;
                }
            }
        }
    }
}