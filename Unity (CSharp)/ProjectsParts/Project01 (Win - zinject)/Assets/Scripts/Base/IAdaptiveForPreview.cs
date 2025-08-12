namespace Base
{
    public interface IAdaptiveForPreview
    {
        bool AttemptToSetAlpha(float value);
        bool AttemptToRestoreAlpha();
    }
}