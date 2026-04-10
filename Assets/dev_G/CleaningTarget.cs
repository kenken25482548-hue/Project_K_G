using UnityEngine;
using TMPro;

public class CleaningTarget : MonoBehaviour
{
    [Header("Stain Info")]
    public string stainName;

    [TextArea(2, 5)]
    public string stainDescription;

    [Header("Required Item")]
    public string requiredItemName;

    [Header("Wrong Popup")]
    public GameObject wrongPopup;
    public TMP_Text wrongPopupText;

    [Header("Correct Result")]
    public GameObject dirtObject;

    [Header("State")]
    public bool isDiscovered = false;
    public bool isCleared = false;

    public static int totalStains = 0;
    public static int inspectedStains = 0;

    private PopupFade popupFade;
    private bool popupOpen = false;
    private float popupJustClosedUntil = -1f;

    public bool IsWrongPopupOpen => popupOpen;

    public bool IsPopupRecentlyClosed()
    {
        return Time.time < popupJustClosedUntil;
    }

    void Awake()
    {
        if (wrongPopup != null)
            popupFade = wrongPopup.GetComponent<PopupFade>();
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

    void Update()
    {
        if (popupOpen && Input.GetKeyDown(KeyCode.E))
        {
            CloseWrongPopup();
        }
    }

    public void Inspect()
    {
        if (isCleared) return;

        if (!isDiscovered)
        {
            isDiscovered = true;
            inspectedStains++;
        }
    }

    public bool TryUseItem(ItemData item)
    {
        if (item == null) return false;
        if (isCleared) return false;
        if (!isDiscovered) return false;
        if (item.isUsed) return false;
        if (popupOpen) return false;

        bool isCorrect = item.itemName == requiredItemName;

        // ใช้ผิดหรือถูกก็นับจำนวนใช้
        item.UseOnce();

        if (isCorrect)
        {
            isCleared = true;

            if (dirtObject != null)
                dirtObject.SetActive(false);

            CloseWrongPopup();
        }
        else
        {
            ShowWrongPopup();
        }

        return true;
    }

    void ShowWrongPopup()
    {
        if (wrongPopupText != null)
            wrongPopupText.text = "อุปกรณ์ไม่ถูกต้อง\nการลองใช้ครั้งนี้นับจำนวนการใช้\nกด E เพื่อปิด";

        if (popupFade != null)
            popupFade.Show();
        else if (wrongPopup != null)
            wrongPopup.SetActive(true);

        popupOpen = true;
    }

    public void CloseWrongPopup()
    {
        if (popupFade != null)
            popupFade.Hide();
        else if (wrongPopup != null)
            wrongPopup.SetActive(false);

        popupOpen = false;

        // กัน E ตัวเดียวกันไปเปิดข้อมูลคราบต่อทันที
        popupJustClosedUntil = Time.time + 0.25f;
    }
}