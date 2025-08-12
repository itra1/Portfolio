using UnityEngine;
using System.Collections;

namespace ExEvent {

	public class UserEvents {
		
		public sealed class OnCoins : BaseEvent {
			public int value;

			public OnCoins(int value) {
				this.value = value;
			}

			public static void Call(int value) {
				BaseEvent.Call(new OnCoins(value));
			}

			public static void CallAsync(int value) {
				BaseEvent.CallAsync(new OnCoins(value));
			}

		}

		public sealed class OnHealthLevel : BaseEvent {
			public int value;

			public OnHealthLevel(int value) {
				this.value = value;
			}

			public static void Call(int value) {
				BaseEvent.Call(new OnHealthLevel(value));
			}

			public static void CallAsync(int value) {
				BaseEvent.CallAsync(new OnHealthLevel(value));
			}
		}

        public sealed class OnEnergyLevel : BaseEvent
        {
            public int value;

            public OnEnergyLevel(int value) {
                this.value = value;
            }

            public static void Call(int value) {
                BaseEvent.Call(new OnEnergyLevel(value));
            }

            public static void CallAsync(int value) {
                BaseEvent.CallAsync(new OnEnergyLevel(value));
            }
        }

        public sealed class OnPowerLevel : BaseEvent {
			public int value;

			public OnPowerLevel(int value) {
				this.value = value;
			}

			public static void Call(int value) {
				BaseEvent.Call(new OnPowerLevel(value));
			}

			public static void CallAsync(int value) {
				BaseEvent.CallAsync(new OnPowerLevel(value));
			}
		}

		public sealed class OnLevelChange : BaseEvent {
			public int value;

			public OnLevelChange(int value) {
				this.value = value;
			}

			public static void Call(int value) {
				BaseEvent.Call(new OnLevelChange(value));
			}

			public static void CallAsync(int value) {
				BaseEvent.CallAsync(new OnLevelChange(value));
			}
		}
		
	}
}