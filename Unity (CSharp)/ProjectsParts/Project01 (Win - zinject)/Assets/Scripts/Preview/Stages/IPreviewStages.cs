using Base;

namespace Preview.Stages
{
    public interface IPreviewStages : IVisible, IVisual
    {
        IPreviewSnapshotMaker SnapshotMaker { get; }
        IPreviewPublisher Publisher { get; }
    }
}