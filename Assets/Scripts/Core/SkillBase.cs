using UnityEngine;

public abstract class SkillBase : MonoBehaviour
{
    [SerializeField] private int _price;
    [SerializeField] private GameObject _gameObject;

    protected bool _isPurchase = false;

    private void OnEnable() => Game.Action.OnStartGame += ResetSkill;
    private void OnDisable() => Game.Action.OnStartGame -= ResetSkill;


    private void ResetSkill() => _isPurchase = false;


    public void Action()
    {
        if (_isPurchase) Execute();
        else _gameObject.SetActive(true);
    }

    public void Buy()
    {
        if (Game.Wallet.Spend(_price))
        {
            _isPurchase = true;
            _gameObject.SetActive(false);
        }
    }

    protected abstract void Execute();
}