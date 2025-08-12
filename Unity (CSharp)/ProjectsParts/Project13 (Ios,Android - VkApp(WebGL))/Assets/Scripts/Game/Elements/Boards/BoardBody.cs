using Game.Game.Elements.Interfaces;
using Game.Game.Elements.Weapons;
using UnityEngine;

namespace Game.Game.Elements.Boards
{
	public class BoardBody : MonoBehaviour, IBoardHandler, IWeaponCollision
	{

		[SerializeField] private Board _board;

		public Board Board => _board;
		public string GameCollisionType => GameCollisionName.Board;

		public void WeaponHit(Weapon knife)
		{
			_board.WeaponHit(knife);
		}

		public void KnockOut()
		{
		}
	}
}
