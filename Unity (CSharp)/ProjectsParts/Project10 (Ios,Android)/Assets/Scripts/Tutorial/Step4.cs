using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial.Steps {
	public class Step4 : Step {
		public override int step {
			get { return 4; }
		}

		public override string yaMetrica {
			get { return "Туториал: выполнен урок большой ямы"; }
		}

		public override string gaDescr {
			get { return "Урок большой ямы"; }
		}

		private bool isDown = false;
		public override void StartStep() {
			base.StartStep();

			InvokeMethod(GenerateBreacMax, 1);
		}

		void GenerateBreacMax() {
			isDown = false;
			RunSpawner.GenerateBreackNow(8);
		}

		[ExEvent.ExEventHandler(typeof(ExEvent.PlayerEvents.PitDown))]
		private void PlayerPitDown(ExEvent.PlayerEvents.PitDown eventData) {
			isDown = true;
			InvokeMethod(GenerateBreacMax, 4);

		}

		[ExEvent.ExEventHandler(typeof(ExEvent.PlayerEvents.PitJump))]
		private void PlayerPitJump(ExEvent.PlayerEvents.PitJump eventData) {
			if(!isDown)
				InvokeMethod(StepComplete, 1);

		}


	}
}