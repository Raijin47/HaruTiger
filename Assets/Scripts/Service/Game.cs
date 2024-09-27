using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance;

    [SerializeField] private GameAudio _audio;
    [SerializeField] private GameWallet _wallet;
    [SerializeField] private GameRecord _record;
    [Space(10)]

    [SerializeReference, SubclassSelector] private AudioSettingsBase _audioType;

    private readonly GameAction GameAction = new();

    public static GameAudio Audio;
    public static GameWallet Wallet;
    public static GameAction Action;
    public static GameRecord Record;

    public AudioSettingsBase AudioSettings => _audioType;

    private void Awake()
    {
        Instance = this;
        Audio = _audio;
        Wallet = _wallet;
        Record = _record;
        Action = GameAction;
    }

    private void Start()
    {
        _wallet.Init();
        _audioType.Init();
        _record.Init();
        _audio.Init();
    }

    public void StartGame() => Action.OnStartGame?.Invoke();
    public void GameOver() => Action.OnGameOver?.Invoke();
}