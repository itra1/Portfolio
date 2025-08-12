using Game.Game.Elements.Weapons.Common;
using UnityEngine;

namespace Game.Game.Elements.Weapons.Elements
{
	public class Knife : Weapon
	{
		[SerializeField] private GameObject _fixedCollider;
		[SerializeField] private Sprite _myKnife;
		[SerializeField] private Sprite _opponentKnife;

		public override string Type => WeaponType.Knife;

		public override void SetMode(int mode)
		{
			base.SetMode(mode);
			_renderer.sprite = mode == 0 ? _myKnife : _opponentKnife;
		}

		protected override void SetOnBoard(bool isBoard)
		{
			base.SetOnBoard(isBoard);
			_fixedCollider.SetActive(isBoard || State == WeaponStates.Lock);
		}
	}
}
