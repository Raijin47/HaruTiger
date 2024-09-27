using System;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public abstract class AudioSettingsBase
{
    [SerializeField] protected AudioMixerGroup _mixer;

    protected const float Min = -30;
    protected const float On = 0;
    protected const float Off = -80;


    protected readonly string MusicSave = "MusicVolume";
    protected readonly string Music = "Music";
    protected readonly string SoundSave = "SoundSave";
    protected readonly string Sound = "Sound";

    public abstract void Init();
}