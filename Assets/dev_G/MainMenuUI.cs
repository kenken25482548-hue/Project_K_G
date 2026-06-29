using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Scene")]
    public string firstSceneName = "1bathroom1";

    [Header("Sounds")]
    public AudioClip hoverSound;
    public AudioClip clickSound;
    public AudioClip bgmClip;

    private AudioSource audioSource;

    void Start()
    {
        // ปลดล็อก Cursor ทุกครั้งที่เข้า MainMenu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        audioSource = GetComponent<AudioSource>();

        // เปิดเพลง BGM
        if (bgmClip != null)
        {
            audioSource.clip = bgmClip;
            audioSource.loop = true;
            audioSource.volume = 0.5f;
            audioSource.Play();
        }

        // ผูกเสียง Hover ให้ทุกปุ่ม
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (Button btn in buttons)
        {
            AddHoverSound(btn);
        }
    }

    void AddHoverSound(Button btn)
    {
        EventTrigger trigger = btn.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = btn.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry hoverEntry = new EventTrigger.Entry();
        hoverEntry.eventID = EventTriggerType.PointerEnter;
        hoverEntry.callback.AddListener((data) => PlayHover());
        trigger.triggers.Add(hoverEntry);
    }

    public void PlayHover()
    {
        if (audioSource != null && hoverSound != null)
            audioSource.PlayOneShot(hoverSound, 0.7f);
    }

    public void StartGame()
    {
        PlayClick();
        Invoke(nameof(LoadScene), 0.2f);
    }

    public void QuitGame()
    {
        PlayClick();
        Invoke(nameof(DoQuit), 0.2f);
    }

    void PlayClick()
    {
        if (audioSource != null && clickSound != null)
            audioSource.PlayOneShot(clickSound, 1f);
    }

    void LoadScene()
    {
        SceneManager.LoadScene(firstSceneName);
    }

    void DoQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}