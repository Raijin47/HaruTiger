public class SkillDestroyArea : SkillBase
{
    protected override void Execute()
    {
        TileIllusion.Instance.SetDestroyAction();
        _isPurchase = false;
    }
}