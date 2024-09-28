using System;
using UnityEngine;

[Serializable]
public class Wallet
{
    public event Action<float> OnSetMoney;
    public event Action OnAddMoney;
    public event Action OnSpendMoney;

    [SerializeField] protected int _money;
    [SerializeField] private int _starting;

    protected readonly string SaveName = "Money";

    public int Money => _money;

    public virtual void Init()
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

    protected void Save() => PlayerPrefs.SetInt(SaveName, _money);
}