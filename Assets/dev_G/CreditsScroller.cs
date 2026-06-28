using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class CreditsScroller : MonoBehaviour
{
    [Header("UI")]
    public RectTransform creditsText;
    public Button backButton;

    [Header("Settings")]
    public float scrollSpeed = 80f;
    public float endY = 1200f;

    void Start()
    {
        if (backButton != null)
            backButton.onClick.AddListener(GoToMainMenu);
    }

    void Update()
    {
        if (creditsText == null) return;

        creditsText.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

        if (creditsText.anchoredPosition.y >= endY)
        {
            GoToMainMenu();
        }
    }

    void GoToMainMenu()
    {
        SceneManager.LoadScene("0Mainmenu0");
    }
}