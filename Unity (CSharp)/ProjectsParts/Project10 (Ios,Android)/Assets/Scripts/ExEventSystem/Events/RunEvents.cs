using System;
using UnityEngine;
using System.Collections;

namespace ExEvent {

	public class RunEvents {
    /// <summary>
    /// Изменение фазы забега
    /// </summary>
    public sealed class RunPhaseChange: BaseEvent {
      public RunnerPhase oldPhase;
      public RunnerPhase newPhase;

      public RunPhaseChange(RunnerPhase oldPhase, RunnerPhase newPhase) {
        this.oldPhase = oldPhase;
        this.newPhase = newPhase;
      }

      public static void Call(RunnerPhase oldPhase, RunnerPhase newPhase) {
        BaseEvent.Call(new RunPhaseChange(oldPhase, newPhase));
      }

      public static void CallAsync(RunnerPhase oldPhase, RunnerPhase newPhase) {
        BaseEvent.CallAsync(new RunPhaseChange(oldPhase, newPhase));
      }

    }
    /// <summary>
    /// Окончание обрыва
    /// </summary>
    public sealed class BreackEnd: BaseEvent {
      public BreackEnd() {   }

      public static void Call() {
        BaseEvent.Call(new BreackEnd());
      }

      public static void CallAsync() {
        BaseEvent.CallAsync(new BreackEnd());
      }

    }

    public sealed class TapScreen : BaseEvent {

			public static void Call() {
				BaseEvent.Call(new TapScreen());
			}
		}

		public sealed class StartFirstRunAnim : BaseEvent {
			public static void Call() {
				BaseEvent.Call(new StartFirstRunAnim());
			}
		}

		/// <summary>
		/// Coins Change
		/// </summary>
		public sealed class CoinsChange : BaseEvent {

			public int coins;

			public CoinsChange(int coins) {
				this.coins = coins;
			}
			public static void Call(int coins) {
				BaseEvent.Call(new CoinsChange(coins));
			}

			public static void CallAsync(int coins) {
				BaseEvent.CallAsync(new CoinsChange(coins));
			}

		}

		/// <summary>
		/// Cristall Change
		/// </summary>
		public sealed class CristallChange : BaseEvent {

			public int cristall;

			public CristallChange(int cristall) {
				this.cristall = cristall;
			}
			public static void Call(int cristall) {
				BaseEvent.Call(new CristallChange(cristall));
			}

			public static void CallAsync(int cristall) {
				BaseEvent.CallAsync(new CristallChange(cristall));
			}
		}

		/// <summary>
		/// BlackMark Change
		/// </summary>
		public sealed class BlackMarkChange : BaseEvent {

			public int blackMark;

			public BlackMarkChange(int blackMark) {
				this.blackMark = blackMark;
			}
			public static void Call(int blackMark) {
				BaseEvent.Call(new BlackMarkChange(blackMark));
			}

			public static void CallAsync(int blackMark) {
				BaseEvent.CallAsync(new BlackMarkChange(blackMark));
			}
		}

		/// <summary>
		/// Keys Change
		/// </summary>
		public sealed class KeysChange : BaseEvent {

			public int keys;

			public KeysChange(int keys) {
				this.keys = keys;
			}
			public static void Call(int keys) {
				BaseEvent.Call(new KeysChange(keys));
			}

			public static void CallAsync(int keys) {
				BaseEvent.CallAsync(new KeysChange(keys));
			}
		}

		/// <summary>
		/// Изменение региона
		/// </summary>
		public sealed class RegionChange : BaseEvent {

			public RegionType newType;

			public RegionChange(RegionType regionType) {
        this.newType = regionType;
			}
			public static void Call(RegionType regionType) {
				BaseEvent.Call(new RegionChange(regionType));
			}

			public static void CallAsync(RegionType regionType) {
				BaseEvent.CallAsync(new RegionChange(regionType));
			}
		}

		public sealed class SpecialPlatform : BaseEvent {

			public bool isActivate;
			public int typ;
			public Action callback;

			public SpecialPlatform(bool isActivate, int typ, Action callback = null) {
				this.isActivate = isActivate;
				this.typ = typ;
				this.callback = callback;
			}
			public static void Call(bool isActivate, int typ, Action callback = null) {
				BaseEvent.Call(new SpecialPlatform(isActivate, typ, callback));
			}

			public static void CallAsync(bool isActivate, int typ, Action callback = null) {
				BaseEvent.CallAsync(new SpecialPlatform(isActivate, typ, callback));
			}
		}

		public sealed class SpecialBarrier : BaseEvent {

			public bool isActivate;
			public SpecialBarriersTypes? barrier;

			public SpecialBarrier(bool isActivate, SpecialBarriersTypes? barrier) {
				this.isActivate = isActivate;
				this.barrier = barrier;
			}
			public static void Call(bool isActivate, SpecialBarriersTypes? barrier) {
				BaseEvent.Call(new SpecialBarrier(isActivate, barrier));
			}

			public static void CallAsync(bool isActivate, SpecialBarriersTypes? barrier) {
				BaseEvent.CallAsync(new SpecialBarrier(isActivate, barrier));
			}
		}

		public sealed class CoinsGenerate : BaseEvent {

			public Vector2 position;
			public int barrierType;

			public CoinsGenerate(Vector2 position, int barrierType = 0) {
				this.position = position;
				this.barrierType = barrierType;
			}
			public static void Call(Vector2 position, int barrierType = 0) {
				BaseEvent.Call(new CoinsGenerate(position, barrierType));
			}

			public static void CallAsync(Vector2 position, int barrierType = 0) {
				BaseEvent.CallAsync(new CoinsGenerate(position, barrierType));
			}
		}

		/// <summary>
		/// Событие окончания забега
		/// </summary>
		public sealed class LevelEnd: BaseEvent {

			public static void Call() {
				BaseEvent.Call(new LevelEnd());
			}

			public static void CallAsync() {
				BaseEvent.CallAsync(new LevelEnd());
			}

		}
		
	}
}