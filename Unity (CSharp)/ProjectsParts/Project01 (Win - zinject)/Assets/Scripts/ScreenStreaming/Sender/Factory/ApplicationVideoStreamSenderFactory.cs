using ScreenStreaming.Sender.Source.Factory;
using Settings;
using UnityEngine;

namespace ScreenStreaming.Sender.Factory
{
    public class ApplicationVideoStreamSenderFactory : IApplicationVideoStreamSenderFactory
    {
        private readonly IPrefabSettings _prefabs;
        private readonly IApplicationVideoStreamSourceFactory _factory;
        
        public ApplicationVideoStreamSenderFactory(IPrefabSettings prefabs, IApplicationVideoStreamSourceFactory factory)
        {
            _prefabs = prefabs;
            _factory = factory;
        }
        
        public IApplicationVideoStreamSender Create(RectTransform parent)
        {
            if (parent == null)
            {
                Debug.LogError("An attempt was detected to assign a null parent to the ApplicationVideoStreamSender");
                return null;
            }
            
            ApplicationVideoStreamSender original = null;
            
            foreach (var prefab in _prefabs.RenderStreamingObjects)
            {
                if (!prefab.TryGetComponent<ApplicationVideoStreamSender>(out var component))
                    continue;
                
                original = component;
                break;
            }
            
            if (original == null)
            {
                Debug.LogError("Render streaming sender original is not found among components of prefabs");
                return null;
            }
            
            var originalObject = original.gameObject;
            var senderObject = Object.Instantiate(originalObject, parent);
            
            senderObject.name = originalObject.name;
            
            var sender = senderObject.GetComponent<ApplicationVideoStreamSender>();
            
            sender.Initialize(_factory);
            
            return sender;
        }
    }
}