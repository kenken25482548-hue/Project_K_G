using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonHoverMotion :
    MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    ISelectHandler,
    IDeselectHandler
{
    [SerializeField] private Vector2 hoverOffset = new Vector2(16f, 0f);
    [SerializeField] private float hoverScale = 1.025f;
    [SerializeField] private float animationSpeed = 15f;
    [SerializeField] private float pressedScale = 0.985f;

    private RectTransform rectTransform;
    private RectTransform accent;
    private Vector2 restPosition;
    private Vector2 restAccentSize;
    private bool pointerInside;
    private bool selected;
    private bool pressed;

    void Awake()
    {
        CacheReferences();
    }

    public void Configure(Vector2 offset, float scale, float speed)
    {
        CacheReferences();
        hoverOffset = offset;
        hoverScale = scale;
        animationSpeed = speed;
        restPosition = rectTransform.anchoredPosition;

        if (accent != null)
            restAccentSize = accent.sizeDelta;
    }

    void Update()
    {
        if (rectTransform == null)
            return;

        bool highlighted = pointerInside || selected;
        Vector2 targetPosition = restPosition + (highlighted ? hoverOffset : Vector2.zero);

        float targetScale = 1f;
        if (pressed)
            targetScale = pressedScale;
        else if (highlighted)
            targetScale = hoverScale;

        float blend = 1f - Mathf.Exp(-animationSpeed * Time.unscaledDeltaTime);
        rectTransform.anchoredPosition = Vector2.Lerp(
            rectTransform.anchoredPosition,
            targetPosition,
            blend);
        rectTransform.localScale = Vector3.Lerp(
            rectTransform.localScale,
            Vector3.one * targetScale,
            blend);

        if (accent != null)
        {
            Vector2 targetAccentSize = restAccentSize;
            targetAccentSize.x = highlighted ? restAccentSize.x + 5f : restAccentSize.x;
            accent.sizeDelta = Vector2.Lerp(accent.sizeDelta, targetAccentSize, blend);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerInside = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerInside = false;
        pressed = false;

        // เมาส์ออกจากปุ่มแล้วให้คืนตำแหน่ง แม้ปุ่มจะยังเป็น Selected ของ EventSystem
        selected = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pressed = false;
    }

    public void OnSelect(BaseEventData eventData)
    {
        selected = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        selected = false;
        pressed = false;
    }

    void OnDisable()
    {
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = restPosition;
            rectTransform.localScale = Vector3.one;
        }

        if (accent != null)
            accent.sizeDelta = restAccentSize;

        pointerInside = false;
        selected = false;
        pressed = false;
    }

    void CacheReferences()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
            if (rectTransform != null)
                restPosition = rectTransform.anchoredPosition;
        }

        if (accent == null)
        {
            Transform accentTransform = transform.Find("Accent");
            if (accentTransform != null)
            {
                accent = accentTransform as RectTransform;
                if (accent != null)
                    restAccentSize = accent.sizeDelta;
            }
        }
    }
}
