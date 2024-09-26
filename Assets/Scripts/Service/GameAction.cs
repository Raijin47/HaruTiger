using System;

public class GameAction
{
    public Action<Tile> OnStartMovement;
    public Action<Tile> OnEndMovement;
    public Action<Tile> OnStartFalling;
    public Action<Tile> OnEndFalling;
    public Action OnEndMove;
    public Action<bool> OnBlockAction;
}