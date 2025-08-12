using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial.Steps {

	public class Step2 : Step {
		public override int step {
			get { return 2; }
		}

		public override string yaMetrica {
			get { return "Туториал: выполнен урок препядствий"; }
		}

		public override string gaDescr { get { return "Урок препядствия"; } }

		public GameObject prefab;

		public override void StartStep() {
			base.StartStep();

			InvokeMethod(GenerateBarrier, 1);
		}

		void GenerateBarrier() {
			GameObject inst =
				Instantiate(prefab,
					new Vector3(CameraController.displayDiff.rightDif(1.3f), CameraController.displayDiff.transform.position.y - 2, 0),
					Quaternion.identity) as GameObject;
			inst.SetActive(true);

			inst.SetActive(true);
			inst.GetComponent<TutorialTarget1>().SetCallback(StepMoverComplete, StepMoverFailed);

		}

		private void StepMoverComplete() {

			InvokeMethod(StepComplete, 1);

		}

		private void StepMoverFailed() {
			GenerateBarrier();
		}

	}
}