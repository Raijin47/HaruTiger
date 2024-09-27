using System.Collections;
using UnityEngine;

public class TileIllusion : MonoBehaviour
{
    [SerializeField] private float _speed;

    private SpriteRenderer _sprite;

    private Transform _transform;
    public float _min;
    public float _max;

    private Tile _currentTile;
    private Camera _camera;
    private Coroutine _movementTileProcess;

    private readonly Color ActiveColor = new(0.8f, 0.8f, 0.8f, 0.8f);
    private const float Distance = 8;
    private bool onBlockAction;
    private bool onGameActive = false;

    private void Awake()
    {
        _camera = Camera.main;
        _sprite = GetComponent<SpriteRenderer>();
        _transform = transform;
    }

    private void Start()
    {
        Game.Action.OnStartGame += () => { onGameActive = true; };
        Game.Action.OnGameOver += () => { onGameActive = false; };
        Game.Action.OnStartMovement += StartMovement;
        Game.Action.OnEndMovement += StopMovement;
        Game.Action.OnBlockAction += (bool value) => { onBlockAction = value; };
    }

    private void View(Tile tile)
    {
        tile.Collider.enabled = false;
        _sprite.sprite = tile.Sprite;
        _transform.localPosition = tile.Transform.localPosition;

        RaycastHit2D left = Physics2D.Raycast(_transform.localPosition, Vector2.left * Distance);
        RaycastHit2D right = Physics2D.Raycast(_transform.localPosition, Vector2.right * Distance);

        if(left)
        {
            float offset = tile.Size > 1 ? tile.Size != 4 ? 1 : 2 : 0;
            _min = Mathf.Round(left.point.x + offset);
        }
        else _min = tile.Min;

        if (right)
        {
            float offset = tile.Size == 3 || tile.Size == 4 ? 2 : 1;
            _max = Mathf.Round(right.point.x - offset);
        }
        else _max = tile.Max;

        tile.Collider.enabled = true;
        tile.Color = ActiveColor;
    }

    private void StartMovement(Tile tile)
    {
        if (!onGameActive) return; 
        if (onBlockAction) return;

        _currentTile = tile;
        View(tile);

        ReleaseCoroutine();
        _movementTileProcess = StartCoroutine(MovementTileProcess());
    }

    private IEnumerator MovementTileProcess()
    {
        while (true)
        {
            float horizontal = _camera.ScreenToWorldPoint(Input.mousePosition).x;
            Vector2 target = new(Mathf.Clamp(Mathf.Floor(horizontal), _min, _max), _transform.localPosition.y);
            _transform.localPosition = Vector2.Lerp(_transform.localPosition, target, Time.deltaTime * _speed);
            yield return null;
        }
    }

    private void StopMovement(Tile tile)
    {
        if (!onGameActive) return;
        if (onBlockAction) return;

        if (_currentTile == null) return;

        ReleaseCoroutine();

        _transform.localPosition = new(Mathf.Round(_transform.localPosition.x), _transform.localPosition.y);

        if (tile.Transform.localPosition.x != _transform.localPosition.x)
        {
            tile.Transform.localPosition = _transform.localPosition;
            Game.Action.OnEndMove?.Invoke();
        }

        tile.Color = Color.white;
        _currentTile = null;
        _transform.localPosition = new Vector2(0, 11);
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