using UnityEngine;

namespace Game.Scripts.Providers.Shop.Prices
{
	[System.Serializable]
	public class RealPrice : IPrice
	{
		[SerializeField] private float _value;
		[SerializeField] private float _oldValue;

		public float Value => _value;
		public float OldValue => _oldValue;
	}
}
