using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    public Image[] slotBackgrounds;
    public Image[] slotIcons;
    public TMP_Text[] slotUseTexts;

    [Header("Visual")]
    public Color normalColor = new Color32(255, 255, 255, 40);
    public Color selectedColor = new Color32(255, 255, 255, 140);

    public Vector3 normalScale = Vector3.one;
    public Vector3 selectedScale = new Vector3(1.1f, 1.1f, 1f);

    private int currentSlot = 0;

    void Start()
    {
        SetSelectedSlot(0);
    }

    public void SetSelectedSlot(int index)
    {
        currentSlot = Mathf.Clamp(index, 0, Mathf.Max(0, slotBackgrounds.Length - 1));
        RefreshSelection();
    }

    public void SetSlot(int index, ItemData item)
    {
        if (!IsValidIndex(index)) return;

        if (slotIcons != null && index < slotIcons.Length && slotIcons[index] != null)
        {
            if (item != null && item.itemSprite != null)
            {
                slotIcons[index].sprite = item.itemSprite;
                slotIcons[index].enabled = true;
                slotIcons[index].color = Color.white;
                slotIcons[index].preserveAspect = true;
            }
            else
            {
                slotIcons[index].sprite = null;
                slotIcons[index].enabled = false;
            }
        }

        if (slotUseTexts != null && index < slotUseTexts.Length && slotUseTexts[index] != null)
        {
            if (item != null)
                slotUseTexts[index].text = item.usesLeft.ToString();
            else
                slotUseTexts[index].text = "";
        }
    }

    public void ClearSlot(int index)
    {
        if (!IsValidIndex(index)) return;

        if (slotIcons != null && index < slotIcons.Length && slotIcons[index] != null)
        {
            slotIcons[index].sprite = null;
            slotIcons[index].enabled = false;
        }

        if (slotUseTexts != null && index < slotUseTexts.Length && slotUseTexts[index] != null)
        {
            slotUseTexts[index].text = "";
        }
    }

    public void RefreshSlot(int index, ItemData item)
    {
        if (item == null)
            ClearSlot(index);
        else
            SetSlot(index, item);
    }

    public void RefreshAll(ItemData[] inventory)
    {
        int max = 0;

        if (inventory != null)
            max = inventory.Length;
        else
            max = 0;

        for (int i = 0; i < max; i++)
        {
            RefreshSlot(i, inventory[i]);
        }
    }

    private void RefreshSelection()
    {
        if (slotBackgrounds == null) return;

        for (int i = 0; i < slotBackgrounds.Length; i++)
        {
            if (slotBackgrounds[i] == null) continue;

            if (i == currentSlot)
            {
                slotBackgrounds[i].color = selectedColor;
                slotBackgrounds[i].transform.localScale = selectedScale;
            }
            else
            {
                slotBackgrounds[i].color = normalColor;
                slotBackgrounds[i].transform.localScale = normalScale;
            }
        }
    }

    private bool IsValidIndex(int index)
    {
        return index >= 0 && slotBackgrounds != null && index < slotBackgrounds.Length;
    }
}