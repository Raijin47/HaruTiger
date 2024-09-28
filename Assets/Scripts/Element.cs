using UnityEngine;

public class Element : MonoBehaviour
{
    [SerializeField] private int _id;

    [SerializeField] private GameObject _enable;
    [SerializeField] private GameObject _disable;

    [SerializeField] private int _money;
    [SerializeField] private int[] _type;
    [SerializeField] private int[] _value;

    public int Money => _money;
    public int[] Type => _type;
    public int[] Value => _value;

    private readonly string SaveName = "Element";

    public void Init()
    {
        bool purchase = PlayerPrefs.GetInt(SaveName+_id, 0) == 1;

        _enable.SetActive(purchase);
        _disable.SetActive(!purchase);
    }

    public void Purchase()
    {
        _enable.SetActive(true);
        _disable.SetActive(false);
        PlayerPrefs.SetInt(SaveName + _id, 1);
    }
}