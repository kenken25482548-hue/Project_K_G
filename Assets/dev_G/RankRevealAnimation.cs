using TMPro;
using UnityEngine;

/// <summary>Small unscaled reveal used by the end-of-level clear rank.</summary>
public class RankRevealAnimation : MonoBehaviour
{
    private float elapsed;
    private TMP_Text label;
    private int rank;

    public void Configure(int value)
    {
        rank = value;
        label = GetComponent<TMP_Text>();
        transform.localScale = Vector3.one * 0.65f;
    }

    private void Update()
    {
        elapsed += Time.unscaledDeltaTime;
        float reveal = Mathf.SmoothStep(0.65f, 1f, Mathf.Clamp01(elapsed / 0.4f));
        float pulse = rank == 3 ? 1f + Mathf.Sin(elapsed * 5f) * 0.045f : 1f;
        transform.localScale = Vector3.one * reveal * pulse;
        if (label != null && rank == 3)
            label.color = Color.Lerp(new Color(0.49f, 0.90f, 1f, 1f), Color.white, (Mathf.Sin(elapsed * 5f) + 1f) * 0.25f);
    }
}
