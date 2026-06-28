using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("ใส่ชื่อ Scene ด่านแรก")]
    public string firstSceneName = "1bathroom1";

    public void StartGame()
    {
        SceneManager.LoadScene(firstSceneName);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}