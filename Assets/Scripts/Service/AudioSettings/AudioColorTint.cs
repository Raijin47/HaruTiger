using System;
using UnityEngine;

[Serializable]
public class AudioColorTint : AudioSettings
{
    [SerializeField] private Sprite _onSprite;
    [SerializeField] private Sprite _offSprite;

    public Sprite OnSprite => _onSprite;
    public Sprite OffSprite => _offSprite;

    private bool _soundValue;
    private bool _musicValue;

    public override void Init()
    {
        SoundValue = PlayerPrefs.GetInt(SoundSave, 1) == 1;
        MusicValue = PlayerPrefs.GetInt(MusicSave, 1) == 1;
    }

    public bool SoundValue
    {
        get => _soundValue;
        set
        {
            _soundValue = value;
            _mixer.audioMixer.SetFloat(Sound, value ? On : Off);
            PlayerPrefs.SetInt(SoundSave, value ? 1 : 0);
        }
    }

    public bool MusicValue
    {
        get => _musicValue;
        set
        {
            _musicValue = value;
            _mixer.audioMixer.SetFloat(Music, value ? On : Off);
            PlayerPrefs.SetInt(MusicSave, value ? 1 : 0);
        }
    }
}