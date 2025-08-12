using Core.Engine.uGUI.Screens.Signals;
using UnityEngine;
using UnityEngine.Events;
using Zenject;
#if UNITY_EDITOR

using UnityEditor;
#endif

namespace Core.Engine.uGUI.Screens {
#if UNITY_EDITOR
	[CustomEditor(typeof(Screen), true)]
	public class ScreenEditor : Editor {
		private Screen t;

		private void OnEnable() {
			t = (Screen)target;
		}

		public override void OnInspectorGUI() {
			if (t.RemaneGameObjectByType()) {
				EditorUtility.SetDirty(t);
			}
			base.OnInspectorGUI();
		}
	}

#endif

	public abstract class Screen : MonoBehaviour, IScreen {
		private readonly UnityEvent _OnHide = new();

		protected SignalBus _signalBus;

		private string _screenType;

		[Inject]
		public void ScreenInit(SignalBus signalBus) {
			_signalBus = signalBus;
		}

		public virtual void Show() {
			gameObject.SetActive(true);
			_signalBus.Fire(new ScreenVisibleChangeSignal() { ScreenType = _screenType, IsVisible = true });
		}

		public virtual void Hide() {
			EmitOnHide();
			gameObject.SetActive(false);
			_signalBus.Fire(new ScreenVisibleChangeSignal() { ScreenType = _screenType, IsVisible = false });
		}
		public void SetScreenType(string screenType) {
			_screenType = screenType;
		}

		protected void EmitOnHide() {
			_OnHide?.Invoke();
			_OnHide?.RemoveAllListeners();
		}

		public IScreen OnHide(UnityAction callback) {
			_OnHide.AddListener(callback);
			return this;
		}
	}
}