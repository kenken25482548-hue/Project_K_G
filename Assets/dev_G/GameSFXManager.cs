using UnityEngine;
using UnityEngine.Audio;

public class GameSFXManager : MonoBehaviour
{
    public static GameSFXManager Instance;

    [Header("Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("SFX Clips")]
    public AudioClip pickupSfx;
    public AudioClip openInfoSfx;
    public AudioClip slotChangeSfx;
    public AudioClip correctUseSfx;
    public AudioClip wrongUseSfx;
    public AudioClip closePopupSfx;
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
    }

    public static void PlaySfx(AudioClip clip, float volume = 1f)
    {
        if (Instance == null || Instance.sfxSource == null || clip == null) return;
        Instance.sfxSource.PlayOneShot(clip, volume);
    }

    public void SetBGMVolume(float value)
    {
        if (bgmSource != null)
            bgmSource.volume = value;

        PlayerPrefs.SetFloat("BGMVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        if (sfxSource != null)
            sfxSource.volume = value;

        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public float GetBGMVolume()
    {
        return PlayerPrefs.GetFloat("BGMVolume", 0.12f);
    }

    public float GetSFXVolume()
    {
        return PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

    void Start()
    {
        SetBGMVolume(GetBGMVolume());
        SetSFXVolume(GetSFXVolume());
    }
}