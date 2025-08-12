using Core.Engine.Elements.Player;

using UnityEngine;

namespace Assets.Scripts.GameItems.Platforms
{
	public class PlatformDamage : MonoBehaviour, IPlayerDamage
	{
		public float PlayerDamageValue => 1;
	}
}
