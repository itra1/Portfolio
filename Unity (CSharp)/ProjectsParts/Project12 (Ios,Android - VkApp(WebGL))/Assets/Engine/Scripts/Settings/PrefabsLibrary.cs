using UnityEngine;

namespace Engine.Scripts.Settings
{
	[System.Serializable]
	public class PrefabsLibrary
	{
		[SerializeField] private GameObject _songBigUiElement;

		public GameObject SongBigUiElement => _songBigUiElement;
	}
}
