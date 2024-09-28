using TMPro;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textGame, _textEnd;

    private void OnEnable() 
    {
        Game.Score.OnResetScore += ResetValue;
        Game.Score.OnAddScore += Set;
    }

    private void OnDisable() 
    {
        Game.Score.OnResetScore -= ResetValue;
        Game.Score.OnAddScore -= Set;
    }

    private void ResetValue()
    {
        _textGame.text = $"{1}";
    }

    private void Set()
    {
        int current = Mathf.Clamp(Game.Score.Score / 100, 1, 100);

        _textGame.text = $"{current}";
        _textEnd.text = $"Level: {current}";
    }
}