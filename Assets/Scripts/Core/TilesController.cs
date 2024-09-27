using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesController : MonoBehaviour
{
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

    private void Start()
    {
        Game.Action.OnStartGame += StartGame;
        Game.Action.OnEndMove += StartAction;
        Game.Action.OnStartFalling += AddFallingTile;
        Game.Action.OnEndFalling += RemovedFallingTile;
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