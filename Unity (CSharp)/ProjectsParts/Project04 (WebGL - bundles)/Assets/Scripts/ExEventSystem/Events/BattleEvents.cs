using System.Collections;
using System.Collections.Generic;
using Cells;
using Network.Input;
using UnityEngine;

namespace ExEvent {

	public class BattleEvents {

		public sealed class StartBattle : BaseEvent {
			public Network.Input.BattleUpdate battleStart;

			public StartBattle(Network.Input.BattleUpdate battleStart) {
				this.battleStart = battleStart;
			}

			public static void Call(Network.Input.BattleUpdate battleStart) {
				BaseEvent.Call(new StartBattle(battleStart));
			}

    }
    public sealed class LoadNewCells: BaseEvent {

      public LoadNewCells() {
      }

      public static void Call() {
        BaseEvent.Call(new LoadNewCells());
      }

    }
    public sealed class LoadAllModels : BaseEvent {

			public LoadAllModels() {
			}

			public static void Call() {
				BaseEvent.Call(new LoadAllModels());
			}

			public static void CallAsync() {
				BaseEvent.CallAsync(new LoadAllModels());
			}

		}

		public sealed class BattleUpdate : BaseEvent {
			public Network.Input.BattleUpdate battleStart;

			public BattleUpdate(Network.Input.BattleUpdate battleStart) {
				this.battleStart = battleStart;
			}

			public static void Call(Network.Input.BattleUpdate battleStart) {
				BaseEvent.Call(new BattleUpdate(battleStart));
			}

		}

		public sealed class BattleRoundChange : BaseEvent {
			public int battleRound;

			public BattleRoundChange(int battleRound) {
				this.battleRound = battleRound;
			}

			public static void Call(int battleRound) {
				BaseEvent.Call(new BattleRoundChange(battleRound));
			}

		}

		public sealed class BattleEnd : BaseEvent {

			public BattleEnd() {}

			public static void Call() {
				BaseEvent.Call(new BattleEnd());
			}

		}

		public sealed class OnChangeEnemyLight : BaseEvent {

			public OnChangeEnemyLight() { }

			public static void Call() {
				BaseEvent.Call(new OnChangeEnemyLight());
			}

		}
		public sealed class OnChangeFriendLight : BaseEvent {

			public OnChangeFriendLight() { }

			public static void Call() {
				BaseEvent.Call(new OnChangeFriendLight());
			}

		}
		public sealed class MainPlayerCreate : BaseEvent {

			public MainPlayerCreate() { }

			public static void Call() {
				BaseEvent.Call(new MainPlayerCreate());
			}

		}


		public sealed class OnErrorAttack : BaseEvent {

			public string text;

			public OnErrorAttack(string text) {
				this.text = text;
			}

			public static void Call(string text) {
				BaseEvent.Call(new OnErrorAttack(text));
			}

		}
		public sealed class OnPlayerWeaponChange : BaseEvent {
			
			public OnPlayerWeaponChange() {}

			public static void Call() {
				BaseEvent.Call(new OnPlayerWeaponChange());
			}

		}

		public sealed class OnChangeIsOsada : BaseEvent {

			public OnChangeIsOsada() { }

			public static void Call() {
				BaseEvent.Call(new OnChangeIsOsada());
			}

		}
		public sealed class OnChangeActionMode : BaseEvent {

			public OnChangeActionMode() { }

			public static void Call() {
				BaseEvent.Call(new OnChangeActionMode());
			}

		}

	}
}