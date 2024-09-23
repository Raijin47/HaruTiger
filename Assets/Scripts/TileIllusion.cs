using UnityEngine;

public class TileIllusion : MonoBehaviour
{
    private SpriteRenderer _sprite;
    private BoxCollider2D _collider;
    private readonly Vector2 OffsetVector = new(0.5f, 0);

    private Tile _tile;
    public Tile Tile 
    {
        private get => _tile;
        set
        {
            _tile = value;
            _sprite.sprite = value.Sprite;
            _collider.size = new Vector2(value.Size + 1, 0.5f);
            _collider.offset = value.Size % 2 != 0 ? OffsetVector : Vector2.zero;
            Max = value.Max;
            Min = value.Min;
        }
    }
    public Transform Transform { get; private set; }
    public float Min { get; private set; }
    public float Max { get; private set; }

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider2D>();
        Transform = transform;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent(out Tile tile))
        {
            if (tile == Tile) return;
            else
            {
                if (other.transform.position.x > transform.position.x) Max = transform.position.x;
                else Min = transform.position.x;
            }
        }
    }
}
