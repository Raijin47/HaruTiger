using System;
using UnityEngine;

[Serializable]
public class GameWallet
{
    public event Action<float> OnSetMoney;
    public event Action OnAddMoney;
    public event Action OnSpendMoney;

    [SerializeField] private int _money;
    [SerializeField] private int _starting;

    private readonly string SaveName = "Money";

    public int Money => _money;

    public void Init()
    {
        _money = PlayerPrefs.GetInt(SaveName, _starting);
        OnSetMoney?.Invoke(_money);
    }

    public void Add(int value)
    {
        _money += value;
        OnAddMoney?.Invoke();
        Save();
    }

    public bool Spend(int value)
    {
        if (_money >= value)
        {
            _money -= value;
            OnSpendMoney?.Invoke();
            Save();
            return true;
        }
        else return false;
    }

    private void Save() => PlayerPrefs.SetInt(SaveName, _money);
}