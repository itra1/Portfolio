using UnityEngine;

namespace Game.Players.Skins {
	public class RandomSkinMesh : SkinMesh {
		[SerializeField] private GameObject[] _objects;

		protected override void AfterInitiate() {
			base.AfterInitiate();

			int index = Random.Range(0, _objects.Length);

			for (int i = 0; i < _objects.Length; i++) {
				_objects[i].gameObject.SetActive(i == index);
			}
		}
	}
}
