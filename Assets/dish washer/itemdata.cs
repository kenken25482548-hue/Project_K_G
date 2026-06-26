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

    private Vector3 startPosition;
    private Quaternion startRotation;

    void Awake()
    {
        colliders = GetComponentsInChildren<Collider>(true);
        renderers = GetComponentsInChildren<Renderer>(true);
        rb = GetComponent<Rigidbody>();

        startPosition = transform.position;
        startRotation = transform.rotation;
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

    public void ReturnToStart()
    {
        isPicked = false;

        transform.position = startPosition;
        transform.rotation = startRotation;

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
    }

    public void Consume()
    {
        isPicked = false;
        isUsed = true;

        gameObject.SetActive(false);
    }
}