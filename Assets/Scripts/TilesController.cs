using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesController : MonoBehaviour
{
    [SerializeField] private TileIllusion _illusion;
    [SerializeField] private Tile[] _tiles;
    [SerializeField] private Transform _content;
    [SerializeField] private TileData[] _data;

    public static Action<Tile> OnStartMove;
    public static Action<Tile> OnEndMove;

    private Camera _camera;
    private Coroutine _movementTileProcess;

    private readonly List<Tile> UsedTile = new();
    private readonly List<Tile> FreeTile = new();

    private bool _isStop = false;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Start()
    {
        OnStartMove += StartMovement;
        OnEndMove += StopMovement;
        _ = StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        CreateNewRow();
        ActivateRow();
        yield return new WaitForSeconds(0.5f);
        CreateNewRow();
    }

    private void CreateNewRow()
    {
        int random = UnityEngine.Random.Range(0, _data.Length);

        var data = _data[random];

        for (int i = 0; i < data.Tiles.Length; i++)
        {
            var obj = Instantiate(data.Tiles[i], _content);
            obj.Init();
            obj.Transform.localPosition = data.Position[i];
            FreeTile.Add(obj);
        }
    }

    private void ActivateRow()
    {
        for (int i = FreeTile.Count - 1; i >= 0; i--)
        {
            FreeTile[i].Rigidbody.simulated = true;
            UsedTile.Add(FreeTile[i]);
            FreeTile.Remove(FreeTile[i]);
        }
    }

    private void StartMovement(Tile tile)
    {
        if (_isStop) return;
        _illusion.Tile = tile;
        _illusion.gameObject.SetActive(true);
        _illusion.Transform.localPosition = tile.Transform.localPosition;

        ReleaseCoroutine();
        _movementTileProcess = StartCoroutine(MovementTileProcess());
    }

    private IEnumerator MovementTileProcess()
    {
        while (true)
        {
            float horizontal = _camera.ScreenToWorldPoint(Input.mousePosition).x;
            _illusion.Transform.localPosition = new Vector2(Mathf.Clamp(Mathf.Floor(horizontal), _illusion.Min, _illusion.Max), _illusion.Transform.localPosition.y);
            yield return null;
        }
    }

    private void StopMovement(Tile tile)
    {
        ReleaseCoroutine();
        if (tile.Transform.localPosition.x != _illusion.Transform.localPosition.x)
        {
            tile.Transform.localPosition = _illusion.Transform.localPosition;
            _isStop = true;
            StartCoroutine(MovementRowCoroutine(tile));
        }

        _illusion.gameObject.SetActive(false);
    }

    private void ReleaseCoroutine()
    {
        if (_movementTileProcess != null)
        {
            StopCoroutine(_movementTileProcess);
            _movementTileProcess = null;
        }
    }

    private IEnumerator MovementRowCoroutine(Tile tile)
    {
        yield return new WaitForSeconds(0.2f);

        while(tile.Rigidbody.velocity.y != 0) yield return null;

        foreach (Tile obj in UsedTile)
        {
            obj.Rigidbody.position += Vector2.up;
        }

        ActivateRow();
        CreateNewRow();

        _isStop = false;
    }
}