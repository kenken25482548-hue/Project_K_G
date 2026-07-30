using TMPro;
using UnityEngine;

public class ObjectivePanelUI : MonoBehaviour
{
    [Header("Texts")]
    public TMP_Text inspectObjectiveText;
    public TMP_Text cleanObjectiveText;
    public TMP_Text unlockObjectiveText;

    [Header("Optional")]
    public TMP_Text titleText;

    [Header("Colors")]
    public Color titleColor = new Color32(255, 255, 255, 255);       // #FFFFFF
    public Color normalColor = new Color32(244, 251, 255, 255);      // #F4FBFF
    public Color lockedColor = new Color32(255, 214, 107, 255);      // #FFD66B
    public Color readyColor = new Color32(140, 255, 183, 255);       // #8CFFB7

    private CleaningTarget[] stains;

    void Start()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "1bathroom1" &&
            GetComponent<BathroomHudPolish>() == null)
        {
            gameObject.AddComponent<BathroomHudPolish>();
        }

        RefreshStainList();
        UpdateObjectiveUI();
    }

    void Update()
    {
        // เผื่อมีการเปิด/ปิด stain object ระหว่างเล่น
        if (stains == null || stains.Length != CleaningTarget.totalStains)
        {
            RefreshStainList();
        }

        UpdateObjectiveUI();
    }

    void RefreshStainList()
    {
        stains = FindObjectsOfType<CleaningTarget>();
    }

    void UpdateObjectiveUI()
    {
        int total = stains != null ? stains.Length : 0;
        int inspected = GetInspectedCount();
        int cleared = GetClearedCount();

        bool allInspected = total > 0 && inspected >= total;

        if (titleText != null)
        {
            titleText.text = "ภารกิจ";
            titleText.color = titleColor;
        }

        if (inspectObjectiveText != null)
        {
            inspectObjectiveText.text = "ตรวจสอบคราบทั้งหมด " + inspected + " / " + total;
            inspectObjectiveText.color = normalColor;
        }

        if (cleanObjectiveText != null)
        {
            cleanObjectiveText.text = "ล้างคราบทั้งหมด " + cleared + " / " + total;
            cleanObjectiveText.color = normalColor;
        }

        if (unlockObjectiveText != null)
        {
            if (allInspected)
            {
                unlockObjectiveText.text = "ปลดล็อกการหยิบไอเทม: พร้อมใช้งาน";
                unlockObjectiveText.color = readyColor;
            }
            else
            {
                unlockObjectiveText.text = "ปลดล็อกการหยิบไอเทม: ยังไม่พร้อม";
                unlockObjectiveText.color = lockedColor;
            }
        }
    }

    int GetClearedCount()
    {
        int count = 0;

        if (stains == null) return 0;

        for (int i = 0; i < stains.Length; i++)
        {
            if (stains[i] != null && stains[i].isCleared)
            {
                count++;
            }
        }

        return count;
    }

    int GetInspectedCount()
    {
        int count = 0;

        if (stains == null) return 0;

        for (int i = 0; i < stains.Length; i++)
        {
            if (stains[i] != null && stains[i].isDiscovered)
                count++;
        }

        return count;
    }
}
