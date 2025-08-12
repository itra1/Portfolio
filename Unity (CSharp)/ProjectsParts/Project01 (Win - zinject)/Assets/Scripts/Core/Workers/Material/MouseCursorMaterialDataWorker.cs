using com.ootii.Messages;
using Core.Materials.Data;
using Core.Messages;

namespace Core.Workers.Material
{
    public class MouseCursorMaterialDataWorker : IAfterAddingToStorage
    {
        public void PerformActionAfterAddingToStorageOf(MaterialData material)
        {
            if (material is MouseCursorMaterialData)
                MessageDispatcher.SendMessage(MessageType.MouseCursorLoad);
        }
    }
}