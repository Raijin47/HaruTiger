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
    [SerializeField] private TileData[] _tutorialData;
    [SerializeField] private ParticleSystem _particle;
    [SerializeField] private ParticleSystem _sliceParticle;
    [SerializeField] private ParticleSystem[] _typeParticles;

    private Coroutine _actionProcessCoroutine;
    private Coroutine _removeProcessCoroutine;


    private readonly Vector2 _startCheckPosition = new(0, 9);
    private readonly Vector2 _checkSize = new(7,0.5f);
    private readonly Vector2 GameOverZone = new(0, 10);

    private readonly WaitForSeconds Interval = new(.1f);

    private readonly List<Tile> ActiveTile = new();
    private readonly List<Tile> PreviewTile = new();
    private readonly List<Tile> FallingTile = new();
    private readonly List<Tile> RiseTileList = new();

    private readonly int _countCheck = 10;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Game.Action.OnStartGame += StartGame;
        Game.Action.OnEndMove += StartAction;
    }

    public void RemoveActiveTile(Tile tile)
    {
        ActiveTile.Remove(tile);
        FallingTile.Remove(tile);
        RiseTileList.Remove(tile);
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
                RiseTileList.Remove(slicing[i]);

                int r = Random.Range(0, 1);
                Vector2 pos = slicing[i].Transform.position + Vector3.left * 3;

                _sliceParticle.transform.localPosition = slicing[i].Transform.localPosition;
                _sliceParticle.Play();

                for(int a = 0; a < 4; a++)
                {
                    var tile = Instantiate(_factory.GetOne(r), _content);
                    tile.Init();
                    tile.StartFalling();
                    ActiveTile.Add(tile);
                    tile.Collider.enabled = true;
                    pos += Vector2.right;
                    tile.Transform.localPosition = pos;
                }

                slicing[i].Release();
            }
            RemoveTile();
            return true;
        }

        return false;
    }

    private int GetParticle(int id)
    {
        return id switch
        {
            < 2 => 0,
            < 5 => 1,
            5 => 2,
            > 5 => 3,
        };
    }

    public bool DestroyRandom()
    {
        if (ActiveTile.Count != 0)
        {
            List<Tile> destroy = new();

            int r = ActiveTile[Random.Range(0, ActiveTile.Count - 1)].ID;

            int particle = GetParticle(r);


            foreach (Tile tile in ActiveTile)
                if (tile.ID == r)
                    destroy.Add(tile);

            _ = StartCoroutine(DestroyRandomProcess(destroy, particle));

            return true;
        }
        else return false;
    }

    private IEnumerator DestroyRandomProcess(List<Tile> destroy, int particle)
    {
        for (int i = destroy.Count - 1; i >= 0; i--)
        {
            FallingTile.Remove(destroy[i]);
            ActiveTile.Remove(destroy[i]);
            RiseTileList.Remove(destroy[i]);

            _typeParticles[particle].transform.localPosition = destroy[i].Transform.localPosition;
            _typeParticles[particle].Play();

            destroy[i].ReleaseAdd();
            yield return Interval;
        }
    }

    private void StartGame()
    {
        if(PlayerPrefs.GetInt("Tutorial", 0) != 1)
        {
            for (int i = 0; i < _tutorialData[0].Size.Length; i++)
            {
                var tile = Instantiate(_factory.GetTile(_tutorialData[0].Size[i]), _content);
                tile.Init();
                tile.Transform.localPosition = _tutorialData[0].Position[i];
                ActiveTile.Add(tile);
                tile.Collider.enabled = true;
                tile.Blocking = i != 1;
                tile.StartFalling();
            }

            for (int i = 0; i < _tutorialData[1].Size.Length; i++)
            {
                var tile = Instantiate(_factory.GetTile(_tutorialData[1].Size[i]), _content);
                tile.Init();
                tile.Transform.localPosition = _tutorialData[1].Position[i];
                tile.Blocking = true;
                PreviewTile.Add(tile);
            }

            return;
        }


        for (int i = ActiveTile.Count - 1; i >= 0; i--)
            Destroy(ActiveTile[i].gameObject);
        

        for (int i = PreviewTile.Count - 1; i >= 0; i--)
            Destroy(PreviewTile[i].gameObject);

        ActiveTile.Clear();
        PreviewTile.Clear();
        FallingTile.Clear();
        RiseTileList.Clear();

        CreateNewRow(Vector2.zero);
        CreateNewRow(Vector2.up);
        CreateNewRow(Vector2.up * 2);
        RiseTile();
        CreateNewRow(Vector2.zero);

        if(_checkProcessCoroutine != null)
        {
            StopCoroutine(_checkProcessCoroutine);
            _checkProcessCoroutine = null;
        }
        _checkProcessCoroutine = StartCoroutine(Check());
    }

    private Coroutine _checkProcessCoroutine;

    private void RemoveTile()
    {
        if(_removeProcessCoroutine != null)
        {
            StopCoroutine(_removeProcessCoroutine);
            _removeProcessCoroutine = null;
        }

        _removeProcessCoroutine = StartCoroutine(RemoveTileProcessCoroutine());
    }

    private void CreateNewRow(Vector2 pos)
    {
        int random = Random.Range(0, _data.Length);

        var data = _data[random];

        for (int i = 0; i < data.Size.Length; i++)
        {
            var tile = Instantiate(_factory.GetTile(data.Size[i]), _content);
            tile.Init();
            tile.Transform.localPosition = data.Position[i] + pos;
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
            obj.Rise();

        RemoveTile();
    }

    public void AddFallingTile(Tile tile) => FallingTile.Add(tile);
    public void RemovedFallingTile(Tile tile) => FallingTile.Remove(tile);
    public void AddRiseTile(Tile tile) => RiseTileList.Add(tile);
    public void RemoveRiseTile(Tile tile) => RiseTileList.Remove(tile);


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
        while (FallingTile.Count != 0 | RiseTileList.Count != 0) yield return null;

        yield return Interval;

        yield return RemoveTileProcessCoroutine();

        RiseTile();
        CreateNewRow(Vector2.zero);

        CheckGameOver();

        yield return Interval;
        yield return RemoveTileProcessCoroutine();

        Game.Action.OnBlockAction?.Invoke(false);
    }

    private IEnumerator RemoveTileProcessCoroutine()
    {
        Game.Action.OnBlockAction?.Invoke(true);
        Vector2 pos = _startCheckPosition;
        int value;

        yield return Interval;

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

        if (FallingTile.Count != 0 | RiseTileList.Count != 0)
        {
            yield return Interval;
            while (FallingTile.Count != 0) yield return null;
            RemoveTile();
        }
        Game.Action.OnBlockAction?.Invoke(false);
    }

    private IEnumerator Check()
    {
        while(true)
        {
            if (ActiveTile.Count == 0 && !TileIllusion.Instance.onBlockAction)
            {
                RiseTile();
                CreateNewRow(Vector2.zero);
                RemoveTile();
            }

            yield return new WaitForSeconds(2f);
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