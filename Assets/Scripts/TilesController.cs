using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesController : MonoBehaviour
{
    [SerializeField] private TileIllusion _illusion;

    public static Action<Tile> OnStartMove;
    public static Action<Tile> OnEndMove;
    public static Action<Tile> OnAddTile;

    private Camera _camera;
    private Coroutine _movementTileProcess;

    private readonly List<Tile> UsedTile = new();
    private readonly List<Tile> FreeTile = new();
    void Start()
    {
        _camera = Camera.main;
        OnStartMove += StartMovement;
        OnEndMove += StopMovement;
        OnAddTile += AddTile;
    }

    private void AddTile(Tile tile)
    {
        UsedTile.Add(tile);
    }

    private void StartMovement(Tile tile)
    {
        _illusion.Transform.localPosition = tile.Transform.localPosition;
        _illusion.gameObject.SetActive(true);
        _illusion.Sprite = tile.Sprite;

        ReleaseCoroutine();
        _movementTileProcess = StartCoroutine(MovementTileProcess(tile));
    }

    private IEnumerator MovementTileProcess(Tile tile)
    {
        while (true)
        {
            Vector2 pos = _camera.ScreenToWorldPoint(Input.mousePosition);

            float x = Mathf.Floor(pos.x);

            _illusion.Transform.localPosition = new Vector2(Mathf.Clamp(Mathf.Floor(pos.x), tile.MinX, tile.MaxX), _illusion.Transform.localPosition.y);
            yield return null;
        }
    }

    private void StopMovement(Tile tile)
    {
        ReleaseCoroutine();
        tile.Transform.localPosition = _illusion.Transform.localPosition;


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
}