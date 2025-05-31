using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance;

    [SerializeField] private GameAudio _audio;

    [Space(10)]

    [SerializeReference, SubclassSelector] private Wallet _walletType;
    [SerializeReference, SubclassSelector] private AudioSettings _audioType;

    [Space(10)]
    [SerializeReference, SubclassSelector] private List<Component> _components;

    private readonly GameScore GameScore = new();
    private readonly GameAction GameAction = new();

    public static GameAudio Audio;
    public static Wallet Wallet;
    public static GameAction Action;
    public static GameScore Score;

    public AudioSettings AudioSettings => _audioType;

    private void Awake()
    {
        Instance = this;
        Audio = _audio;
        Wallet = _walletType;
        Score = GameScore;
        Action = GameAction;
    }

    private void Start()
    {
        _walletType.Init();
        _audioType.Init();
        GameScore.Init();
        _audio.Init();

        foreach (Component component in _components)
            component.Init();

    }

    public T Get<T>() where T : Component
    {
        return (T)_components.FirstOrDefault(item => item is T);
    }

    public void StartGame() => Action.OnStartGame?.Invoke();
    public void GameOver() => Action.OnGameOver?.Invoke();
}