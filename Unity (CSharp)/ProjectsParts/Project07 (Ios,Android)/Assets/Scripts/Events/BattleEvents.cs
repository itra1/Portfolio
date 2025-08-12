using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ExEvent {

	public class BattleEvents {

    public sealed class BattleStart: BaseEvent {

      public BattleStart() { }

      public static void Call() {
        BaseEvent.Call(new BattleStart());
      }

      public static void CallAsync() {
        BaseEvent.CallAsync(new BattleStart());
      }

    }

    public sealed class BattlePhaseChange: BaseEvent {
      public BattleController.BattlePhase phase;
      public BattleController.BattlePhase oldPhase;

      public BattlePhaseChange(BattleController.BattlePhase phase, BattleController.BattlePhase oldPhase) {
        this.phase = phase;
        this.oldPhase = oldPhase;
      }

      public static void Call(BattleController.BattlePhase phase, BattleController.BattlePhase oldPhase) {
        BaseEvent.Call(new BattlePhaseChange(phase, oldPhase));
      }

      public static void CallAsync(BattleController.BattlePhase phase, BattleController.BattlePhase oldPhase) {
        BaseEvent.CallAsync(new BattlePhaseChange(phase, oldPhase));
      }

    }

		public sealed class EnemyClick : BaseEvent {
			public Enemy enemy;

			public EnemyClick(Enemy enemy) {
				this.enemy = enemy;
			}

			public static void Call(Enemy enemy) {
				BaseEvent.Call(new EnemyClick(enemy));
			}

			public static void CallAsync(Enemy enemy) {
				BaseEvent.CallAsync(new EnemyClick(enemy));
			}

		}

		public sealed class EnemyDead : BaseEvent {
			public Enemy enemy;

			public EnemyDead(Enemy enemy) {
				this.enemy = enemy;
			}

			public static void Call(Enemy enemy) {
				BaseEvent.Call(new EnemyClick(enemy));
			}

			public static void CallAsync(Enemy enemy) {
				BaseEvent.CallAsync(new EnemyClick(enemy));
			}

		}



		public sealed class NextWave : BaseEvent {
			public bool isBoss;
			public int wave, allWave;

			public NextWave(bool isBoss, int wave, int allWave) {
				this.isBoss = isBoss;
				this.wave = wave;
				this.allWave = allWave;
			}

			public static void Call(bool isBoss, int wave, int allWave) {
				BaseEvent.Call(new NextWave(isBoss, wave, allWave));


			}

			public static void CallAsync(bool isBoss, int wave, int allWave) {
				BaseEvent.CallAsync(new NextWave(isBoss, wave, allWave));
			}

		}


		public sealed class WeaponChange : BaseEvent {

      public WeaponBehaviour weapon;

      public WeaponChange(WeaponBehaviour weapon) {
				this.weapon = weapon;
				
			}

			public static void Call(WeaponBehaviour weapon) {
				BaseEvent.Call(new WeaponChange(weapon));


			}

			public static void CallAsync(WeaponBehaviour weapon) {
				BaseEvent.CallAsync(new WeaponChange(weapon));
			}

		}

    public sealed class WeaponGroupChange: BaseEvent {

      public WeaponGroup group;

      public WeaponGroupChange(WeaponGroup group) {
        this.group = group;

      }

      public static void Call(WeaponGroup group) {
        BaseEvent.Call(new WeaponGroupChange(group));

      }

      public static void CallAsync(WeaponGroup group) {
        BaseEvent.CallAsync(new WeaponGroupChange(group));
      }

    }


    /// <summary>
    /// Готовый список орудий
    /// </summary>
    public sealed class ReadyWeapon: BaseEvent {

      public List<WeaponBehaviour> weapons;

      public ReadyWeapon(List<WeaponBehaviour> weapons) {
        this.weapons = weapons;

      }

      public static void Call(List<WeaponBehaviour> weapons) {
        BaseEvent.Call(new ReadyWeapon(weapons));

      }

      public static void CallAsync(List<WeaponBehaviour> weapons) {
        BaseEvent.CallAsync(new ReadyWeapon(weapons));
      }

    }

    public sealed class WeaponShoot: BaseEvent {

      public WeaponBehaviour weapon;
      public Enemy enemy;

      public WeaponShoot(WeaponBehaviour weapon, Enemy enemy) {
        this.weapon = weapon;
        this.enemy = enemy;
      }

      public static void Call(WeaponBehaviour weapon, Enemy enemy) {
        BaseEvent.Call(new WeaponShoot(weapon, enemy));

      }

      public static void CallAsync(WeaponBehaviour weapon, Enemy enemy) {
        BaseEvent.CallAsync(new WeaponShoot(weapon, enemy));
      }

    }

  }
}