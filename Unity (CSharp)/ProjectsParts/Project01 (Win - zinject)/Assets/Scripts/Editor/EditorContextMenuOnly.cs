using System.Collections.Generic;
using System.Linq;
using System.Text;
using Elements.Widgets.Base.Presenter;
using Settings;
using UnityEngine;
using Zenject;
using Debug = Core.Logging.Debug;

namespace Editor
{
    public static class EditorContextMenuOnly
    {
        [ContextMenu("Print tags")]
        private static void PrintOpenTags()
        {
            var prefabSettings = StaticContext.Container.TryResolve<IPrefabSettings>();
			
            if (prefabSettings == null)
            {
                Debug.LogError("You need to create prefab settings first");
                return;
            }
			
            var widgets = prefabSettings.Widgets;

            if (widgets == null)
            {
                Debug.LogError("You need to create widget prefabs and then actualize prefab settings");
                return;
            }
			
            var tags = new HashSet<string>();
            var messageBuffer = new StringBuilder();
			
            foreach (var widget in prefabSettings.Widgets.OfType<IWidgetPresenter>())
            {
                var tag = $"V {widget.Tag}";
				
                if (!tags.Add(tag))
                    continue;

                messageBuffer.Append($"{tag}\n");
            }

            Debug.Log(messageBuffer.ToString());
        }
    }
}