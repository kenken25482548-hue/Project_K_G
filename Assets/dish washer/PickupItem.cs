using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public GameObject item;

    public void Pick()
    {
        Destroy(item);
    }
}