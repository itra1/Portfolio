using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Scripts.GameItems.Platforms
{
	public partial class PlatformFormations
	{
		[ContextMenu("SpawnRandomFormation")]
		public async UniTask SpawnRandomFormation()
		{
			for (var i = 0; i < _formations.Count; i++)
			{
				Debug.Log("spawn list");
				RemoveComponents();
				Debug.Log("spawn index " + i);
				SetFormation(i);
				await UniTask.Delay(3000);
				Debug.Log("Stop spawn list");
			}

		}

	}
}
