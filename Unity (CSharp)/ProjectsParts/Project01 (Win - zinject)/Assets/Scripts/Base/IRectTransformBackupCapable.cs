namespace Base
{
    public interface IRectTransformBackupCapable : IOriginalRectTransformRestorer, IRectTransformable, IOriginalRectTransform
    {
        void SaveOriginalRectTransform();
    }
}