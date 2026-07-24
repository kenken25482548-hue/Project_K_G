using UnityEngine;
using TMPro;

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

            GameSFXManager.PlaySfx(GameSFXManager.Instance != null ? GameSFXManager.Instance.correctUseSfx : null, 1f);

            if (popupOpen)
                CloseWrongPopup();
        }
        else
        {
            GameSFXManager.PlaySfx(GameSFXManager.Instance != null ? GameSFXManager.Instance.wrongUseSfx : null, 1f);
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

        if (dirtObject != gameObject)
        {
            dirtObject.SetActive(false);
            return;
        }

        Renderer[] dirtRenderers = dirtObject.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < dirtRenderers.Length; i++)
            dirtRenderers[i].enabled = false;

        ParticleSystem[] dirtParticles = dirtObject.GetComponentsInChildren<ParticleSystem>(true);
        for (int i = 0; i < dirtParticles.Length; i++)
            dirtParticles[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        UnityEngine.Rendering.Universal.DecalProjector[] dirtDecals =
            dirtObject.GetComponentsInChildren<UnityEngine.Rendering.Universal.DecalProjector>(true);

        for (int i = 0; i < dirtDecals.Length; i++)
            dirtDecals[i].enabled = false;
    }

    void ShowWrongPopup()
    {
        if (wrongPopup != null)
            wrongPopup.SetActive(true); // ← เพิ่มบรรทัดนี้

        if (wrongPopupText != null)
            wrongPopupText.text = "อุปกรณ์ไม่ถูกต้อง\nการลองใช้ครั้งนี้นับจำนวนการใช้\nกด E เพื่อปิด";

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
