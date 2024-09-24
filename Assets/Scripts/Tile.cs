using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private int _size;
    [SerializeField] private float _min;
    [SerializeField] private float _max;

    private Rigidbody2D _rigidbody;
    private SpriteRenderer _sprite;
    private Transform _transform;

    public Sprite Sprite => _sprite.sprite;
    public Rigidbody2D Rigidbody => _rigidbody;
    public int Size => _size;
    public int Row => Mathf.RoundToInt(transform.localPosition.y); 
    public Transform Transform => _transform;
    public float Min => _min;
    public float Max => _max;

    public void Init()
    {
        _transform = transform;
        _sprite = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnMouseUp()
    {
        TilesController.OnEndMove?.Invoke(this);
        _sprite.color = Color.white;
    }
    private void OnMouseDown() 
    {
        TilesController.OnStartMove?.Invoke(this);
        _sprite.color = new Color(0.8f,0.8f,0.8f,0.8f);
    }
}