using System.Collections;
using TMPro;
using UnityEngine;

public class MoneyView : MonoBehaviour
{
    [SerializeField] private string _prefix;
    private TextMeshProUGUI _text;
    private float _current;

    private const float Speed = 5;
    private Coroutine _changeProcess;

    private void Awake() => _text = GetComponent<TextMeshProUGUI>();

    private void Start()
    {
        Game.Wallet.OnSetMoney += Set;
        Game.Wallet.OnAddMoney += Add;
        Game.Wallet.OnSpendMoney += Spend;
        Set(Game.Wallet.Money);
    }

    private void OnEnable()
    {
        if (Game.Wallet != null)
        {
            Game.Wallet.OnSetMoney += Set;
            Game.Wallet.OnAddMoney += Add;
            Game.Wallet.OnSpendMoney += Spend;
            Set(Game.Wallet.Money);
        }
    }

    private void OnDisable()
    {
        Game.Wallet.OnAddMoney -= Add;
        Game.Wallet.OnSpendMoney -= Spend;
        Game.Wallet.OnSetMoney -= Set;

        if (_changeProcess != null)
            StopCoroutine(_changeProcess);
    }

    private void Set(float value)
    {
        _current = value;
        _text.text = $"{Mathf.RoundToInt(_current)}{_prefix}";
    }

    private void Add()
    {
        if (!gameObject.activeInHierarchy) return;
        if (_changeProcess != null)
            StopCoroutine(_changeProcess);
        _changeProcess = StartCoroutine(AddMoneyProcess());
    }

    private void Spend()
    {
        if (!gameObject.activeInHierarchy) return;
        if (_changeProcess != null)
            StopCoroutine(_changeProcess);
        _changeProcess = StartCoroutine(SpendMoneyProcess());
    }

    private IEnumerator AddMoneyProcess()
    {
        while(_current < Game.Wallet.Money)
        {
            Set(Mathf.Lerp(_current, Game.Wallet.Money, Time.deltaTime * Speed));
            yield return null;
        }

        Set(Game.Wallet.Money);
    }

    private IEnumerator SpendMoneyProcess()
    {
        while (_current > Game.Wallet.Money)
        {
            Set(Mathf.Lerp(_current, Game.Wallet.Money, Time.deltaTime * Speed));
            yield return null;
        }

        Set(Game.Wallet.Money);
    }
}