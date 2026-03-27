using UnityEngine;

public class ItemInfo : MonoBehaviour
{
    public GameObject infoPanel;

    public void ShowInfo()
    {
        infoPanel.SetActive(true);
    }
}