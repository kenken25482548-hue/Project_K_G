using UnityEngine;

public class GameSFXManager : MonoBehaviour
{
    public static GameSFXManager Instance;

    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Player / UI")]
    public AudioClip pickupSfx;
    public AudioClip openInfoSfx;
    public AudioClip slotChangeSfx;

    [Header("Cleaning")]
    public AudioClip correctUseSfx;
    public AudioClip wrongUseSfx;
    public AudioClip closePopupSfx;

    [Header("Level")]
    public AudioClip successSfx;
    public AudioClip failSfx;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void Play(AudioClip clip, float volume = 1f)
    {
        if (audioSource == null || clip == null) return;
        audioSource.PlayOneShot(clip, volume);
    }

    public static void PlaySfx(AudioClip clip, float volume = 1f)
    {
        if (Instance == null) return;
        Instance.Play(clip, volume);
    }
}