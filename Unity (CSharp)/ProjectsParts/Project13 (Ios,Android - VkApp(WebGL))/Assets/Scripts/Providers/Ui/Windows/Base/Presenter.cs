using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Game.Providers.Ui.Windows.Base {
	public abstract class Presenter :MonoBehaviour, IWindow {
		private readonly UnityEvent _OnHide = new();

		protected SignalBus _signalBus;

		private string _screenType;

		[Inject]
		public void ScreenInit(SignalBus signalBus) {
			_signalBus = signalBus;
		}

		protected virtual void Awake() { }

		public async UniTask Show() {
			gameObject.SetActive(true);
			await Open();
			await UniTask.Yield();
		}

		public async UniTask Hide() {
			EmitOnHide();
			await Close();
			gameObject.SetActive(false);
		}

		public virtual async UniTask Open() {
			await UniTask.Yield();
		}

		public virtual async UniTask Close() {
			await UniTask.Yield();
		}

		public void SetScreenType(string screenType) {
			_screenType = screenType;
		}

		protected void EmitOnHide() {
			_OnHide?.Invoke();
			_OnHide?.RemoveAllListeners();
		}

		public IWindow OnHide(UnityAction callback) {
			_OnHide.AddListener(callback);
			return this;
		}
	}
}
