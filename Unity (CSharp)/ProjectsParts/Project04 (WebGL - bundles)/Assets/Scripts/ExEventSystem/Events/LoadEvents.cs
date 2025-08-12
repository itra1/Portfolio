using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExEvent {
	public class LoadEvents {

		public sealed class LoadProgress : BaseEvent {
			public string info;
			public float progressValue = 0;

			public LoadProgress(string info, float progressValue) {
				this.info = info;
				this.progressValue = progressValue;
			}

			public static void Call(string info, float progressValue) {
				Call(new LoadProgress(info, progressValue));
			}

		}

		public sealed class StartLoadProgress : BaseEvent {

			public StartLoadProgress() {}

			public static void Call() {
				Call(new StartLoadProgress());
			}

		}
	}
}