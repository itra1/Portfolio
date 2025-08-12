using UnityEngine;

namespace ExEvent {

	public class GameEvents {

		// Нажатие пойнтера
		public sealed class OnPointerDown : BaseEvent {
			public Vector3 pointPosition;

			public OnPointerDown(Vector3 pointPosition) {
				this.pointPosition = pointPosition;
			}

			public static void Call(Vector3 pointPosition) {
				BaseEvent.Call(new OnPointerDown(pointPosition));
			}

		}

		// Отпускание пойнтера
		public sealed class OnPointerUp : BaseEvent {
			public Vector3 pointPosition;

			public OnPointerUp(Vector3 pointPosition) {
				this.pointPosition = pointPosition;
			}

			public static void Call(Vector3 pointPosition) {
				BaseEvent.Call(new OnPointerUp(pointPosition));
			}

		}

		// Смещение пойнтера
		public sealed class OnPointerDrag : BaseEvent {
			public Vector3 pointPosition;

			public OnPointerDrag(Vector3 pointPosition) {
				this.pointPosition = pointPosition;
			}

			public static void Call(Vector3 pointPosition) {
				BaseEvent.Call(new OnPointerDrag(pointPosition));
			}

		}

		// Событие наведение на букву
		public sealed class OnAlphaEnter : BaseEvent {
			public Vector3 pointPosition;
			public AlphaFloatBehaviour alphaBehaviour;

			public OnAlphaEnter(Vector3 pointPosition, AlphaFloatBehaviour alphaBehaviour) {
				this.pointPosition = pointPosition;
				this.alphaBehaviour = alphaBehaviour;
			}

			public static void Call(Vector3 pointPosition, AlphaFloatBehaviour alphaBehaviour) {
				BaseEvent.Call(new OnAlphaEnter(pointPosition, alphaBehaviour));
			}

		}

		// Событие нажатие на букву
		public sealed class OnAlphaDown : BaseEvent {
			public Vector3 pointPosition;
			public AlphaFloatBehaviour alphaBehaviour;

			public OnAlphaDown(Vector3 pointPosition, AlphaFloatBehaviour alphaBehaviour) {
				this.pointPosition = pointPosition;
				this.alphaBehaviour = alphaBehaviour;
			}

			public static void Call(Vector3 pointPosition, AlphaFloatBehaviour alphaBehaviour) {
				BaseEvent.Call(new OnAlphaDown(pointPosition, alphaBehaviour));
			}

		}

		// Событие изменения слова
		public sealed class OnChangeWord : BaseEvent {
			public string word;

			public OnChangeWord(string word) {
				this.word = word;
			}

			public static void Call(string word) {
				BaseEvent.Call(new OnChangeWord(word));
			}
		}

		// Событие изменения слова
		public sealed class OnWordSelect : BaseEvent {
			public string word;
			public SelectWord select;

			public OnWordSelect(string word, SelectWord select) {
				this.word = word;
				this.select = select;
			}

			public static void Call(string word, SelectWord select) {
				BaseEvent.Call(new OnWordSelect(word, select));
			}

		}

		// Событие изменения количества сердец
		public sealed class OnChangeHeart : BaseEvent {
			public int heart;

			public OnChangeHeart(int heart) {
				this.heart = heart;
			}

			public static void Call(int heart) {
				BaseEvent.Call(new OnChangeHeart(heart));
			}

		}

		public sealed class OnChangeGamePhase : BaseEvent {
			public GamePhase last;
			public GamePhase next;

			public OnChangeGamePhase(GamePhase last, GamePhase next) {
				this.last = last;
				this.next = next;
			}

			public static void Call(GamePhase last, GamePhase next) {
				BaseEvent.Call(new OnChangeGamePhase(last, next));
			}
		}

		/// <summary>
		/// Окончание уровня
		/// </summary>
		public sealed class OnBattleChangePhase : BaseEvent {
			public BattlePhase phase;

			public OnBattleChangePhase(BattlePhase phase) {
				this.phase = phase;
			}

			public static void Call(BattlePhase phase) {
				BaseEvent.Call(new OnBattleChangePhase(phase));
			}
		}

		public sealed class OnHintFirstLetterReady : BaseEvent {
			public OnHintFirstLetterReady() { }

			public static void Call() {
				BaseEvent.Call(new OnHintFirstLetterReady());
			}
		}

		public sealed class OnHintFirstLetterCompleted : BaseEvent {
			public bool isCompleted;
			public string word;
			public int? numLetter;

