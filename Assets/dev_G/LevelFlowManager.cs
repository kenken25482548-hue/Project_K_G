using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFlowManager : MonoBehaviour
{
    [Header("Optional")]
    public GameObject levelCompletePanel;

    private bool levelCompleted = false;

    void Update()
    {
        if (levelCompleted) return;

        CleaningTarget[] stains = FindObjectsOfType<CleaningTarget>(true);

        if (stains.Length == 0) return;

        bool allCleared = true;

        for (int i = 0; i < stains.Length; i++)
        {
            if (stains[i] != null && !stains[i].isCleared)
            {
                allCleared = false;
                break;
            }
        }

        if (allCleared)
        {
            levelCompleted = true;

            if (levelCompletePanel != null)
                levelCompletePanel.SetActive(true);
        }
    }

    public void LoadNextLevel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.Log("ไม่มีด่านถัดไปแล้ว");
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}