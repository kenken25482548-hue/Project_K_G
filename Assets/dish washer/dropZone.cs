using UnityEngine;

public class DropZone : MonoBehaviour
{
    public static bool playerInDropZone = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInDropZone = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInDropZone = false;
        }
    }
}