using ModestTree;
using UnityEngine;

namespace Engine.Assets.Engine.Scripts.Timelines.NotesDestroy
{
	public class NotePartsMesh : MonoBehaviour
	{
		[SerializeField] private Material _materialPrefab;

		private MeshRenderer[] _renderers;
		private Material _material;

		private void Initiate()
		{
			if (_renderers.IsEmpty())
				_renderers = GetComponentsInChildren<MeshRenderer>();

			if (_material == null)
			{
				_material = Instantiate(_materialPrefab);
				foreach (var item in _renderers)
					item.sharedMaterial = _material;
			}
		}

		public void SetTexture(Texture2D tex)
			=> _material.SetTexture("_BaseTex", tex);

		private void SetColor(Color color)
			=> _material.SetColor("_Color", color);

		private void ResetPositionsComponent()
		{
			Initiate();
			SetColor(Color.white);

			foreach (var item in _renderers)
			{
				item.transform.localPosition = Vector3.zero;
				item.transform.rotation = Quaternion.identity;
			}
		}
	}
}
