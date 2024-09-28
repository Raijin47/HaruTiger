using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class AudioSlider : AudioSettings
{
    [SerializeField] private Slider _sliderMusic, _sliderSFX;

    public override void Init()
    {
        float musicVolume = PlayerPrefs.GetFloat(MusicSave, -10);
        float sfxVolume = PlayerPrefs.GetFloat(SoundSave, -10);

        _sliderMusic.value = musicVolume;
        _sliderSFX.value = sfxVolume;

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
        _mixer.audioMixer.SetFloat(Sound, sfxVolume);

        PlayerPrefs.SetFloat(SoundSave, sfxVolume);
    }
}