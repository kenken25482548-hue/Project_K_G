using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelFlowManager : MonoBehaviour
{
    [Header("UI - Success")]
    public GameObject levelCompletePanel;
    public TMP_Text completeSubText;

    [Header("UI - Fail")]
    public GameObject levelFailPanel;
    public TMP_Text failSubText;

    private bool levelEnded = false;
    private CleaningTarget[] stains;

    void Start()
    {
        stains = FindObjectsOfType<CleaningTarget>();

        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(false);

        if (levelFailPanel != null)
            levelFailPanel.SetActive(false);
    }

    void Update()
    {
        if (PauseMenuUI.IsPaused)
            return;

        if (levelEnded)
        {
            HandleEndInput();
            return;
        }

        CheckLevelState();
    }

    void CheckLevelState()
    {
        if (stains == null || stains.Length == 0) return;

        int totalCount = 0;
        int clearedCount = 0;

        for (int i = 0; i < stains.Length; i++)
        {
            if (stains[i] == null) continue;

            totalCount++;

            if (stains[i].isCleared)
                clearedCount++;
        }

        if (totalCount > 0 && clearedCount >= totalCount)
        {
            levelEnded = true;
            ShowLevelComplete(clearedCount, totalCount);
            return;
        }

        if (NoUsableItemsLeft() && clearedCount < totalCount)
        {
            levelEnded = true;
            ShowLevelFail(clearedCount, totalCount);
        }
    }

    bool NoUsableItemsLeft()
    {
        ItemData[] allItems = FindObjectsOfType<ItemData>(true);

        if (allItems == null || allItems.Length == 0)
            return true;

        for (int i = 0; i < allItems.Length; i++)
        {
            if (allItems[i] != null && allItems[i].HasUsesLeft())
                return false;
        }

        return true;
    }

    void ShowLevelComplete(int clearedCount, int totalCount)
    {
        // Keep progress independent from build indices: the launch intro scene is before the menu.
        string[] missionScenes = { "1bathroom1", "2Kitchen2", "3iving room3", "4bedroom4" };
        int missionIndex = System.Array.IndexOf(missionScenes, SceneManager.GetActiveScene().name);
        GameProgress.UnlockLevel(Mathf.Max(0, missionIndex));

        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(true);

        if (completeSubText != null)
            completeSubText.text = "ล้างคราบครบทั้งหมด " + clearedCount + " / " + totalCount + "\n\nกด N เพื่อไปด่านถัดไป\nกด R เพื่อเริ่มใหม่";

        GameSFXManager.PlaySfx(GameSFXManager.Instance != null ? GameSFXManager.Instance.successSfx : null, 1f);
    }

    void ShowLevelFail(int clearedCount, int totalCount)
    {
        if (levelFailPanel != null)
            levelFailPanel.SetActive(true);

        if (failSubText != null)
            failSubText.text = "ภารกิจไม่สำเร็จ\nล้างคราบได้ " + clearedCount + " / " + totalCount + "\nไอเทมหมดแล้ว\nกด R เพื่อเริ่มใหม่";

        GameSFXManager.PlaySfx(GameSFXManager.Instance != null ? GameSFXManager.Instance.failSfx : null, 1f);
    }

    void HandleEndInput()
    {
        if (levelCompletePanel != null && levelCompletePanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.N))
                LoadNextLevel();
        }

        if (Input.GetKeyDown(KeyCode.R))
            RestartLevel();
    }

    public void LoadNextLevel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            PlayerPrefs.SetInt("NextScene", nextIndex);
            PlayerPrefs.Save();
            SceneManager.LoadScene("9Loadingscene9");
        }
        else
        {
            SceneManager.LoadScene("Credits");
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
