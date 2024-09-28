using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [SerializeField] private Element[] _element;
    [SerializeField] private GameObject _panelInfo;
    [SerializeField] private Image[] _images;
    [SerializeField] private Sprite[] _sprite;
    [SerializeField] private TextMeshProUGUI[] _text;
    [SerializeField] private ButtonBase _button;
    [SerializeField] private GameObject _enable, _disable;

    private MultyWallet _wallet;

    private int _current;

    private void Start()
    {
        _wallet = Game.Wallet as MultyWallet;

        for (int i = 0; i < _element.Length; i++) _element[i].Init();
    }

    public void OpenInfo(int id)
    {
        _current = id;

        _panelInfo.SetActive(true);

        for(int i = 0; i < 3; i++)
        {
            _images[i].sprite = _sprite[_element[id].Type[i]];
            _images[i].SetNativeSize();

            _text[i].text = $"{_wallet.Currancy[_element[id].Type[i]]}/{_element[id].Value[i]}";
        }

        _text[3].text = $"{_wallet.Money}/{_element[id].Money}";

        bool isActive = IsPurchaseAvailable();
        _button.enabled = isActive;
        _enable.SetActive(isActive);
        _disable.SetActive(!isActive);
    }

    public void Buy()
    {
        if(_wallet.SpendMulty(_element[_current].Money, _element[_current].Type, _element[_current].Value))        
            _element[_current].Purchase();        
    }

    private bool IsPurchaseAvailable()
    {
        for (int i = 0; i < 3; i++)
            if (_wallet.Currancy[_element[_current].Type[i]] < _element[_current].Value[i])
                return false;

        if (_wallet.Money < _element[_current].Money)
            return false;
        
        else return true;
    }
}