using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private SpriteRenderer _sprite;
    public Sprite Sprite => _sprite.sprite;
    public int Size;
    public int Row => Mathf.RoundToInt(transform.localPosition.y); 
    public Transform Transform;
    public float MinX;
    public float MaxX;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {

        TilesController.OnAddTile?.Invoke(this);
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