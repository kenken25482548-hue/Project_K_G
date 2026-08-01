using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    private const string NextSceneKey = "NextScene";
    private const int DefaultFirstLevelIndex = 2;

    [Header("UI")]
    public Slider loadingBar;
    public TMP_Text loadingText;

    void Start()
    {
        Time.timeScale = 1f;

        int nextScene = PlayerPrefs.GetInt(NextSceneKey, DefaultFirstLevelIndex);

        if (nextScene < 0 ||
            nextScene >= SceneManager.sceneCountInBuildSettings ||
            nextScene == SceneManager.GetActiveScene().buildIndex)
        {
            Debug.LogWarning(
                $"NextScene index {nextScene} ไม่ถูกต้อง ใช้ด่านแรก index {DefaultFirstLevelIndex} แทน");
            nextScene = DefaultFirstLevelIndex;
        }

        if (loadingBar != null)
            loadingBar.value = 0f;

        if (loadingText != null)
            loadingText.text = "กำลังโหลด... 0%";

        StartCoroutine(LoadLevel(nextScene));
    }

    IEnumerator LoadLevel(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        if (operation == null)
        {
            Debug.LogError($"ไม่สามารถโหลด Scene index {sceneIndex} ได้");
            yield break;
        }

        operation.allowSceneActivation = false;
        float displayedProgress = 0f;

        while (!operation.isDone)
        {
            float realProgress = Mathf.Clamp01(operation.progress / 0.9f);
            displayedProgress = Mathf.MoveTowards(
                displayedProgress,
                realProgress,
                Time.unscaledDeltaTime * 0.5f);

            if (loadingBar != null)
                loadingBar.value = displayedProgress;

            if (loadingText != null)
                loadingText.text = $"กำลังโหลด... {Mathf.RoundToInt(displayedProgress * 100f)}%";

            if (displayedProgress >= 1f)
            {
                yield return new WaitForSecondsRealtime(0.5f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
