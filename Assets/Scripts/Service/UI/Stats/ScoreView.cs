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
        Game.Score.OnResetScore += ResetScore;
        Game.Score.OnAddScore += Add;
    }

    private void OnEnable()
    {
        if (Game.Score != null)
        {
            Game.Score.OnResetScore += ResetScore;
            Game.Score.OnAddScore += Add;
            Set(Game.Score.Score);
        }
    }

    private void OnDisable()
    {
        Game.Score.OnAddScore -= Add;
        Game.Score.OnResetScore -= ResetScore;

        if (_changeProcess != null)
        {
            StopCoroutine(_changeProcess);
            _changeProcess = null;
        }

    }

    private void ResetScore() 
    {
        if (_changeProcess != null)
        {
            StopCoroutine(_changeProcess);
            _changeProcess = null;
        }
        Set(0);
    } 

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
        while (_current < Game.Score.Score)
        {
            Set(Mathf.Lerp(_current, Game.Score.Score, Time.deltaTime * Speed));
            yield return null;
        }

        Set(Game.Score.Score);
    }
}