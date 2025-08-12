using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExEvent {
	public class TutorialEvents {

		public sealed class LevelLoad : BaseEvent {

			public LevelLoad() {}

			public static void Call() {
				BaseEvent.Call(new LevelLoad());
			}

		}

		public sealed class PlayMovePointer : BaseEvent {

			public PlayMovePointer() { }

			public static void Call() {
				BaseEvent.Call(new PlayMovePointer());
			}

		}

		public sealed class TutorialEnd : BaseEvent {

			public TutorialEnd() { }

			public static void Call() {
				BaseEvent.Call(new TutorialEnd());
			}

		}

	}
}