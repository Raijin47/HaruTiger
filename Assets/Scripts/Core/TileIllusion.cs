using System.Collections;
using UnityEngine;

public class TileIllusion : MonoBehaviour
{
    public static TileIllusion Instance;

    [SerializeField] private float _speed;
    [SerializeField] private Sprite _destroySprite;
    [SerializeField] private ParticleSystem _particle;

    private SpriteRenderer _sprite;
    private BoxCollider2D _collider;

    private Transform _transform;
    public float _min { get; set; }
    public float _max { get; set; }

    private Tile _currentTile;
    private Camera _camera;
    private Coroutine _movementTileProcess;

    private readonly Color ActiveColor = new(0.8f, 0.8f, 0.8f, 0.8f);
    private const float Distance = 8;
    public bool onBlockAction;
    private bool onGameActive = false;
    private bool onDestroy = false;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _camera = Camera.main;
        _sprite = GetComponent<SpriteRenderer>();
        _transform = transform;
        Instance = this;
    }

    private void Start()
    {
        Game.Action.OnStartGame += () => { onGameActive = true; onDestroy = false; };
        Game.Action.OnGameOver += () => { onGameActive = false; };
        Game.Action.OnStartMovement += StartMovement;
        Game.Action.OnEndMovement += StopMovement;
        Game.Action.OnBlockAction += (bool value) => { onBlockAction = value; };
    }

    private readonly Vector2 destroyPos = new (-1, 6);

    public void SetDestroyAction()
    {
        onDestroy = true;
        _collider.enabled = true;
        _transform.localPosition = destroyPos;
        _sprite.sprite = _destroySprite;
    }

    private void OnMouseDown()
    {
        if (onDestroy) 
        {
            ReleaseCoroutine();
            _movementTileProcess = StartCoroutine(MovementDestroyProcess());
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + Vector3.right * 0.5f, new Vector2(5,5));
    }

    private void OnMouseUp()
    {
        if (onDestroy)
        {
            onDestroy = false;
            ReleaseCoroutine();
            _collider.enabled = false;

            _particle.transform.localPosition = _transform.localPosition;
            _particle.Play();

            RaycastHit2D[] hit = Physics2D.BoxCastAll(_transform.position + Vector3.right * 0.5f, new Vector2(5, 5), 0, Vector2.zero);

            for (int i = 0; i < hit.Length; i++)
            {
                if(hit[i].collider.TryGetComponent(out Tile tile))
                {
                    tile.ReleaseAdd();
                }
            }

            _transform.localPosition = new Vector2(0, 15);
        }
    }

    private const float _minX = -2;
    private const float _maxX = 1;
    private const float _minY = 2;
    private const float _maxY = 7;

    private IEnumerator MovementDestroyProcess()
    {
        while (true)
        {
            float horizontal = _camera.ScreenToWorldPoint(Input.mousePosition).x;
            float vertical = _camera.ScreenToWorldPoint(Input.mousePosition).y;

            horizontal = Mathf.Clamp(Mathf.Floor(horizontal), _minX, _maxX);
            vertical = Mathf.Clamp(Mathf.Round(vertical), _minY, _maxY);

            Vector2 target = new(horizontal, vertical);
            _transform.localPosition = Vector2.Lerp(_transform.localPosition, target, Time.deltaTime * _speed);
            yield return null;
        }
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
        if (onDestroy) return;

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
        _transform.localPosition = new Vector2(0, 15);
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