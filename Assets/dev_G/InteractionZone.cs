using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class InteractionZone : MonoBehaviour
{
    public enum VisualStyle
    {
        Automatic,
        StainEffect,
        ItemBeacon,
        Hidden
    }

    [Header("Interaction Range")]
    [Min(0.25f)]
    public float radius = 1.5f;

    [Header("Visual Style")]
    public VisualStyle visualStyle = VisualStyle.Hidden;

    [Header("Stain Electric Ring")]
    [Min(0.2f)]
    public float stainEffectDiameter = 0.75f;
    public float stainEffectHeight = 0.035f;
    [Range(0.25f, 1f)]
    public float stainEffectOpacity = 0.9f;

    [Header("Item Helper")]
    public Color itemBeaconColor = new Color(1f, 0.82f, 0.12f, 0.9f);
    [Min(0.2f)]
    public float itemBeaconHeight = 1.25f;

    public bool IsPlayerInside { get; private set; }

    private SphereCollider zoneCollider;
    private GameObject visualRoot;
    private SpriteRenderer stainRingRenderer;
    private LineRenderer itemBeam;
    private LineRenderer itemMarker;
    private bool isDisabled;

    void Awake()
    {
        SetupZoneCollider();
        CreateVisual();
    }

    void OnValidate()
    {
        SetupZoneCollider();
    }

    void LateUpdate()
    {
        if (isDisabled || visualRoot == null)
            return;

        if (GetActiveVisualStyle() == VisualStyle.StainEffect)
            UpdateStainEffect();
        else if (GetActiveVisualStyle() == VisualStyle.ItemBeacon)
            UpdateItemBeacon();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerItemSystem>() != null)
            IsPlayerInside = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<PlayerItemSystem>() != null)
            IsPlayerInside = false;
    }

    public void DisableZone()
    {
        isDisabled = true;
        IsPlayerInside = false;

        if (zoneCollider != null)
            zoneCollider.enabled = false;

        if (visualRoot != null)
            visualRoot.SetActive(false);
    }

    public bool IsPositionInside(Vector3 position)
    {
        Vector3 horizontalOffset = position - GetInteractionPosition();
        horizontalOffset.y = 0f;
        return horizontalOffset.sqrMagnitude <= radius * radius;
    }

    void SetupZoneCollider()
    {
        zoneCollider = GetComponent<SphereCollider>();
        zoneCollider.isTrigger = true;
        zoneCollider.radius = radius;
    }

    VisualStyle GetActiveVisualStyle()
    {
        if (visualStyle != VisualStyle.Automatic)
            return visualStyle;

        if (GetComponent<CleaningTarget>() != null)
            return VisualStyle.StainEffect;

        if (GetComponent<ItemData>() != null)
            return VisualStyle.ItemBeacon;

        return VisualStyle.Hidden;
    }

    void CreateVisual()
    {
        if (GetActiveVisualStyle() == VisualStyle.Hidden)
            return;

        visualRoot = new GameObject("InteractionVisual");
        visualRoot.transform.SetParent(transform, false);

        if (GetActiveVisualStyle() == VisualStyle.StainEffect)
            CreateStainEffect();
        else
            CreateItemBeacon();
    }

    void CreateStainEffect()
    {
        Texture2D texture = Resources.Load<Texture2D>("Effects/StainElectricRing");
        if (texture == null)
        {
            Debug.LogWarning("Missing Effects/StainElectricRing texture.");
            return;
        }

        GameObject ringObject = new GameObject("StainElectricRing");
        ringObject.transform.SetParent(visualRoot.transform, false);

        stainRingRenderer = ringObject.AddComponent<SpriteRenderer>();
        stainRingRenderer.sprite = Sprite.Create(
            texture,
            new Rect(0f, 0f, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            512f
        );
        stainRingRenderer.sortingOrder = 20;
        stainRingRenderer.color = new Color(1f, 1f, 1f, stainEffectOpacity);

        UpdateStainEffect();
    }

    void CreateItemBeacon()
    {
        itemBeam = CreateLine("ItemBeaconBeam", 0.035f, false, itemBeaconColor);
        itemBeam.positionCount = 2;

        itemMarker = CreateLine("ItemBeaconMarker", 0.035f, true, itemBeaconColor);
        itemMarker.positionCount = 4;

        UpdateItemBeacon();
    }

    LineRenderer CreateLine(string objectName, float width, bool loop, Color color)
    {
        GameObject lineObject = new GameObject(objectName);
        lineObject.transform.SetParent(visualRoot.transform, false);

        LineRenderer line = lineObject.AddComponent<LineRenderer>();
        line.useWorldSpace = true;
        line.loop = loop;
        line.startWidth = width;
        line.endWidth = width;
        line.startColor = color;
        line.endColor = color;
        line.material = new Material(Shader.Find("Sprites/Default"));
        return line;
    }

    void UpdateStainEffect()
    {
        if (stainRingRenderer == null)
            return;

        float pulse = 1f + Mathf.Sin(Time.time * 3f) * 0.06f;
        Vector3 position = GetInteractionPosition() + Vector3.up * stainEffectHeight;

        stainRingRenderer.transform.position = position;
        stainRingRenderer.transform.rotation = Quaternion.Euler(90f, 0f, Time.time * 10f);

        float spriteWidth = stainRingRenderer.sprite.bounds.size.x;
        float scale = stainEffectDiameter / spriteWidth * pulse;
        stainRingRenderer.transform.localScale = new Vector3(scale, scale, 1f);
        stainRingRenderer.color = new Color(1f, 1f, 1f, stainEffectOpacity * (0.85f + Mathf.Sin(Time.time * 4f) * 0.15f));
    }

    void UpdateItemBeacon()
    {
        if (itemBeam == null || itemMarker == null)
            return;

        float bob = Mathf.Sin(Time.time * 3f) * 0.08f;
        Vector3 basePosition = GetInteractionPosition() + Vector3.up * 0.12f;
        Vector3 markerPosition = GetInteractionPosition() + Vector3.up * (itemBeaconHeight + bob);

        itemBeam.SetPosition(0, basePosition);
        itemBeam.SetPosition(1, markerPosition);

        float markerSize = 0.16f;
        itemMarker.SetPosition(0, markerPosition + new Vector3(0f, markerSize, 0f));
        itemMarker.SetPosition(1, markerPosition + new Vector3(markerSize, 0f, 0f));
        itemMarker.SetPosition(2, markerPosition + new Vector3(0f, -markerSize, 0f));
        itemMarker.SetPosition(3, markerPosition + new Vector3(-markerSize, 0f, 0f));
    }

    Vector3 GetInteractionPosition()
    {
        CleaningTarget stain = GetComponent<CleaningTarget>();
        if (stain != null && stain.dirtObject != null)
            return stain.dirtObject.transform.position;

        return transform.position;
    }
}
