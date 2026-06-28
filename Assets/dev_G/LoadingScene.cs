using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class LoadingScene : MonoBehaviour
{
    [Header("UI")]
    public Slider loadingBar;
    public TMP_Text loadingText;

    void Start()
    {
        int nextScene = PlayerPrefs.GetInt("NextScene", 2);
        StartCoroutine(LoadLevel(nextScene));
    }

    IEnumerator LoadLevel(int sceneIndex)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneIndex);
        op.allowSceneActivation = false;

        float fakeProgress = 0f;

        while (!op.isDone)
        {
            float realProgress = Mathf.Clamp01(op.progress / 0.9f);
            fakeProgress = Mathf.MoveTowards(fakeProgress, realProgress, Time.deltaTime * 0.5f);

            if (loadingBar != null)
                loadingBar.value = fakeProgress;

            if (loadingText != null)
                loadingText.text = "กำลังโหลด... " + Mathf.Round(fakeProgress * 100) + "%";

            if (fakeProgress >= 1f)
            {
                yield return new WaitForSeconds(0.5f);
                op.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}