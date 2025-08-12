using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial.Steps {
	public class Step8 : Step {
		public override int step {
			get { return 8; }
		}

		public override string yaMetrica {
			get { return "Туториал: окончен"; }
		}

		public override string gaDescr {
			get { return "Туториал окончен"; }
		}


		public override void StartStep() {
			base.StartStep();

			InvokeMethod(StepComplete, 3);
		}

	}
}