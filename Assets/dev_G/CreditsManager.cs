using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour
{
    public float autoReturnTime = 10f; // กลับ MainMenu อัตโนมัติกี่วิ

    void Start()
    {
        // ปลดล็อก Cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Invoke(nameof(GoToMainMenu), autoReturnTime);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.Escape))
        {
            GoToMainMenu();
        }
    }

    public void GoToMainMenu()
    {
        // ปลดล็อก Cursor ก่อนกลับ
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("0Mainmenu0");
    }
}