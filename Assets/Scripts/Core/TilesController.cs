using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesController : MonoBehaviour
{
    public static TilesController Instance;

    
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private Transform _content;
    [SerializeField] private TilesFactory _factory;
    [SerializeField] private TileData[] _data;
    [SerializeField] private ParticleSystem _particle;

    private Coroutine _actionProcessCoroutine;

    private readonly Vector2 _startCheckPosition = new(0, 9);
    private readonly Vector2 _checkSize = new(7,0.5f);
    private readonly Vector2 GameOverZone = new(0, 10);

    private readonly WaitForSeconds Interval = new(.2f);

    private readonly List<Tile> ActiveTile = new();
    private readonly List<Tile> PreviewTile = new();
    private readonly List<Tile> FallingTile = new();

    private readonly int _countCheck = 10;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Game.Action.OnStartGame += StartGame;
        Game.Action.OnEndMove += StartAction;
        Game.Action.OnStartFalling += AddFallingTile;
        Game.Action.OnEndFalling += RemovedFallingTile;
    }

    public void RemoveActiveTile(Tile tile)
    {
        ActiveTile.Remove(tile);
        FallingTile.Remove(tile);
    }

    public bool Slice()
    {
        List<Tile> slicing = new();

        foreach (Tile tile in ActiveTile)        
            if (tile.Size == 4)      
                slicing.Add(tile);
                
        if(slicing.Count != 0)
        {
            for(int i = slicing.Count - 1; i >= 0; i--)
            {
                FallingTile.Remove(slicing[i]);
                ActiveTile.Remove(slicing[i]);

                int r = Random.Range(0, 1);
                Vector2 pos = slicing[i].Transform.position + Vector3.left * 3;

                for(int a = 0; a < 4; a++)
                {
                    var tile = Instantiate(_factory.GetOne(r), _content);
                    tile.Init();
                    ActiveTile.Add(tile);
                    tile.Collider.enabled = true;
                    pos += Vector2.right;
                    tile.Transform.localPosition = pos;
                }

                slicing[i].Release();
            }

            AddRow();
            return true;
        }

        return false;
    }

    public bool DestroyRandom()
    {
        if (ActiveTile.Count != 0)
        {
            List<Tile> destroy = new();

            int r = ActiveTile[Random.Range(0, ActiveTile.Count - 1)].Size;


            foreach (Tile tile in ActiveTile)
                if (tile.Size == r)
                    destroy.Add(tile);

            for (int i = destroy.Count - 1; i >= 0; i--)
            {
                FallingTile.Remove(destroy[i]);
                ActiveTile.Remove(destroy[i]);

                destroy[i].Release();
            }

            AddRow();
            return true;
        }
        else return false;
    }

    private void StartGame()
    {
        for (int i = ActiveTile.Count - 1; i >= 0; i--)
            Destroy(ActiveTile[i].gameObject);
        

        for (int i = PreviewTile.Count - 1; i >= 0; i--)
            Destroy(PreviewTile[i].gameObject);

        ActiveTile.Clear();
        PreviewTile.Clear();
        FallingTile.Clear();

        for (int i = 0; i < 3; i++)
        {
            CreateNewRow();
            RiseTile();
        }

        CreateNewRow();
    }

    private void CreateNewRow()
    {
        int random = Random.Range(0, _data.Length);

        var data = _data[random];

        for (int i = 0; i < data.Size.Length; i++)
        {
            var tile = Instantiate(_factory.GetTile(data.Size[i]), _content);
            tile.Init();
            tile.Transform.localPosition = data.Position[i];
            PreviewTile.Add(tile);
        }
    }

    private void RiseTile()
    {
        for (int i = PreviewTile.Count - 1; i >= 0; i--)
        {
            PreviewTile[i].Collider.enabled = true;
            ActiveTile.Add(PreviewTile[i]);
            PreviewTile.Remove(PreviewTile[i]);
        }

        foreach (Tile obj in ActiveTile)
        {
            obj.Transform.localPosition += Vector3.up;
        }
    }

    private void AddFallingTile(Tile tile) => FallingTile.Add(tile);
    private void RemovedFallingTile(Tile tile) => FallingTile.Remove(tile);

    private void StartAction()
    {
        if(_actionProcessCoroutine != null)
        {
            StopCoroutine(_actionProcessCoroutine);
            _actionProcessCoroutine = null;
        }
        _actionProcessCoroutine = StartCoroutine(ActionProcessCoroutine());
    }

    private IEnumerator ActionProcessCoroutine()
    {
        Game.Action.OnBlockAction?.Invoke(true);

        yield return Interval;
        while (FallingTile.Count != 0) yield return null;
        yield return Interval;

        yield return RemoveTileProcessCoroutine();

        RiseTile();
        CreateNewRow();



        yield return Interval;
        yield return RemoveTileProcessCoroutine();
        yield return Interval;

        CheckGameOver();

        Game.Action.OnBlockAction?.Invoke(false);
    }

    private IEnumerator RemoveTileProcessCoroutine()
    {
        Vector2 pos = _startCheckPosition;
        int value;

        for (int i = 0; i < _countCheck; i++)
        {
            Collider2D[] col = Physics2D.OverlapBoxAll(pos, _checkSize, 0f);
            value = 0;


            for (int a = 0; a < col.Length; a++)
            {
                if (!col[a]) break;
                if (col[a].gameObject.TryGetComponent(out Tile tile))
                {
                    value += tile.Size;
                }
                yield return null;
            }

            if (value == 8)
            {
                _particle.transform.position = pos;
                _particle.Play();

                Game.Wallet.Add(8);
                Game.Score.Add(40);
                Game.Action.OnBonus?.Invoke();
                for (int b = 0; b < col.Length; b++)
                {
                    if (!col[b]) break;

                    if (col[b].gameObject.TryGetComponent(out Tile tile))
                    {
                        ActiveTile.Remove(tile);
                        tile.Release();
                    }

                    yield return null;
                }
            }

            pos += Vector2.down;
            yield return null;
        }

        yield return Interval;

        if (FallingTile.Count != 0)
        {
            yield return Interval;
            while (FallingTile.Count != 0) yield return null;
            yield return RemoveTileProcessCoroutine();
        }
    }

    public void AddRow()
    {
        if(ActiveTile.Count == 0)
        {
            RiseTile();
            CreateNewRow();
        }
    }

    private void CheckGameOver()
    {
        if(Physics2D.BoxCast(GameOverZone, _checkSize, 0, Vector2.zero))
        {
            _gameOverPanel.SetActive(true);
            Game.Audio.OnGameOver();
            Game.Action.OnGameOver?.Invoke();
        }
    }
}