using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class TilesFactory
{
    [SerializeField] private Tile[] _x1;
    [SerializeField] private Tile[] _x2;
    [SerializeField] private Tile _x3;
    [SerializeField] private Tile[] _x4;

    public Tile GetTile(int size)
    {
        return size switch
        {
            1 => _x1[Random.Range(0, _x1.Length)],
            2 => _x2[Random.Range(0, _x2.Length)],
            3 => _x3,
            4 => _x4[Random.Range(0, _x4.Length)],
            _ => throw new NotImplementedException()
        };
    }

    public Tile GetOne(int id)
    {
        return _x1[id];
    }
}