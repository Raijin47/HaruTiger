using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class TilesFactory
{
    [SerializeField] private Tile[] _x1;
    [SerializeField] private Tile[] _x2;
    [SerializeField] private Tile[] _x3;
    [SerializeField] private Tile _x4;

    public Tile GetTile(int size)
    {
        return size switch
        {
            1 => _x1[Random.Range(0, _x1.Length)],
            2 => _x2[Random.Range(0, _x2.Length)],
            3 => _x3[Random.Range(0, _x3.Length)],
            4 => _x4,
            _ => _x1[Random.Range(0, _x1.Length)]
        };
    }

    public Tile GetOne(int id)
    {
        return _x1[id];
    }
}