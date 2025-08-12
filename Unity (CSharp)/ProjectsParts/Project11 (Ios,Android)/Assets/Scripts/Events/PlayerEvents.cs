using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ExEvent {
	public class PlayerEvents {

		// Изменение числа монет
		public sealed class CoinsChange : BaseEvent {
			public int count;

			public CoinsChange(int count) {
				this.count = count;
			}

			public static void Call(int count) {
				BaseEvent.Call(new CoinsChange(count));
			}

		}
		
		public sealed class HintFirstLetterChange : BaseEvent {
			public int count;

			public HintFirstLetterChange(int count) {
				this.count = count;
			}

			public static void Call(int count) {
				BaseEvent.Call(new HintFirstLetterChange(count));
			}

		}

		public sealed class HintEnyLetterChange : BaseEvent {
			public int count;

			public HintEnyLetterChange(int count) {
				this.count = count;
			}

			public static void Call(int count) {
				BaseEvent.Call(new HintEnyLetterChange(count));
			}

		}

		public sealed class SetUnlimitedHint : BaseEvent {
			public bool active;

			public SetUnlimitedHint(bool active) {
				this.active = active;
			}

			public static void Call(bool active) {
				BaseEvent.Call(new SetUnlimitedHint(active));
			}

		}

		public sealed class HintFirstWordChange : BaseEvent {
			public int count;

			public HintFirstWordChange(int count) {
				this.count = count;
			}

			public static void Call(int count) {
				BaseEvent.Call(new HintFirstWordChange(count));
			}

		}


		public sealed class StarsChange : BaseEvent {
			public int count;

			public StarsChange(int count) {
				this.count = count;
			}

			public static void Call(int count) {
				BaseEvent.Call(new StarsChange(count));
			}

		}

		public sealed class OnLoad : BaseEvent {

			public OnLoad() {}

			public static void Call() {
				BaseEvent.Call(new OnLoad());
			}

		}

		public sealed class OnChangeCompany : BaseEvent {
			public string language;
			public bool force;

			public OnChangeCompany(string language, bool force = false) {
				this.language = language;
				this.force = force;
			}

			public static void Call(string language, bool force = false) {
				BaseEvent.Call(new OnChangeCompany(language,force));
			}

		}

		public sealed class OnChangeTranslate : BaseEvent {
			public string language;

			public OnChangeTranslate(string language) {
				this.language = language;
			}

			public static void Call(string language) {
				BaseEvent.Call(new OnChangeTranslate(language));
			}

		}

		public sealed class OnByeLocation : BaseEvent {
			public int? locationNum;
			public bool force;

			public OnByeLocation(int? locationNum, bool force = false) {
				this.locationNum = locationNum;
				this.force = force;
			}

			public static void Call(int? locationNum, bool force = false) {
				BaseEvent.Call(new OnByeLocation(locationNum, force));
			}

		}

		public sealed class OnAddDecor : BaseEvent {
			public int decorNum;

			public OnAddDecor(int decorNum) {
				this.decorNum = decorNum;
			}

			public static void Call(int decorNum) {
				BaseEvent.Call(new OnAddDecor(decorNum));
			}

		}



	}
}