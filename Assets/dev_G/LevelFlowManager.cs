using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelFlowManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject levelCompletePanel;
    public TMP_Text completeSubText;

    private bool levelCompleted = false;

    void Start()
    {
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(false);
    }

    void Update()
    {
        if (!levelCompleted)
        {
            CheckLevelComplete();
        }
        else
        {
            HandleLevelCompleteInput();
        }
    }

    void CheckLevelComplete()
    {
        CleaningTarget[] stains = FindObjectsOfType<CleaningTarget>(true);

        if (stains == null || stains.Length == 0) return;

        bool allCleared = true;
        int clearedCount = 0;

        for (int i = 0; i < stains.Length; i++)
        {
            if (stains[i] != null)
            {
                if (stains[i].isCleared)
                    clearedCount++;
                else
                    allCleared = false;
            }
        }

        if (allCleared)
        {
            levelCompleted = true;
            ShowLevelComplete(clearedCount, stains.Length);
        }
    }

    void ShowLevelComplete(int clearedCount, int totalCount)
    {
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(true);

        if (completeSubText != null)
            completeSubText.text = "ล้างคราบครบทั้งหมด " + clearedCount + " / " + totalCount + "\n\nกด N เพื่อไปด่านถัดไป\nกด R เพื่อเริ่มใหม่";
    }

    void HandleLevelCompleteInput()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            LoadNextLevel();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartLevel();
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