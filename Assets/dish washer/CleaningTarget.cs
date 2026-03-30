using UnityEngine;
using TMPro;

public class CleaningTarget : MonoBehaviour
{
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

        if (item.isCorrectTool)
        {
            isCleared = true;

            if (dirtObject != null)
                dirtObject.SetActive(false);

            item.Consume();
            CloseWrongPopup();
        }
        else
        {
            item.Consume();

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