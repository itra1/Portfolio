using Core.Engine.Factorys.Base;
using Scripts.GameItems.Platforms;
using UnityEngine;

namespace Scripts.Common
{
	public class PlatformFactory :PrefabPoolFactory<Platform>
	{
		private SceneComponents _sceneComponents;

		public PlatformFactory(SceneComponents sceneComponents)
		{
			_sceneComponents = sceneComponents;

			var prefab = Resources.Load<GameObject>("Prefabs/Platforms/Platform");
			InitPooler(prefab.GetComponent<Platform>(), sceneComponents.PlatformParent);
			_prefabPooler.InitInstances(10);

		}

		public override Platform GetInstance()
		{
			var item = base.GetInstance();
			_container.Inject(item);
			return item;
		}

		public void HideAll()
		{
			_prefabPooler?.HideAll();
		}
	}
}
