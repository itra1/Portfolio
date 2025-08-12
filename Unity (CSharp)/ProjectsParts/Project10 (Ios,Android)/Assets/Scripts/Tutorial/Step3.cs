using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial.Steps {
	public class Step3 : Step {
		public override int step {
			get { return 3; }
		}

		public override string yaMetrica {
			get { return "Туториал: выполнен урок малой ямы"; }
		}

		public override string gaDescr {
			get { return "Урок малой ямы"; }
		}

		private bool isDown = false;
		public override void StartStep() {
			base.StartStep();

			InvokeMethod(GenerateBreackMin, 1);
		}

		void GenerateBreackMin() {
			isDown = false;
			RunSpawner.GenerateBreackNow(5);
		}

		[ExEvent.ExEventHandler(typeof(ExEvent.PlayerEvents.PitDown))]
		private void PlayerPitDown(ExEvent.PlayerEvents.PitDown eventData) {

			isDown = true;
			InvokeMethod(GenerateBreackMin, 4);

		}

		[ExEvent.ExEventHandler(typeof(ExEvent.PlayerEvents.PitJump))]
		private void PlayerPitJump(ExEvent.PlayerEvents.PitJump eventData) {
			if (!isDown)
				InvokeMethod(StepComplete, 1);

		}


	}
}