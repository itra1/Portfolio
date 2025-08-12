using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExEvent {
	public class LoadEvents {

		public sealed class LoadProgress : BaseEvent {
			public float loadProgress;

			public LoadProgress(float loadProgress) {
				this.loadProgress = loadProgress;
			}

			public static void Call(float loadProgress) {
				BaseEvent.Call(new LoadProgress(loadProgress));
			}

		}
	}
}