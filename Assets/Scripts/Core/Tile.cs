using System.Collections;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private int _id;
    [SerializeField] private int _size;
    [SerializeField] private float _min;
    [SerializeField] private float _max;

    private SpriteRenderer _sprite;
    private Coroutine _updateProcessCoroutine;
    private RaycastHit2D[] _collision;

    private const float _fallingSpeed = 5;

    public Transform Transform { get; private set; }
    public BoxCollider2D Collider { get; private set; }
    public Sprite Sprite => _sprite.sprite;
    public Color Color { set => _sprite.color = value; }
    public bool OnAction { get; private set; }
    public float Min => _min;
    public float Max => _max;
    public int Size => _size;

    public void Init()
    {
        Transform = transform;
        _sprite = GetComponent<SpriteRenderer>();
        Collider = GetComponent<BoxCollider2D>();

        Activate();
    }

    public void Activate()
    {
        OnAction = false;

        if (_updateProcessCoroutine != null)
        {
            StopCoroutine(_updateProcessCoroutine);
            _updateProcessCoroutine = null;
        }
        _updateProcessCoroutine = StartCoroutine(UpdateProcessCoroutine());
    }

    private void OnMouseUp()
    {
        Game.Action.OnEndMovement?.Invoke(this);
    }

    private void OnMouseDown() 
    {
        Game.Action.OnStartMovement?.Invoke(this);
    }

    public void Release()
    {
        Game.Wallet.Add(_size);
        Game.Record.Add(_size * 5);
        Game.Action.OnEndFalling?.Invoke(this);
        Destroy(gameObject);
    }

    private bool CanFall()
    {
        Vector2 center = Transform.localPosition + (Size % 2 != 0 ? new Vector2(0.5f, 0) : Vector3.zero) + Vector3.down;
        Vector2 size = new(Size - 0.5f, 0.5f);

        _collision = new RaycastHit2D[4];
        Physics2D.BoxCastNonAlloc(center,  size, 0, Vector2.zero, _collision);

        bool canFall = true;

        for (int i = 0; i < _collision.Length; i++)
        {
            if (!_collision[i]) break;
            if (_collision[i].collider.TryGetComponent(out Tile tile))
            {
                if (!tile.OnAction)
                {
                    canFall = false;
                    break;
                }
            }
            else
            {
                canFall = false;
                break;
            }
        }

        SendMessage(canFall);
        return canFall;
    }

    private void SendMessage(bool onAction)
    {
        if (OnAction == onAction) return;
        OnAction = onAction;
        if (OnAction) Game.Action.OnStartFalling?.Invoke(this);
        else Game.Action.OnEndFalling?.Invoke(this);
    }

    private IEnumerator UpdateProcessCoroutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.1f);

            if (CanFall()) yield return FallingProcessCoroutine(); 
        }
    }

    private IEnumerator FallingProcessCoroutine()
    {
        Vector3 target = transform.localPosition + Vector3.down;

        while (transform.localPosition != target)
        {
            transform.localPosition = Vector2.MoveTowards(transform.localPosition, target, Time.deltaTime * _fallingSpeed);
            yield return null;
        }

        if (CanFall()) yield return FallingProcessCoroutine();
    }
}