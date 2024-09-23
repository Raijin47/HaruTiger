using System;
using System.Collections;
using UnityEngine;

public class TilesController : MonoBehaviour
{
    [SerializeField] private TileIllusion _illusion;
    [SerializeField] private Tile[] _tiles;

    public static Action<Tile> OnStartMove;
    public static Action<Tile> OnEndMove;

    private Camera _camera;
    private Coroutine _movementTileProcess;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Start()
    {
        OnStartMove += StartMovement;
        OnEndMove += StopMovement;
    }

    private void StartMovement(Tile tile)
    {
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
            MoveRow();
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

    private void MoveRow()
    {
        StartCoroutine(MovementRowCoroutine());
    }
    [SerializeField] private Transform _element;

    private IEnumerator MovementRowCoroutine()
    {
        float elapsedSize = 1;

        while(_element.transform.localScale.y < 3.5)
        {
            elapsedSize += Time.deltaTime * 5;
            _element.localScale = Vector2.one * elapsedSize;
            yield return null;
        }

        Debug.Log("continue");

        _element.localScale = Vector2.one;

    }
}