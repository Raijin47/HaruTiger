public class SkillSlice : SkillBase
{
    protected override void Execute()
    {
        if (TilesController.Instance.Slice())
            _isPurchase = false;
    }
}