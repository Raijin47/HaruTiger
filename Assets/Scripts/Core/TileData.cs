using UnityEngine;

[CreateAssetMenu(fileName = "New Preset", menuName = "ScriptableObject/Preset", order = 51)]

public class TileData : ScriptableObject
{
    [SerializeField] private int[] _size;
    [SerializeField] private Vector2[] _position;

    public int[] Size => _size;
    public Vector2[] Position => _position;
}