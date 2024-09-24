using UnityEngine;

[CreateAssetMenu(fileName = "New Preset", menuName = "ScriptableObject/Preset", order = 51)]

public class TileData : ScriptableObject
{
    [SerializeField] private Tile[] _tiles;
    [SerializeField] private Vector2[] _position;

    public Tile[] Tiles => _tiles;
    public Vector2[] Position => _position;
}