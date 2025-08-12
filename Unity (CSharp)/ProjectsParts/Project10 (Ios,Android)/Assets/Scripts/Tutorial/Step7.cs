using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial.Steps {
	public class Step7 : Step {
		public override int step {
			get { return 7; }
		}

		public override string yaMetrica {
			get { return "Туториал: выполнен урок по убийству врага"; }
		}

		public override string gaDescr {
			get { return "Убийство врага"; }
		}
		private GeneralEnemy enemy;
		public override void StartStep() {
			base.StartStep();

			enemy = GameObject.FindObjectOfType<GeneralEnemy>();

			enemy.GetComponent<Enemy>().OnDead += EnemyDead;

			InvokeMethod(GenerateWeapon, 1);
		}

		public GameObject weaponPrefab;

		void GenerateWeapon() {
			GameObject obj =
				Instantiate(weaponPrefab,
					new Vector3(CameraController.displayDiff.transform.position.x + CameraController.displayDiff.right * 1.2f, 7f,
						transform.position.z),
					Quaternion.identity) as GameObject;
			obj.transform.parent = transform;
			obj.SetActive(true);
		}

		[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.BoxLeftDestroy))]
		private void BoxLeftDestroy(ExEvent.GameEvents.BoxLeftDestroy eventData) {
			InvokeMethod(GenerateWeapon, 1);

		}

		private void EnemyDead() {

			enemy.GetComponent<Enemy>().OnDead -= EnemyDead;
			InvokeMethod(StepComplete, 1);
		}

		[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.SwordLeftDestroy))]
		private void SwordLeftDestroy(ExEvent.GameEvents.SwordLeftDestroy eventData) {
			InvokeMethod(GenerateWeapon, 1);

		}


	}
}