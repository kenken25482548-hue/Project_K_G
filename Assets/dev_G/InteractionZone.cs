using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class InteractionZone : MonoBehaviour
{
    [Header("Interaction Range")]
    [Min(0.25f)]
    public float radius = 1.5f;

    [Header("Ring Visual")]
    public bool showRing = true;
    public Color ringColor = new Color(0.2f, 1f, 0.45f, 0.8f);
    [Min(0.005f)]
    public float ringWidth = 0.025f;
    [Range(12, 64)]
    public int ringSegments = 32;
    [Tooltip("ปรับลงค่าติดลบเพื่อวางวงไว้ที่พื้นใต้คราบหรือไอเทม")]
    public float ringHeightOffset = 0.03f;

    public bool IsPlayerInside { get; private set; }

    private SphereCollider zoneCollider;
    private LineRenderer ringRenderer;

    void Awake()
    {
        SetupZoneCollider();
        CreateRing();
    }

    void OnValidate()
    {
        SetupZoneCollider();

        if (ringRenderer != null)
            UpdateRing();
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
        IsPlayerInside = false;

        if (zoneCollider != null)
            zoneCollider.enabled = false;

        if (ringRenderer != null)
            ringRenderer.enabled = false;
    }

    void SetupZoneCollider()
    {
        zoneCollider = GetComponent<SphereCollider>();
        zoneCollider.isTrigger = true;
        zoneCollider.radius = radius;
    }

    void CreateRing()
    {
        GameObject ringObject = new GameObject("InteractionRing");
        ringObject.transform.SetParent(transform, false);
        ringRenderer = ringObject.AddComponent<LineRenderer>();
        ringRenderer.useWorldSpace = true;
        ringRenderer.loop = true;
        ringRenderer.textureMode = LineTextureMode.Stretch;
        ringRenderer.material = new Material(Shader.Find("Sprites/Default"));

        UpdateRing();
    }

    void UpdateRing()
    {
        if (ringRenderer == null) return;

        int segments = Mathf.Max(12, ringSegments);
        ringRenderer.positionCount = segments;
        ringRenderer.startWidth = ringWidth;
        ringRenderer.endWidth = ringWidth;
        ringRenderer.startColor = ringColor;
        ringRenderer.endColor = ringColor;
        ringRenderer.enabled = showRing;

        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            ringRenderer.SetPosition(
                i,
                transform.position + new Vector3(
                    Mathf.Cos(angle) * radius,
                    ringHeightOffset,
                    Mathf.Sin(angle) * radius
                )
            );
        }
    }
}
