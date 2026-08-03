using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class CleaningTarget : MonoBehaviour
{
    [Header("Stain Info")]
    public string stainName;

    [TextArea(2, 5)]
    public string stainDescription;

    [Header("Required Item")]
    public string requiredItemName;

    [Header("Wrong Popup")]
    public GameObject wrongPopup;
    public TMP_Text wrongPopupText;

    [Header("Correct Result")]
    public GameObject dirtObject;

    [Header("Clear Visual")]
    [Min(0.05f)] public float clearFadeDuration = 0.55f;

    [Header("State")]
    public bool isDiscovered = false;
    public bool isCleared = false;

    public static int totalStains = 0;
    public static int inspectedStains = 0;

    private PopupFade popupFade;
    private bool popupOpen = false;
    private float popupJustClosedUntil = -1f;

    public bool IsWrongPopupOpen => popupOpen;

    public bool IsPopupRecentlyClosed()
    {
        return Time.time < popupJustClosedUntil;
    }

    void Awake()
    {
        if (wrongPopup != null)
            popupFade = wrongPopup.GetComponent<PopupFade>();
    }

    void OnEnable()
    {
        totalStains++;
    }

    void OnDisable()
    {
        totalStains = Mathf.Max(0, totalStains - 1);

        if (isDiscovered)
            inspectedStains = Mathf.Max(0, inspectedStains - 1);
    }

    void Update()
    {
        if (PauseMenuUI.IsPaused)
            return;

        if (popupOpen && Input.GetKeyDown(KeyCode.E))
        {
            CloseWrongPopup();
        }
    }

    public void Inspect()
    {
        if (isCleared) return;

        if (!isDiscovered)
        {
            isDiscovered = true;
            inspectedStains++;
        }
    }

    public bool TryUseItem(ItemData item)
    {
        if (item == null) return false;
        if (isCleared) return false;
        if (!isDiscovered) return false;
        if (item.isUsed) return false;
        if (popupOpen) return false;

        bool isCorrect = item.itemName == requiredItemName;

        item.UseOnce();

        if (isCorrect)
        {
            isCleared = true;

            HideDirtVisual();

            InteractionZone interactionZone = GetComponent<InteractionZone>();
            if (interactionZone != null)
                interactionZone.DisableZone();

            GameSFXManager.PlayUseFeedback(GameSFXManager.Instance != null ? GameSFXManager.Instance.correctUseSfx : null);

            if (popupOpen)
                CloseWrongPopup();
        }
        else
        {
            GameSFXManager.PlayUseFeedback(GameSFXManager.Instance != null ? GameSFXManager.Instance.wrongUseSfx : null);
            ShowWrongPopup();
        }

        return true;
    }

    void HideDirtVisual()
    {
        StainMarkerBlueZone[] stainMarkers = GetComponentsInChildren<StainMarkerBlueZone>(true);
        for (int i = 0; i < stainMarkers.Length; i++)
            stainMarkers[i].gameObject.SetActive(false);

        if (dirtObject == null)
            return;

        StartCoroutine(FadeAndHideDirt(dirtObject));
    }

    IEnumerator FadeAndHideDirt(GameObject target)
    {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>(true);
        UnityEngine.Rendering.Universal.DecalProjector[] decals =
            target.GetComponentsInChildren<UnityEngine.Rendering.Universal.DecalProjector>(true);
        ParticleSystem[] particles = target.GetComponentsInChildren<ParticleSystem>(true);

        List<Material> fadeMaterials = new List<Material>();
        List<Color> originalColors = new List<Color>();
        for (int i = 0; i < renderers.Length; i++)
        {
            foreach (Material material in renderers[i].materials)
            {
                if (material == null) continue;
                if (material.HasProperty("_BaseColor"))
                {
                    fadeMaterials.Add(material);
                    originalColors.Add(material.GetColor("_BaseColor"));
                }
                else if (material.HasProperty("_Color"))
                {
                    fadeMaterials.Add(material);
                    originalColors.Add(material.GetColor("_Color"));
                }
            }
        }

        float elapsed = 0f;
        while (elapsed < clearFadeDuration)
        {
            float alpha = 1f - elapsed / clearFadeDuration;
            for (int i = 0; i < decals.Length; i++)
                if (decals[i] != null) decals[i].fadeFactor = alpha;
            for (int i = 0; i < fadeMaterials.Count; i++)
            {
                Color faded = originalColors[i];
                faded.a *= alpha;
                if (fadeMaterials[i].HasProperty("_BaseColor")) fadeMaterials[i].SetColor("_BaseColor", faded);
                else fadeMaterials[i].SetColor("_Color", faded);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < particles.Length; i++)
            if (particles[i] != null) particles[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        for (int i = 0; i < decals.Length; i++)
            if (decals[i] != null) decals[i].enabled = false;
        for (int i = 0; i < renderers.Length; i++)
            if (renderers[i] != null) renderers[i].enabled = false;

        if (target != gameObject)
            target.SetActive(false);
    }

    void ShowWrongPopup()
    {
        if (wrongPopup != null)
            wrongPopup.SetActive(true); // ← เพิ่มบรรทัดนี้

        if (wrongPopupText != null)
            wrongPopupText.text = "ใช้ไอเทมไม่ถูกต้อง\nการลองใช้ครั้งนี้นับจำนวนการใช้\nกด E เพื่อปิด";

        if (popupFade != null)
            popupFade.Show();
        else if (wrongPopup != null)
            wrongPopup.SetActive(true);

        popupOpen = true;
    }

    public void CloseWrongPopup()
    {
        if (popupOpen)
        {
            GameSFXManager.PlaySfx(GameSFXManager.Instance != null ? GameSFXManager.Instance.closePopupSfx : null, 0.9f);
        }

        if (popupFade != null)
            popupFade.Hide();
        else if (wrongPopup != null)
            wrongPopup.SetActive(false);

        popupOpen = false;
        popupJustClosedUntil = Time.time + 0.25f;
    }
}
