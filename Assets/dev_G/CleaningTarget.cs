using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CleaningTarget : MonoBehaviour
{
    [Header("Stain Info")]
    public string stainName;

    [TextArea(3, 5)]
    public string stainDescription;

    [Header("Required Item (Hidden from player)")]
    public string requiredItemName;

    [Header("Wrong Popup")]
    public GameObject wrongPopup;
    public TMP_Text wrongPopupText;

    [Header("Correct Result")]
    public GameObject dirtObject;

    [Header("State")]
    public bool isDiscovered = false;
    public bool isCleared = false;

    private PopupFade popupFade;
    private bool popupOpen = false;

    void Awake()
    {
        if (wrongPopup != null)
            popupFade = wrongPopup.GetComponent<PopupFade>();
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
        isDiscovered = true;
    }

    public bool TryUseItem(ItemData item)
    {
        if (item == null) return false;
        if (isCleared) return false;
        if (!isDiscovered) return false;
        if (item.isUsed) return false;

        bool isCorrect = item.itemName == requiredItemName;

        // แบบ B = ใช้ถูกหรือผิดก็ลดจำนวนใช้
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

        CheckLevelFailCondition();
        return true;
    }

    private void ShowWrongPopup()
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
    }

    private void CheckLevelFailCondition()
    {
        CleaningTarget[] allTargets = FindObjectsByType<CleaningTarget>(FindObjectsSortMode.None);

        bool hasUnclearedStains = false;
        foreach (CleaningTarget target in allTargets)
        {
            if (target != null && !target.isCleared)
            {
                hasUnclearedStains = true;
                break;
            }
        }

        if (!hasUnclearedStains)
            return;

        ItemData[] allItems = FindObjectsByType<ItemData>(FindObjectsSortMode.None);

        bool hasUsableItemLeft = false;
        foreach (ItemData data in allItems)
        {
            if (data != null && data.HasUsesLeft())
            {
                hasUsableItemLeft = true;
                break;
            }
        }

        if (!hasUsableItemLeft)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}