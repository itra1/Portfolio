using UnityEngine;

namespace Game.Scripts.Providers.Shop.Settings
{
	public abstract class ProductPropertyBase : ScriptableObject
	{
		[SerializeField] private string _title;
		public abstract string Type { get; }
		public string Title => _title;
	}
}
