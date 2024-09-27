using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreView : MonoBehaviour
{
    [SerializeField] private string _prefix;
    private TextMeshProUGUI _text;
    private float _current;

    private const float Speed = 5;
    private Coroutine _changeProcess;

    private void Awake() => _text = GetComponent<TextMeshProUGUI>();

    private void Start()
    {
        Game.Record.OnResetScore += ResetScore;
        Game.Record.OnAddScore += Add;
    }

    private void OnEnable()
    {
        if (Game.Record != null)
        {
            Game.Record.OnResetScore += ResetScore;
            Game.Record.OnAddScore += Add;
            Set(Game.Record.Score);
        }
    }

    private void OnDisable()
    {
        Game.Record.OnAddScore -= Add;
        Game.Record.OnResetScore -= ResetScore;

        if (_changeProcess != null)
        {
            StopCoroutine(_changeProcess);
            _changeProcess = null;
        }

    }

    private void ResetScore() => Set(0);

    private void Set(float value)
    {
        _current = value;
        _text.text = $"{_prefix}{Mathf.RoundToInt(_current)}";
    }

    private void Add()
    {
        if (!gameObject.activeInHierarchy) return;

        if (_changeProcess != null)
        {
            StopCoroutine(_changeProcess);
            _changeProcess = null;
        }

        _changeProcess = StartCoroutine(AddScoreProcess());
    }

    private IEnumerator AddScoreProcess()
    {
        while (_current < Game.Record.Score)
        {
            Set(Mathf.Lerp(_current, Game.Record.Score, Time.deltaTime * Speed));
            yield return null;
        }

        Set(Game.Record.Score);
    }
}