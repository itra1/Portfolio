using UnityEngine;
using System.Collections;

namespace ExEvent {

	public class BattleEvents {

		public sealed class StartBattle : BaseEvent {
			public static void Call() {
				BaseEvent.Call(new StartBattle());
			}
		}
		
		public sealed class BattlePhaseChange : BaseEvent {
			public BattlePhase phase;

			public BattlePhaseChange(BattlePhase phase) {
				this.phase = phase;
			}
			
			public static void Call(BattlePhase phase) {
				BaseEvent.Call(new BattlePhaseChange(phase));
			}
		}

		public sealed class Pause : BaseEvent {
			public bool isPause;

			public Pause(bool isPause) {
				this.isPause = isPause;
			}

			public static void Call(bool isPause) {
				BaseEvent.Call(new Pause(isPause));
			}
		}

		public sealed class NumberTimer : BaseEvent {
			public int num;

			public NumberTimer(int num) {
				this.num = num;
			}

			public static void Call(int num) {
				BaseEvent.Call(new NumberTimer(num));
			}
		}

		public sealed class ChangeKeyEnemy : BaseEvent {
			public bool useKeyEnemy;			// Есть ключевой враг
			public EnemyType enemyType;		// Есть енемю тип

			public ChangeKeyEnemy(bool useKeyEnemy, EnemyType enemyType) {
				this.useKeyEnemy = useKeyEnemy;
				this.enemyType = enemyType;
			}

			public static void Call(bool useKeyEnemy, EnemyType enemyType) {
				BaseEvent.Call(new ChangeKeyEnemy(useKeyEnemy, enemyType));
			}
		}

		public sealed class GenerateKeyEnemy : BaseEvent {
			public Enemy enemy;

			public GenerateKeyEnemy(Enemy enemy) {
				this.enemy = enemy;
			}

			public static void Call(Enemy enemy) {
				BaseEvent.Call(new GenerateKeyEnemy(enemy));
			}
		}

		public sealed class OnCloseBattle : BaseEvent {

			public OnCloseBattle() {
			}

			public static void Call() {
				BaseEvent.Call(new OnCloseBattle());
			}
		}

	}
}