using System.Collections;
using UnityEngine;

namespace Game.Weapon {

	public class ManagerAk47 : WeaponStandart {

		private float _timeLastShoot = -1000;
		private bool isShoot = false;

		private Vector3 _shootTarget;
		private float damageKoeff;
		private float damage;
		private int serialShoot = 0;

		public override void Inicialized() {
			base.Inicialized();
			isShoot = false;
		}

		protected override void OnClickDown(Vector3 position) {
			if (!CheckReadyShoot(position, position))
				return;

			isShoot = true;
			serialShoot = 0;
			_shootTarget = position;

		}

		protected override void PlayEmptyAudioBlock() {
			base.PlayEmptyAudioBlock();
			DarkTonic.MasterAudio.MasterAudio.PlaySound("Weapon_AK47_Empty");
		}

		protected override void OnClickUp(Vector3 position) {
			isShoot = false;
		}

    protected override void OnClickDrag(Vector3 position, Vector3 delta) {
      base.OnClickDrag(position, delta);
      pointerDown = position;
    }

    protected override void Update() {
			if (!isShoot)
				return;

			if (Magazin <= 0) {
				//Reload();
				SetActiveStatus(true);
				//OnShoot(tapStart, tapEnd);
				isShoot = false;
				PlayReload();
				OnShootComplited();
				return;
			}

			if (_timeLastShoot + TimeBetweenShoot / 4 > Time.time)
				return;

			if (pointerDown.x < PlayerController.Instance.ShootPoint.x)
				pointerDown.x = PlayerController.Instance.ShootPoint.x;

			serialShoot++;

			PlaygunShootAudio();

			GameObject weapon = PoolerManager.Spawn(bulletPrefab.name);

			weapon.transform.position = PlayerController.Instance.ShootPoint;

			weapon.SetActive(true);
			weapon.GetComponent<Bullet>().Shot(PlayerController.Instance.ShootPoint, NewTapPosition(serialShoot, PlayerController.Instance.ShootPoint, pointerDown));
			BulletDecrement();
			PlayShootAudio();

			_timeLastShoot = Time.time;
		}

		private Vector2 NewTapPosition(int serialShoot, Vector3 tapStart, Vector3 tapEnd) {

			serialShoot -= 3;
			if (serialShoot <= 0)
				serialShoot = 0;

			if (serialShoot == 0)
				return tapEnd;

			float angle = 2 * serialShoot;
			float anglRadian = angle * (Mathf.PI / 180);

			Vector2 alterTapEnd = tapEnd - tapStart;
			tapEnd = new Vector3(alterTapEnd.x * Mathf.Cos(anglRadian) - alterTapEnd.y * Mathf.Sin(anglRadian),
								 alterTapEnd.x * Mathf.Sin(anglRadian) + alterTapEnd.y * Mathf.Cos(anglRadian),
								 0) + tapStart;
			return tapEnd;
		}

		private void Reload() {
			WeaponGenerator.Instance.changeWeapon = false;
		}

		public AudioBlock gunShootudioBlock;
		protected virtual void PlaygunShootAudio() {
			gunShootudioBlock.PlayRandom(this);
		}

		protected override void OnReloadStart() {
			base.OnReloadStart();

			InvokeCustom(_timeReload - 1, () => {
				DarkTonic.MasterAudio.MasterAudio.PlaySound("Weapon_AK47_Reload");
			});
			
		}

		#region Настройки

		public override void GetConfig() {
			base.GetConfig();

			damage = wepConfig.damage.Value;

		}
		#endregion

	}

}