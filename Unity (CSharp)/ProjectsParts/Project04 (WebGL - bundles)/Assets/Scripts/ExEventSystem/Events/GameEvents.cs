using System.Collections;
using System.Collections.Generic;
using Cells;
using Network.Input;
using UnityEngine;

namespace ExEvent {

	public class GameEvents {

		public sealed class KeyDown : BaseEvent {
			public KeyCode keyCode;

			public KeyDown(KeyCode keyCode) {
				this.keyCode = keyCode;
			}

			public static void Call(KeyCode keyCode) {
				BaseEvent.Call(new KeyDown(keyCode));
			}

		}
		public sealed class KeyUp : BaseEvent {
			public KeyCode keyCode;

			public KeyUp(KeyCode keyCode) {
				this.keyCode = keyCode;
			}

			public static void Call(KeyCode keyCode) {
				BaseEvent.Call(new KeyUp(keyCode));
			}

		}

		public sealed class MouseScroll : BaseEvent {
			public float deltaScroll;

			public MouseScroll(float deltaScroll) {
				this.deltaScroll = deltaScroll;
			}

			public static void Call(float deltaScroll) {
				BaseEvent.Call(new MouseScroll(deltaScroll));
			}

		}

		public sealed class MapClick : BaseEvent {
			public Vector2 position;

			public MapClick(Vector2 position) {
				this.position = position;
			}

			public static void Call(Vector2 position) {
				BaseEvent.Call(new MapClick(position));
			}
		}
		
		public sealed class PlayerInfoChange : BaseEvent {
			public PlayerBehaviour player;

			public PlayerInfoChange(PlayerBehaviour player) {
				this.player = player;
			}

			public static void Call(PlayerBehaviour player) {
				BaseEvent.Call(new PlayerInfoChange(player));
			}
		}

		public sealed class TutorLiveChange : BaseEvent {

			public TutorLiveChange() {
			}

			public static void Call() {
				BaseEvent.Call(new TutorLiveChange());
			}
		}

		public sealed class PlayerSelect : BaseEvent {
			public PlayerBehaviour player;

			public PlayerSelect(PlayerBehaviour player) {
				this.player = player;
			}

			public static void Call(PlayerBehaviour player) {
				BaseEvent.Call(new PlayerSelect(player));
			}
		}

		public sealed class ChangeAttackTargets : BaseEvent {
			public BodyElement attackLeft = BodyElement.none;
			public BodyElement attackRight = BodyElement.none;
			public BodyElement shieldLeft1 = BodyElement.none;
			public BodyElement shieldLeft2 = BodyElement.none;
			public BodyElement shieldRight1 = BodyElement.none;
			public BodyElement shieldRight2 = BodyElement.none;

			public ChangeAttackTargets(BodyElement attackLeft, BodyElement attackRight, BodyElement shieldLeft1, BodyElement shieldLeft2, BodyElement shieldRight1, BodyElement shieldRight2) {
				this.attackLeft = BodyElement.none;
				this.attackRight = BodyElement.none;
				this.shieldLeft1 = BodyElement.none;
				this.shieldLeft2 = BodyElement.none;
				this.shieldRight1 = BodyElement.none;
				this.shieldRight2 = BodyElement.none;
			}

			public static void Call(BodyElement attackLeft, BodyElement attackRight, BodyElement shieldLeft1, BodyElement shieldLeft2, BodyElement shieldRight1, BodyElement shieldRight2) {
				BaseEvent.Call(new ChangeAttackTargets(attackLeft, attackRight, shieldLeft1, shieldLeft2, shieldRight1, shieldRight2));
			}
		}

		//public sealed class ClickCell : BaseEvent {
		//	public Cell cellClick;

		//	public ClickCell(Cell cellClick) {
		//		this.cellClick = cellClick;
		//	}

		//	public static void Call(Cell cellClick) {
		//		BaseEvent.Call(new GameEvents.ClickCell(cellClick));
		//	}

		//}

	}

}
