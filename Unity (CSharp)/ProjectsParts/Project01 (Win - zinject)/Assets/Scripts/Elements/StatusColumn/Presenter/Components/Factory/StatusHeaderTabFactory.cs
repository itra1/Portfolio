using Settings;
using UnityEngine;

namespace Elements.StatusColumn.Presenter.Components.Factory
{
    public class StatusHeaderTabFactory : IStatusHeaderTabFactory
    {
        private readonly IPrefabSettings _prefabs;
        
        public StatusHeaderTabFactory(IPrefabSettings prefabs) => _prefabs = prefabs;
        
        public IStatusHeaderTab Create(RectTransform parent, ulong materialId)
        {
            if (parent == null)
            {
                Debug.LogError("An attempt was detected to assign a null parent to the HeaderTab");
                return null;
            }
            
            if (!_prefabs.TryGetComponent<StatusHeaderTab>(out var original))
            {
                Debug.LogError("There is no prefab with the HeaderTab component in the prefab settings");
                return null;
            }
            
            var tab = Object.Instantiate(original.gameObject).GetComponent<StatusHeaderTab>();
            
            tab.SetName($"Tab: {materialId}");
            tab.SetParent(parent);
            tab.AlignToParent();
            tab.SetBackgroundActive(false);
            tab.SetSeparatorActive(false);
            
            return tab;
        }
    }
}