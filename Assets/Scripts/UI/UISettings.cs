using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UISettings : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider mainVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider brightnesSlider;
    [SerializeField] private TMP_Text mainText;
    [SerializeField] private TMP_Text musicText;
    [SerializeField] private TMP_Text sfxText;
    [SerializeField] private TMP_Text brightnesText;

    private void Awake()
    {
        StartCoroutine(DelayAction(1f));
    }

    private IEnumerator DelayAction(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        UpdateSound(0.705407f, 0.3003435f);
        UpdateGraphic();
    }

    private void UpdateSound(float main = 1f, float music = 1f, float sfx = 1f)
    {
        if (PlayerPrefs.HasKey("masterVolume"))
            main = PlayerPrefs.GetFloat("masterVolume");
        mixer.SetFloat("master", Mathf.Log10(main) * 20);
        mainVolumeSlider.value = main;
        mainText.text = $"{(int)(main * 100)}%";

        if (PlayerPrefs.HasKey("musicVolume"))
            music = PlayerPrefs.GetFloat("musicVolume");
        mixer.SetFloat("music", Mathf.Log10(music) * 20);
        musicVolumeSlider.value = music;
        musicText.text = $"{(int)(music * 100)}%";

        if (PlayerPrefs.HasKey("sfxVolume"))
            sfx = PlayerPrefs.GetFloat("sfxVolume");
        mixer.SetFloat("sfx", Mathf.Log10(sfx) * 20);
        sfxVolumeSlider.value = sfx;
        sfxText.text = $"{(int)(sfx * 100)}%";
    }

    private void UpdateGraphic()
    {
        float brightness = 1f;
        if (PlayerPrefs.HasKey("brightness"))
            brightness = PlayerPrefs.GetFloat("brightness");
        brightnesSlider.value = brightness;
        brightnesText.text = $"{(int)(brightness * 100)}%";
        GameManager.Instance.SetVolumeSetting(brightness);
    }

    //public void SetActiveForElements(bool isActive) => brightnesSlider.enabled = sfxVolumeSlider.enabled = musicVolumeSlider.enabled = mainVolumeSlider.enabled = isActive;

    public void SetMainVolume()
    {
        float volume = mainVolumeSlider.value;
        mixer.SetFloat("master", Mathf.Log10(volume) * 20);
        mainText.text = $"{(int)(volume * 100)}%";
        PlayerPrefs.SetFloat("masterVolume", volume);
    }
    public void SetMusicVolume()
    {
        float volume = musicVolumeSlider.value;
        mixer.SetFloat("music", Mathf.Log10(volume) * 20);
        musicText.text = $"{(int)(volume * 100)}%";
        PlayerPrefs.SetFloat("musicVolume", volume);
    }
    public void SetSFXVolume()
    {
        float volume = sfxVolumeSlider.value;
        mixer.SetFloat("sfx", Mathf.Log10(volume) * 20);
        sfxText.text = $"{(int)(volume * 100)}%";
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }
    public void SetBrightnes()
    {
        float brightness = brightnesSlider.value < 0.15f ? 0.15f : brightnesSlider.value;
        GameManager.Instance.SetVolumeSetting(brightness);
        PlayerPrefs.SetFloat("brightness", brightness);
        brightnesText.text = $"{(int)(brightness * 100)}%";
    }
}
