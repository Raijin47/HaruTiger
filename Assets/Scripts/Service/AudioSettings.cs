using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public enum AudioSettingsType
{
    Slider, Image
}

public class AudioSettings : MonoBehaviour
{
    public AudioSettingsType Type;

    [SerializeField] private AudioMixerGroup _mixer;

    [SerializeField] private Slider _sliderMusic, _sliderSFX;

    [SerializeField] private Image _imageMusic, _imageSFX;

    private const float Min = -30;
    private const float Off = -80;

    private readonly string MusicSave = "MusicVolume";
    private readonly string Music = "Music";
    private readonly string SFXSave = "SFXSave";
    private readonly string SFX = "SFX";

    public void Init()
    {
        float musicVolume = PlayerPrefs.GetFloat(MusicSave, -10);
        float sfxVolume = PlayerPrefs.GetFloat(SFXSave, -10);

        switch (Type)
        {
            case AudioSettingsType.Slider:
                _sliderMusic.value = musicVolume;
                _sliderSFX.value = sfxVolume;
                break;
            case AudioSettingsType.Image:

                break;
        }


        ValueMusic(musicVolume);
        ValueSFX(sfxVolume);
    }

    public void ValueMusic(float volume)
    {
        float musicVolume = volume <= Min ? Off : volume;
        _mixer.audioMixer.SetFloat(Music, musicVolume);

        PlayerPrefs.SetFloat(MusicSave, musicVolume);
    }

    public void ValueSFX(float volume)
    {
        float sfxVolume = volume <= Min ? Off : volume;
        _mixer.audioMixer.SetFloat(SFX, sfxVolume);

        PlayerPrefs.SetFloat(SFXSave, sfxVolume);
    }
}