using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial.Steps {

	public class Step1 : Step {

		public override int step { get { return 1; } }

		private int moverStep = 0;

		public override string yaMetrica { get { return "Туториал: выполнен урок перемещения"; } }
		
		public override string gaDescr { get { return "Урок перемещение"; } }

		public GameObject prefab;

		public override void StartStep() {
			base.StartStep();

			moverStep++;

			InvokeMethod(SpawnMover,1);
		}

		private GameObject inst;
		private void SpawnMover() {

			inst = Instantiate(prefab);

			if (moverStep == 1) {
				if(inst != null)
				inst.transform.position = new Vector3(CameraController.displayDiff.rightDif(0.7f),
					CameraController.displayDiff.transform.position.y - 1, 0);
			}
			else {
				if (inst != null)
					inst.transform.position = new Vector3(CameraController.displayDiff.transform.position.x,
					CameraController.displayDiff.transform.position.y - 1, 0);
			}

			if (inst != null)
				inst.SetActive(true);

			if (inst != null)
				inst.GetComponent<TutorialTarget1>().SetCallback(StepMoverComplete, StepMoverFailed);

		}

		private void StepMoverComplete() {

			inst.GetComponent<TutorialTarget1>().SetCallback(null, null);
			moverStep++;

			Debug.Log("StepMoverComplete");

			if (moverStep == 2) {
				InvokeMethod(SpawnMover, 1.5f);
			}
			else {
				InvokeMethod(StepComplete, 1);
			}

		}

		private void StepMoverFailed() {
			//SpawnMover();
		}

		//public void GetMuver() {

		//	if (moverStep == 0) {
		//		GameObject inst =
		//			Instantiate(muver1,
		//				new Vector3(CameraController.displayDiff.rightDif(0.7f), CameraController.displayDiff.transform.position.y - 1, 0),
		//				Quaternion.identity) as GameObject;
		//		muver++;
		//		inst.SetActive(true);
		//	} else if (muver == 1) {
		//		GameObject inst =
		//			Instantiate(muver1,
		//				new Vector3(CameraController.displayDiff.transform.position.x,
		//					CameraController.displayDiff.transform.position.y - 1, 0), Quaternion.identity) as GameObject;
		//		muver++;
		//		inst.SetActive(true);
		//	} else if (muver == 2) {
		//		NextStep();
		//		muver++;
		//	}

		//}


	}

}