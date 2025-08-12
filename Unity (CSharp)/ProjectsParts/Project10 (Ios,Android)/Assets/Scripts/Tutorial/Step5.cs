using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial.Steps {
	public class Step5 : Step {
		public override int step {
			get { return 5; }
		}

		public override string yaMetrica {
			get { return "Туториал: выполнен урок уворачивания от копья"; }
		}

		public override string gaDescr {
			get { return "Урок уворачивания от копья"; }
		}

		public override void StartStep() {
			base.StartStep();

			InvokeMethod(GenerateEnemy, 4);
		}
		private bool isShoot;

		GameObject enemy;
		void GenerateEnemy() {
			enemy = EnemySpawner.GenerateTutor(EnemyTypes.aztecSpear);
			InvokeMethod(EnemyShoot, 5);
		}

		void EnemyShoot() {
			if (enemy != null) {
				isShoot = true;
				enemy.GetComponent<GeneralEnemy>().OnShoot();
			}
		}


		[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.SpearGround))]
		private void SpearGround(ExEvent.GameEvents.SpearGround eventData) {
			if(isShoot)
				InvokeMethod(StepComplete, 1);
			else
				InvokeMethod(EnemyShoot, 1);

		}

		[ExEvent.ExEventHandler(typeof(ExEvent.PlayerEvents.Damage))]
		private void PlayerDamage(ExEvent.PlayerEvents.Damage eventData) {

			isShoot = false;
		}


	}

}