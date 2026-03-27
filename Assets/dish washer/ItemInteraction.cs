using UnityEngine;

public class ItemInteraction : MonoBehaviour
{
    public GameObject interactUI;
    public GameObject infoPanel;

    bool playerInRange = false;
    bool infoOpen = false;

    void Update()
    {
        if (playerInRange)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Destroy(gameObject);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                infoOpen = !infoOpen;
                infoPanel.SetActive(infoOpen);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            interactUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            interactUI.SetActive(false);
            infoPanel.SetActive(false);
            infoOpen = false;
        }
    }
}
void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
    {
        Debug.Log("เข้าเขตไอเทมแล้ว");
        interactUI.SetActive(true);
    }
}