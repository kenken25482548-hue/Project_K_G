using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        // ปลดล็อก Cursor ตั้งแต่เข้าหน้า Credits
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("0Mainmenu0");
    }
}