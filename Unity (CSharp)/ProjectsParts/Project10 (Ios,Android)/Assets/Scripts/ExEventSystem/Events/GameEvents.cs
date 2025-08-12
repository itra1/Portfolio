using System;
using UnityEngine;
using System.Collections;

namespace ExEvent {

	public class GameEvents {

    public sealed class LevelChange: BaseEvent {
      public int level;

      public LevelChange(int level) {
        this.level = level;
      }

      public static void Call(int level) {
        BaseEvent.Call(new LevelChange(level));
      }

      public static void CallAsync(int level) {
        BaseEvent.CallAsync(new LevelChange(level));
      }

    }

    public sealed class QuestionComplete : BaseEvent {
			public Questions.Question question;

			public QuestionComplete(Questions.Question question) {
				this.question = question;
			}

			public static void Call(Questions.Question question) {
				BaseEvent.Call(new QuestionComplete(question));
			}

			public static void CallAsync(Questions.Question question) {
				BaseEvent.CallAsync(new QuestionComplete(question));
			}

		}

		public sealed class SetArmorPlayer : BaseEvent {
			public ClothesSets cloth;

			public SetArmorPlayer(ClothesSets cloth) {
				this.cloth = cloth;
			}

			public static void Call(ClothesSets cloth) {
				BaseEvent.Call(new SetArmorPlayer(cloth));
			}

			public static void CallAsync(ClothesSets cloth) {
				BaseEvent.CallAsync(new SetArmorPlayer(cloth));
			}

		}

		public sealed class PlayerLiveChange : BaseEvent {
			public float value;
			public bool noAnimActive = false;
			public bool first = false;

			public PlayerLiveChange(float value, bool noAnimActive = false, bool first = false) {
				this.value = value;
				this.noAnimActive = noAnimActive;
				this.first = first;
		}

			public static void Call(float value, bool noAnimActive = false, bool first = false) {
				BaseEvent.Call(new PlayerLiveChange(value, noAnimActive, first));
			}

			public static void CallAsync(float value, bool noAnimActive = false, bool first = false) {
				BaseEvent.CallAsync(new PlayerLiveChange(value, noAnimActive, first));
			}

		}

		public sealed class WeaponActiveChange : BaseEvent {
			public WeaponTypes weapon;
			public int count;
			public bool first = false;

			public WeaponActiveChange(WeaponTypes weapon, int count, bool first = false) {
				this.weapon = weapon;
				this.count = count;
				this.first = first;
			}

			public static void Call(WeaponTypes weapon, int count, bool first = false) {
				BaseEvent.Call(new WeaponActiveChange(weapon, count, first));
			}

			public static void CallAsync(WeaponTypes weapon, int count, bool first = false) {
				BaseEvent.CallAsync(new WeaponActiveChange(weapon, count, first));
			}

		}

		public sealed class MagicWeaponChange : BaseEvent {
			public WeaponTypes weapon;
			public int count;
			public bool first = false;

			public MagicWeaponChange(WeaponTypes weapon, int count, bool first = false) {
				this.weapon = weapon;
				this.count = count;
				this.first = first;
			}

			public static void Call(WeaponTypes weapon, int count, bool first = false) {
				BaseEvent.Call(new MagicWeaponChange(weapon, count, first));
			}

			public static void CallAsync(WeaponTypes weapon, int count, bool first = false) {
				BaseEvent.CallAsync(new MagicWeaponChange(weapon, count, first));
			}

		}

		public sealed class SpearGround : BaseEvent {

			public SpearGround() {
			}

			public static void Call() {
				BaseEvent.Call(new SpearGround());
			}

			public static void CallAsync() {
				BaseEvent.CallAsync(new SpearGround());
			}

		}

		public sealed class SpearMirrow : BaseEvent {

			public SpearMirrow() {
			}

			public static void Call() {
				BaseEvent.Call(new SpearMirrow());
			}

			public static void CallAsync() {
				BaseEvent.CallAsync(new SpearMirrow());
			}

		}

		public sealed class BoxLeftDestroy : BaseEvent {

			public BoxController box;

			public BoxLeftDestroy(BoxController box) {
				this.box = box;
			}

			public static void Call(BoxController box) {
				BaseEvent.Call(new BoxLeftDestroy(box));
			}

			public static void CallAsync(BoxController box) {
				BaseEvent.CallAsync(new BoxLeftDestroy(box));
			}

		}

		public sealed class SwordLeftDestroy : BaseEvent {
			
			public SwordLeftDestroy() {
			}

			public static void Call() {
				BaseEvent.Call(new SwordLeftDestroy());
			}

			public static void CallAsync() {
				BaseEvent.CallAsync(new SwordLeftDestroy());
			}

		}



	}
}