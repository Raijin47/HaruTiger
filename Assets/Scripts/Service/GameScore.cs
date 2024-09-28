using System;
using UnityEngine;

[Serializable]
public class GameScore
{
    public event Action OnAddScore;
    public event Action OnResetScore;


    private int _record;
    private int _score;

    private readonly string SaveName = "Record";

    public int Record => _record;
    public int Score => _score;

    public void Init()
    {
        _record = PlayerPrefs.GetInt(SaveName, 0);
        Game.Action.OnStartGame += StartGame;
    }

    private void StartGame() 
    {
        _score = 0;
        OnResetScore?.Invoke();
    } 

    public void Add(int value)
    {
        _score += value;
        OnAddScore?.Invoke();
    }

    public bool GetRecord()
    {
        if (_score > _record)
        {
            _record = _score;
            Save();
            return true;
        }
        else return false;
    }

    private void Save() => PlayerPrefs.SetInt(SaveName, _record);
}