using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileIllusion : MonoBehaviour
{
    private SpriteRenderer _sprite;

    public Sprite Sprite { set => _sprite.sprite = value; }

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }

    public Transform Transform => transform;


}
