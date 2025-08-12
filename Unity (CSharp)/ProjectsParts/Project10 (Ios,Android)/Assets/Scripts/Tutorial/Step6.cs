using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial.Steps {
	public class Step6 : Step {
		public override int step {
			get { return 6; }
		}

		public override string yaMetrica {
			get { return "Туториал: выполнен урок отражения копья"; }
		}

		public override string gaDescr {
			get { return "Урок отражения от копья"; }
		}

		private GeneralEnemy enemy;
		private bool isShoot;
		public override void StartStep() {
			base.StartStep();

			enemy = GameObject.FindObjectOfType<GeneralEnemy>();

			InvokeMethod(EnemyShoot, 1);
		}

		void EnemyShoot() {
			if (enemy != null) {
				isShoot = true;
				enemy.OnShoot();
			}
		}


		[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.SpearGround))]
		private void SpearGround(ExEvent.GameEvents.SpearGround eventData) {
			InvokeMethod(EnemyShoot, 1);

		}

		[ExEvent.ExEventHandler(typeof(ExEvent.PlayerEvents.Damage))]
		private void PlayerDamage(ExEvent.PlayerEvents.Damage eventData) {
			InvokeMethod(EnemyShoot, 1);
		}

		[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.SpearMirrow))]
		private void SpearMirrow(ExEvent.GameEvents.SpearMirrow eventData) {
			InvokeMethod(StepComplete, 1);

		}

	}
}