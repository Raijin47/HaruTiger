using UnityEngine;

public abstract class SkillBase : MonoBehaviour
{
    [SerializeField] private int _price;
    [SerializeField] private GameObject _gameObject;

    public void Use()
    {
        if(Game.Wallet.Spend(_price))
        {
            Action();
            _gameObject.SetActive(false);
        }
    }

    protected abstract void Action();
}