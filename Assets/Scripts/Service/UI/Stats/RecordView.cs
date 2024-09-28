using System.Collections;
using TMPro;
using UnityEngine;

public class RecordView : MonoBehaviour
{
    [SerializeField] private string _prefix;
    private TextMeshProUGUI _text;
    private float _current;

    private const float Speed = 5;
    private Coroutine _changeProcess;

    private void Awake() => _text = GetComponent<TextMeshProUGUI>();

    private void OnEnable()
    {
        Set(Game.Score.Record);

        if(Game.Score.GetRecord())
        {
            if (_changeProcess != null)
            {
                StopCoroutine(_changeProcess);
                _changeProcess = null;
            }

            _changeProcess = StartCoroutine(UpdateRecordProcess());
        }
    }

    private void OnDisable()
    {
        if (_changeProcess != null)
        {
            StopCoroutine(_changeProcess);
            _changeProcess = null;
        }
    }

    private void Set(float value)
    {
        _current = value;
        _text.text = $"{_prefix}{Mathf.RoundToInt(_current)}";
    }

    private IEnumerator UpdateRecordProcess()
    {
        while (_current < Game.Score.Record)
        {
            Set(Mathf.Lerp(_current, Game.Score.Record, Time.deltaTime * Speed));
            yield return null;
        }

        Set(Game.Score.Record);
    }
}