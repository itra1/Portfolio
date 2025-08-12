using Game.Game.Elements.Weapons;
using UnityEngine;

namespace Game.Providers.Battles.Components
{
	public abstract class BattleDuelPlayerBaseState
	{
		public int WinCount { get; set; }
		public Texture2D Avatar { get; set; }
		public int Points { get; set; }
		public int BoardsHit { get; set; }
		public int WinCoins { get; set; }
		public IWeapon SelectedWeapon { get; set; }
	}
}
