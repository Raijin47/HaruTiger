using System;
using UnityEngine;

[Serializable]
public class GameAudio
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _onClickClip;
    [SerializeField] private AudioClip[] _clips;

    public void OnClick()
    {
        _audioSource.clip = _onClickClip;
        _audioSource.Play();
    }

    public void PlayClip(int id)
    {
        _audioSource.clip = _clips[id];
        _audioSource.Play();
    }
}