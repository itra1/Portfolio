using UnityEngine;

namespace Game.Game.Elements.Weapons.Settings
{
	[System.Serializable]
	public class WeaponSettings : IWeaponSettings
	{
		[SerializeField] private string _prefabsPath;
		public string PrefabsPath => _prefabsPath;
	}
}
