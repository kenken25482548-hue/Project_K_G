using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CleaningTarget : MonoBehaviour
{
    public string stainName;
    public string stainDescription;

    public string requiredItemName;

    public GameObject wrongPopup;
    public TMP_Text wrongPopupText;

    public GameObject dirtObject;

    public bool isDiscovered = false;
    public bool isCleared = false;

    public static int totalStains = 0;
    public static int inspectedStains = 0;

    private bool popupOpen = false;
    private float popupCloseTime = 0f;

    public bool IsWrongPopupOpen => popupOpen;

    public bool IsPopupRecentlyClosed()
    {
        return Time.time < popupCloseTime;
    }

    void OnEnable()
    {
        totalStains++;
    }

    void OnDisable()
    {
        totalStains = Mathf.Max(0, totalStains - 1);

        if (isDiscovered)
            inspectedStains = Mathf.Max(0, inspectedStains - 1);
    }

    public void Inspect()
    {
        if (!isDiscovered)
        {
            isDiscovered = true;
            inspectedStains++;
        }
    }

    public bool TryUseItem(ItemData item)
    {
        if (item == null) return false;
        if (!isDiscovered) return false;
        if (isCleared) return false;
        if (item.isUsed) return false;

        // ใช้ผิดหรือถูกก็นับจำนวนใช้
        item.UseOnce();

        if (item.itemName == requiredItemName)
        {
            isCleared = true;

            if (dirtObject != null)
                dirtObject.SetActive(false);

            ClosePopupIfOpen();
        }
        else
        {
            ShowPopup();
        }

        CheckFail();
        return true;
    }

    void ShowPopup()
    {
        if (wrongPopupText != null)
            wrongPopupText.text = "อุปกรณ์ไม่ถูกต้อง\nการลองใช้ครั้งนี้นับจำนวนการใช้\nกด E เพื่อปิด";

        if (wrongPopup != null)
            wrongPopup.SetActive(true);

        popupOpen = true;
    }

    void ClosePopupIfOpen()
    {
        if (wrongPopup != null)
            wrongPopup.SetActive(false);

        popupOpen = false;
    }

    void Update()
    {
        if (popupOpen && Input.GetKeyDown(KeyCode.E))
        {
            if (wrongPopup != null)
                wrongPopup.SetActive(false);

            popupOpen = false;
            popupCloseTime = Time.time + 0.25f;
        }
    }

    void CheckFail()
    {
        CleaningTarget[] stains = FindObjectsByType<CleaningTarget>(FindObjectsSortMode.None);
        ItemData[] items = FindObjectsByType<ItemData>(FindObjectsSortMode.None);

        bool hasUnclearedStain = false;
        foreach (var stain in stains)
        {
            if (stain != null && !stain.isCleared)
            {
                hasUnclearedStain = true;
                break;
            }
        }

        // ถ้าไม่มีคราบเหลือ = ยังไม่ใช่แพ้
        if (!hasUnclearedStain)
            return;

        bool hasUsableItemLeft = false;
        foreach (var item in items)
        {
            if (item != null && !item.isUsed && item.usesLeft > 0)
            {
                hasUsableItemLeft = true;
                break;
            }
        }

        // ถ้ายังมีคราบเหลือ แต่ไม่มีของใช้แล้ว = รีด่าน
        if (!hasUsableItemLeft)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}