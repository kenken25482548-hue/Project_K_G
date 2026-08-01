using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance;

    [Header("Loading Screen")]
    public GameObject loadingScreen;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadLevel(string sceneName, int levelIndex)
    {
        // บันทึกว่าผ่านด่านนี้แล้ว
        GameProgress.UnlockLevel(levelIndex);
        StartCoroutine(LoadAsync(sceneName));
    }

    public void LoadNextLevel(int currentLevelIndex)
    {
        GameProgress.UnlockLevel(currentLevelIndex);
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            StartCoroutine(LoadAsync(nextIndex));
        }
        else
        {
            // จบเกมแล้ว ไป Credits
            SceneManager.LoadScene("Credits");
        }
    }

    IEnumerator LoadAsync(string sceneName)
    {
        if (loadingScreen != null)
            loadingScreen.SetActive(true);

        yield return new WaitForSeconds(1f);

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);

        while (!op.isDone)
            yield return null;
    }

    IEnumerator LoadAsync(int sceneIndex)
    {
        if (loadingScreen != null)
            loadingScreen.SetActive(true);

        yield return new WaitForSeconds(1f);

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneIndex);

        while (!op.isDone)
            yield return null;
    }
}