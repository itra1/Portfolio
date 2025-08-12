using Core.Materials.Data;

namespace Preview.Stages
{
    public interface IPreviewPublisher
    {
        void AttemptToPublishBasedOnQueue(AreaMaterialData areaMaterial, byte[] bytes);
    }
}