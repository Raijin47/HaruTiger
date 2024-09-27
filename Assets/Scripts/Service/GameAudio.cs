using System;
using UnityEngine;

[Serializable]
public class GameAudio
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _onClickClip;
    [SerializeField] private AudioClip _onWinClip, _onBonusClip, _onGameOverClip;

    public void Init()
    {
        Game.Action.OnWin += OnWin;
        Game.Action.OnBonus += OnBonus;
    }

    public void OnClick()
    {
        _audioSource.clip = _onClickClip;
        _audioSource.Play();
    }

    public void OnGameOver()
    {
        _audioSource.clip = _onGameOverClip;
        _audioSource.Play();
    }

    private void OnWin()
    {
        _audioSource.clip = _onWinClip;
        _audioSource.Play();
    }

    private void OnBonus()
    {
        _audioSource.clip = _onBonusClip;
        _audioSource.Play();
    }
}