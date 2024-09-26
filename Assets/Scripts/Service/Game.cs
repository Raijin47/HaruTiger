using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance;

    [SerializeField] private GameAudio _audio;

    private readonly GameAction GameAction = new();

    public GameAudio Audio => _audio;
    public GameAction Action => GameAction;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //_audioSettings.Init();
    }
}