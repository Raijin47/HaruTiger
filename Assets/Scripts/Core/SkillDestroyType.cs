public class SkillDestroyType : SkillBase
{
    protected override void Execute()
    {
        if(TilesController.Instance.DestroyRandom())
            _isPurchase = false;
    }
}