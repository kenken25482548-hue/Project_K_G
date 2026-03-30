using UnityEngine;

public class ItemData : MonoBehaviour
{
    [Header("Info")]
    public string itemName;
    [TextArea(3, 5)]
    public string itemDescription;

    [Header("Drop")]
    public Transform dropPoint;

    [Header("Optional Image")]
    public Sprite itemSprite;

    [HideInInspector] public bool isPicked = false;

    private Collider[] colliders;
    private Renderer[] renderers;
    private Rigidbody rb;

    void Awake()
    {
        colliders = GetComponentsInChildren<Collider>(true);
        renderers = GetComponentsInChildren<Renderer>(true);
        rb = GetComponent<Rigidbody>();
    }

    public void HideItem()
    {
        isPicked = true;

        foreach (Renderer r in renderers)
            r.enabled = false;

        foreach (Collider c in colliders)
            c.enabled = false;

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public void ShowAtDropPoint()
    {
        if (dropPoint == null) return;

        transform.position = dropPoint.position;
        transform.rotation = dropPoint.rotation;

        foreach (Renderer r in renderers)
            r.enabled = true;

        foreach (Collider c in colliders)
            c.enabled = true;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        isPicked = false;
    }
}