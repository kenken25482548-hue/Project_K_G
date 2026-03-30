using UnityEngine;
using TMPro;

public class CleaningTarget : MonoBehaviour
{
    [Header("Required Item")]
    public string requiredItemName;

    [Header("Wrong Popup")]
    public GameObject wrongPopup;
    public TMP_Text wrongPopupText;

    [Header("Correct Result")]
    public GameObject dirtObject;

    [Header("State")]
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

    public void TryUseItem(ItemData item)
    {
        if (item == null || isCleared) return;

        bool isCorrect = item.itemName == requiredItemName;

        if (isCorrect)
        {
            isCleared = true;

            if (dirtObject != null)
                dirtObject.SetActive(false);

            item.Consume();
            CloseWrongPopup();
        }
        else
        {
            item.ReturnToStart();

            if (wrongPopupText != null)
                wrongPopupText.text = "อุปกรณ์ไม่ถูกต้อง\nกด E เพื่อปิด";

            if (popupFade != null)
                popupFade.Show();
            else if (wrongPopup != null)
                wrongPopup.SetActive(true);

            popupOpen = true;
        }
    }

    public void CloseWrongPopup()
    {
        if (popupFade != null)
            popupFade.Hide();
        else if (wrongPopup != null)
            wrongPopup.SetActive(false);

        popupOpen = false;
    }
}