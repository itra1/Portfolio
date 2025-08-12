using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExEvent {

	public class PlayerEvents {

		/// <summary>
		/// Панение в яму
		/// </summary>
		public sealed class PitDown : BaseEvent {

			public PitDown() {
			}

			public static void Call() {
				BaseEvent.Call(new PitDown());
			}

			public static void CallAsync() {
				BaseEvent.CallAsync(new PitDown());
			}

		}

		/// <summary>
		/// Получение урона
		/// </summary>
		public sealed class Damage : BaseEvent {

			public Damage() {
			}

			public static void Call() {
				BaseEvent.Call(new Damage());
			}

			public static void CallAsync() {
				BaseEvent.CallAsync(new Damage());
			}

		}

		public sealed class PitJump : BaseEvent {

			public PitJump() {
			}

			public static void Call() {
				BaseEvent.Call(new PitJump());
			}

			public static void CallAsync() {
				BaseEvent.CallAsync(new PitJump());
			}

		}

	}

}