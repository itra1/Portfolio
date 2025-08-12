using Cysharp.Threading.Tasks;
using Game.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Game.Elements.Weapons
{
	public interface IWeapon : ITransform
	{
		UnityAction<IWeapon> OnBoardEvent { get; set; }
		UnityAction<IWeapon> OnWeaponRecapturedEvent { get; set; }
		UnityAction<IWeapon> OnKnockOut { get; set; }
		UnityAction<IWeapon> OnLossHit { get; set; }

		Vector3 ShootVector { get; set; }
		bool IsShootReady { get; }

		void OnSpawned();
		void Remove();
		void SetMode(int mode);
		UniTask ShootReady();
		void Shoot();
		void WeaponRecaptured();
		void CriticalHit();
	}
}