using UnityEngine;
using System.Collections;

public class PopupFade : MonoBehaviour
{
    private CanvasGroup cg;

    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        if (cg == null)
            cg = gameObject.AddComponent<CanvasGroup>();
    }

    public void Show()
    {
        StopAllCoroutines();
        gameObject.SetActive(true);
        StartCoroutine(FadeIn());
    }

    public void Hide()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeIn()
    {
        cg.alpha = 0f;
        while (cg.alpha < 1f)
        {
            cg.alpha += Time.deltaTime * 5f;
            yield return null;
        }
        cg.alpha = 1f;
    }

    IEnumerator FadeOut()
    {
        while (cg.alpha > 0f)
        {
            cg.alpha -= Time.deltaTime * 5f;
            yield return null;
        }
        cg.alpha = 0f;
        gameObject.SetActive(false);
    }
}