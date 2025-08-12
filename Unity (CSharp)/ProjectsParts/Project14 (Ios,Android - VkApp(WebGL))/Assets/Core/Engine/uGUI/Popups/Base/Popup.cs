using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using Zenject;
using Cysharp.Threading.Tasks;
using Core.Engine.Settings;
using SoundPoint;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Core.Engine.uGUI.Popups {

#if UNITY_EDITOR

	[CustomEditor(typeof(Popup), true)]
	public class PopupEditor : Editor {
		private Popup t;

		private void OnEnable() {
			t = (Popup)target;
		}

		public override void OnInspectorGUI() {
			if (t.RemaneGameObjectByType()) {
				EditorUtility.SetDirty(t);
			}
			base.OnInspectorGUI();
		}
	}

#endif

	public abstract class Popup : MonoBehaviour, IPopup {
		protected IPopupSettings _popupSettings;
		private readonly UnityEvent _OnShow = new();
		private readonly UnityEvent _OnShowComplete = new();
		private readonly UnityEvent _OnHide = new();
		private readonly UnityEvent _OnHideComplete = new();

		protected MaskableGraphic _backImage;
		protected RectTransform _dialog;
		protected CanvasGroup _cg;
		protected SoundLibrary _soundLibrary;
		protected IAudioPointFactory _audioFactory;
		private bool _isFinded = false;
		private Color _defaultBackColor;
		private Color _transparentBackColor;

		protected virtual float TimeShow { get; } = 0.3f;
		protected virtual float TimeHide { get; } = 0.3f;

		[Inject]
		public void InitComponents(IPopupSettings popupSettings, SoundLibrary soundLibrary, IAudioPointFactory audioFacory) {
			_popupSettings = popupSettings;
			_soundLibrary = soundLibrary;
			_audioFactory = audioFacory;
		}

		private void FingComponents() {

			var backTransform = transform.Find("Back");
			if (backTransform != null && backTransform.TryGetComponent<MaskableGraphic>(out var backImage)) {
				_backImage = backImage;
				_defaultBackColor = _backImage.color;
				_transparentBackColor = _defaultBackColor;
				_transparentBackColor.a = 0;
			}

			var dialogTransform = transform.Find("Dialog");
			if (dialogTransform) {
				if (dialogTransform.TryGetComponent<RectTransform>(out var dialogRect))
					_dialog = dialogRect;
				if (dialogTransform.TryGetComponent<CanvasGroup>(out var dialogCg))
					_cg = dialogCg;
			}

			_isFinded = true;
		}

		public async UniTask Show() {
			if (!_isFinded)
				FingComponents();

			BeforeShow();

			if (_backImage != null)
				_backImage.color = _transparentBackColor;
			if (_dialog != null)
				_dialog.localScale = Vector2.zero;
			if (_cg != null)
				_cg.alpha = 0;

			gameObject.SetActive(true);
			EmitOnShow();

			if (_backImage != null)
				_ = _backImage.DOColor(_defaultBackColor, TimeShow);

			if (_dialog != null)
				_ = _dialog.DOScale(Vector2.one, TimeShow).SetEase(_popupSettings.PopupCurveAnimation);

			if (_cg != null)
				_ = DOTween.To(() => _cg.alpha, x => _cg.alpha = x, 1, TimeShow);

			await UniTask.Delay((int)(TimeShow * 1000));

			EmitOnShowComplete();
			AfterShow();
		}

		protected virtual void BeforeShow() { }
		protected virtual void AfterShow() { }

		public async UniTask Hide() {

			BeforeHide();
			EmitOnHide();

			if (_backImage != null)
				_ = _backImage.DOColor(_transparentBackColor, TimeHide);

			if (_dialog != null)
				_ = _dialog.DOScale(Vector2.zero, TimeHide);

			if (_cg != null)
				_ = DOTween.To(() => _cg.alpha, x => _cg.alpha = x, 0, TimeShow);

			await UniTask.Delay((int)(TimeHide * 1000));

			gameObject.SetActive(false);
			EmitOnHideComplete();
			AfterHide();
		}
		protected virtual void BeforeHide() { }
		protected virtual void AfterHide() { }

		protected void EmitOnShow() {
			_OnShow?.Invoke();
			_OnShow?.RemoveAllListeners();
		}

		protected void EmitOnShowComplete() {
			_OnShowComplete?.Invoke();
			_OnShowComplete?.RemoveAllListeners();
		}

		protected void EmitOnHide() {
			_OnHide?.Invoke();
			_OnHide?.RemoveAllListeners();
		}

		protected void EmitOnHideComplete() {
			_OnHideComplete?.Invoke();
			_OnHideComplete?.RemoveAllListeners();
		}

		public IPopup OnShow(UnityAction callback) {
			_OnShow.AddListener(callback);
			return this;
		}

		public IPopup OnShowComplete(UnityAction callback) {
			_OnShowComplete.AddListener(callback);
			return this;
		}

		public IPopup OnHide(UnityAction callback) {
			_OnHide.AddListener(callback);
			return this;
		}

		public IPopup OnHideComplete(UnityAction callback) {
			_OnHideComplete.AddListener(callback);
			return this;
		}
	}
}
