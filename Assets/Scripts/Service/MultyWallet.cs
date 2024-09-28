using System;
using UnityEngine;

[Serializable]
public class MultyWallet : Wallet
{
    [SerializeField] private int[] _currancy;
    public int[] Currancy => _currancy;

    public override void Init()
    {
        base.Init();

        for(int i = 0; i < _currancy.Length; i++)
        {
            _currancy[i] = PlayerPrefs.GetInt(SaveName + i, 0);
        }
    }

    public void AddMulty(int id, int value = 1)
    {
        _currancy[id] += value;
        Save(id);
    }

    public bool SpendMulty(int moneyValue, int[] id, int[] value)
    {
        for(int i = 0; i < id.Length; i++)        
            if (_currancy[id[i]] < value[i]) return false;


        if (Spend(moneyValue))
        {
            for (int i = 0; i < id.Length; i++)
            {
                _currancy[id[i]] -= value[i];
                Save(id[i]);
            }
            return true;
        }
        else return false;
    }

    protected void Save(int id) => PlayerPrefs.SetInt(SaveName + id, _currancy[id]);
}