public class MusicColorTintView : BaseColorTintView
{
    protected override bool Get()
    {
        return _audioColorTint.MusicValue;
    }

    protected override void Swap()
    {
        _audioColorTint.MusicValue = !_audioColorTint.MusicValue;
        UpdateState();
    }
}