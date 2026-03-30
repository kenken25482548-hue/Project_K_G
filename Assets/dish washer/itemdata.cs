using UnityEngine;

public class ItemData : MonoBehaviour
{
    [Header("Info")]
    public string itemName;

    [TextArea(3, 5)]
    public string itemDescription;

    [Header("Image")]
    public Sprite itemSprite;

    [Header("Gameplay")]
    public bool isCorrectTool = false;

    [HideInInspector] public bool isPicked = false;
    [HideInInspector] public bool isUsed = false;

    private Collider[] colliders;
    private Renderer[] renderers;
    private Rigidbody rb;

    void Awake()
    {
        colliders = GetComponentsInChildren<Collider>(true);
        renderers = GetComponentsInChildren<Renderer>(true);
        rb = GetComponent<Rigidbody>();
    }

    public void Pick()
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

    public void Consume()
    {
        isPicked = false;
        isUsed = true;

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
}