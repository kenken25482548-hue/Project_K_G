using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider sfxSlider;

    void Start()
    {
        if (GameSFXManager.Instance == null) return;

        if (bgmSlider != null)
        {
            bgmSlider.value = GameSFXManager.Instance.GetBGMVolume();
            bgmSlider.onValueChanged.AddListener(GameSFXManager.Instance.SetBGMVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = GameSFXManager.Instance.GetSFXVolume();
            sfxSlider.onValueChanged.AddListener(GameSFXManager.Instance.SetSFXVolume);
        }
    }
}