using System.Collections.Generic;
using Base;
using Core.Elements.Windows.Base.Data;
using Elements.StatusColumn.Presenter.Components.Factory;
using Settings;
using Settings.Data;
using UnityEngine;

namespace Elements.StatusColumn.Presenter.Components
{
    [DisallowMultipleComponent, RequireComponent(typeof(RectTransform))]
    public class StatusHeader : MonoBehaviour, IVisible, IUnloadable
    {
        private IDictionary<ulong, IStatusHeaderTab> _tabsByMaterialId;
        private IList<IStatusHeaderTab> _orderedTabs;
        private IUISettings _settings;
        private IStatusHeaderTabFactory _factory;
        
        private RectTransform _rectTransform;
        
        public RectTransform RectTransform => 
            _rectTransform == null ? _rectTransform = GetComponent<RectTransform>() : _rectTransform;
        
        public bool Visible => gameObject.activeSelf;
        
        public int TabsCount => _orderedTabs.Count;
        
        public void Initialize(IUISettings settings, IStatusHeaderTabFactory factory)
        {
            _tabsByMaterialId = new Dictionary<ulong, IStatusHeaderTab>();
            _orderedTabs = new List<IStatusHeaderTab>();
            _settings = settings;
            _factory = factory;
        }
        
        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);

        public bool ContainsTab(WindowMaterialData material) => _tabsByMaterialId.ContainsKey(material.Id);
        
        public bool AddTab(WindowMaterialData material, WindowMaterialIconType iconType, string materialName)
        {
            var materialId = material.Id;
            
            if (ContainsTab(material))
            {
                Debug.LogError($"An attempt was detected to add a tab by material id {materialId}, which had already been contained in the status header tab dictionary");
                return false;
            }
            
            var targetTab = _factory.Create(RectTransform, materialId);
            
            targetTab.SetIconSprite(_settings.GetWindowMaterialIconSprite(iconType));
            targetTab.SetTitleText(materialName);
            targetTab.SetBackgroundActive(false);
            targetTab.SetSeparatorActive(false);
            
            _tabsByMaterialId.Add(materialId, targetTab);
            _orderedTabs.Add(targetTab);
            
            return true;
        }
        
        public void ActivateTab(WindowMaterialData material)
        {
            var materialId = material.Id;
            
            if (!_tabsByMaterialId.TryGetValue(materialId, out var targetTab))
            {
                Debug.LogError($"An attempt was detected to activate a tab by material id {materialId} which was not found in the status header tab dictionary");
                return;
            }
            
            IStatusHeaderTab previousTab = null;
            
            foreach (var tab in _orderedTabs)
            {
                tab.SetBackgroundActive(tab == targetTab);
                tab.SetSeparatorActive(false);
                
                previousTab?.SetSeparatorActive(!tab.IsBackgroundActive && !previousTab.IsBackgroundActive);
                
                previousTab = tab;
            }
        }
        
        public bool RemoveTab(ulong materialId)
        {
            if (!_tabsByMaterialId.Remove(materialId, out var targetTab))
            {
                Debug.LogError($"An attempt was detected to remove a tab by material id {materialId} which was not found in the status header tab dictionary");
                return false;
            }
            
            _orderedTabs.Remove(targetTab);
            
            targetTab.Unload();
            
            return true;
        }
        
        public void ReorderTabs(IReadOnlyList<WindowMaterialData> orderedMaterials)
        {
            if (_tabsByMaterialId.Count != orderedMaterials.Count)
            {
                Debug.LogError("A mismatch was detected between the count of tabs and the count of ordered materials");
                return;
            }
            
            _orderedTabs.Clear();
            
            foreach (var material in orderedMaterials)
            {
                var materialId = material.Id;
                
                if (!_tabsByMaterialId.TryGetValue(materialId, out var targetTab))
                {
                    Debug.LogError($"An attempt was detected to reorder a tab by material id {materialId} which was not found in the status header tab dictionary");
                    continue;
                }
                
                _orderedTabs.Add(targetTab);
            }
            
            ForceRebuildLayoutTabs();
        }
        
        public void Unload()
        {
            foreach (var tab in _orderedTabs)
                tab.Unload();
            
            _tabsByMaterialId.Clear();
            _orderedTabs.Clear();
        }
        
        private void ForceRebuildLayoutTabs()
        {
            IStatusHeaderTab previousTab = null;
            
            foreach (var tab in _orderedTabs)
            {
                var tabLocalPositionX = 0f;
                
                tab.SetSeparatorActive(false);
                
                if (previousTab != null)
                {
                    previousTab.SetSeparatorActive(!tab.IsBackgroundActive && !previousTab.IsBackgroundActive);
                    
                    var previousTabRectTransform = previousTab.RectTransform;
                    var previousTabLocalPosition = previousTabRectTransform.localPosition;
                    
                    tabLocalPositionX = previousTabLocalPosition.x + previousTabRectTransform.rect.width;
                }
                
                var tabRectTransform = tab.RectTransform;
                
                tabRectTransform.localPosition = new Vector2(tabLocalPositionX, tabRectTransform.localPosition.y);
                
                var tabTitleRect = tab.TitleRect;
                var tabWidth = Mathf.Ceil(tabTitleRect.x + tabTitleRect.width + 20.0f);
                var tabHeight = RectTransform.rect.height;
                
                tabRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tabWidth);
                tabRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tabHeight);
                
                previousTab = tab;
            }
        }
    }
}