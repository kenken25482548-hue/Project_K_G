using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InfoUIManager : MonoBehaviour
{
    public GameObject infoPanel;
    public TMP_Text infoText;
    public Image itemImage;

    bool isOpen = false;

    public void ToggleInfo(string text, Sprite image)
    {
        isOpen = !isOpen;

        infoPanel.SetActive(isOpen);

        if (isOpen)
        {
            infoText.text = text;
            itemImage.sprite = image;
        }
    }
}