public class SoundColorTintView : BaseColorTintView
{
    protected override bool Get()
    {
        return _audioColorTint.SoundValue;
    }

    protected override void Swap()
    {
        _audioColorTint.SoundValue = !_audioColorTint.SoundValue;
        UpdateState();
    }
}