			public OnHintFirstLetterCompleted(bool isCompleted, string word = null, int? numLetter = null) {
				this.isCompleted = isCompleted;
				this.word = word;
				this.numLetter = numLetter;
			}

			public static void Call(bool isCompleted, string word = null, int? numLetter = null) {
				BaseEvent.Call(new OnHintFirstLetterCompleted(isCompleted, word, numLetter));
			}
		}

		public sealed class OnHintAnyLetterReady : BaseEvent {
			public OnHintAnyLetterReady() { }

			public static void Call() {
				BaseEvent.Call(new OnHintAnyLetterReady());
			}
		}

		public sealed class OnHintAnyLetterCompleted : BaseEvent {
			public bool isCompleted;
			public string word;
			public int? numLetter;
			public OnHintAnyLetterCompleted(bool isCompleted, string word = null, int? numLetter = null) {
				this.isCompleted = isCompleted;
				this.word = word;
				this.numLetter = numLetter;
			}

			public static void Call(bool isCompleted, string word = null, int? numLetter = null) {
				BaseEvent.Call(new OnHintAnyLetterCompleted(isCompleted, word, numLetter));
			}
		}

		public sealed class OnHintFirstWordReady : BaseEvent {
			public OnHintFirstWordReady() { }

			public static void Call() {
				BaseEvent.Call(new OnHintFirstWordReady());
			}
		}

		public sealed class OnHintFirstWordCompleted : BaseEvent {
			public bool isCompleted;
			public string word;

			public OnHintFirstWordCompleted(bool isCompleted, string word = null) {
				this.isCompleted = isCompleted;
				this.word = word;
			}

			public static void Call(bool isCompleted, string word = null) {
				BaseEvent.Call(new OnHintFirstWordCompleted(isCompleted, word));
			}
		}

		// Показать перевод слова
		public sealed class ShowWordTranslate : BaseEvent {
			public GameCompany.Word word;

			public ShowWordTranslate(GameCompany.Word word) {
				this.word = word;
			}

			public static void Call(GameCompany.Word word) {
				BaseEvent.Call(new ShowWordTranslate(word));
			}
		}

		public sealed class SwipeLocation : BaseEvent {
			public float alpha;

			public SwipeLocation(float alpha) {
				this.alpha = alpha;
			}

			public static void Call(float alpha) {
				BaseEvent.Call(new SwipeLocation(alpha));
			}
		}

		public sealed class SwipeComplete : BaseEvent {

			public SwipeComplete() {
			}

			public static void Call() {
				BaseEvent.Call(new SwipeComplete());
			}
		}

		// Событие изменения слова
		public sealed class OnWordSave : BaseEvent {

			public OnWordSave() {
			}

			public static void Call() {
				BaseEvent.Call(new OnWordSave());
			}

		}

		public sealed class OnLetterLoaded : BaseEvent {

			public OnLetterLoaded() { }

			public static void Call() {
				BaseEvent.Call(new OnLetterLoaded());
			}

		}

		public sealed class OnChangeMusic : BaseEvent {
			public bool isActive;

			public OnChangeMusic(bool isActive) {
				this.isActive = isActive;
			}

			public static void Call(bool isActive) {
				BaseEvent.Call(new OnChangeMusic(isActive));
			}

		}

		public sealed class OnChangeEffects : BaseEvent {
			public bool isActive;

			public OnChangeEffects(bool isActive) {
				this.isActive = isActive;
			}

			public static void Call(bool isActive) {
				BaseEvent.Call(new OnChangeEffects(isActive));
			}

		}

		public sealed class OnFishTouch : BaseEvent {
			public bool isActive;

			public OnFishTouch() {
			}

			public static void Call() {
				BaseEvent.Call(new OnFishTouch());
			}

		}

		public sealed class NetworkChange : BaseEvent {
			public bool isActive;

			public NetworkChange(bool isActive) {
				this.isActive = isActive;
			}

			public static void Call(bool isActive) {
				BaseEvent.Call(new NetworkChange(isActive));
			}

		}

		public sealed class OnBillingInit : BaseEvent {
			public bool isActive;

			public OnBillingInit(bool isActive) {
				this.isActive = isActive;
			}

			public static void Call(bool isActive) {
				BaseEvent.Call(new OnBillingInit(isActive));
			}

		}


		public sealed class OnDailyBonusResume : BaseEvent {

			public OnDailyBonusResume() {
			}

			public static void Call() {
				BaseEvent.Call(new OnDailyBonusResume());
			}

		}

	}

}
