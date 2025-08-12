using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Providers.Audio.Base;
using Game.Providers.Audio.Handlers;
using StringDrop;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace Game.Providers.Ui.Popups.Base {
	public abstract class Popup : MonoBehaviour, IPopup {

		[SerializeField, StringDropList(typeof(SoundNames))]
		protected string _openAudioClip = SoundNames.UiPopUp;

		protected IPopupSettings _popupSettings;

		private readonly UnityEvent _OnShow = new();
		private readonly UnityEvent _OnShowComplete = new();
		private readonly UnityEvent _OnHide = new();
		private readonly UnityEvent _OnHideComplete = new();

		protected Image _backImage;
		protected RectTransform _dialog;
		protected CanvasGroup _cg;
		protected AudioHandler _audioHandler;

		private Color _defaultBackColor;
		private Color _transparentBackColor;
		private bool _changeVisible = false;

		protected virtual float TimeShow { get; } = 0.3f;
		protected virtual float TimeHide { get; } = 0.3f;

		private bool _isFinded = false;

		[Inject]
		public void InitComponents(AudioHandler audioHandler, IPopupSettings popupSettings) {
			_popupSettings = popupSettings;
			_audioHandler = audioHandler;
		}

		protected virtual void Awake() {
		}

		protected virtual void OnEnable() {
			if (!string.IsNullOrEmpty(_openAudioClip))
				_ = _audioHandler.PlayRandomClip(_openAudioClip);
		}

		private void FingComponents() {
			var backTransform = transform.Find("Back");
			if (backTransform != null && backTransform.TryGetComponent<Image>(out var backImage)) {
				_backImage = backImage;
				_defaultBackColor = _backImage.color;
				_transparentBackColor = _defaultBackColor;
				_transparentBackColor.a = 0;
			}
			var dialogTransform = transform.Find("Dialog");
			if (dialogTransform) {
				_dialog = dialogTransform.GetOrAddComponent<RectTransform>();
				_cg = dialogTransform.GetOrAddComponent<CanvasGroup>();
			}

			_isFinded = true;
		}

		public virtual async UniTask Show() {
			if (_changeVisible)
				return;
			_changeVisible = true;
			gameObject.SetActive(true);

			var isFirst = _isFinded;

			if (!_isFinded)
				FingComponents();
			if (isFirst)
				await UniTask.Yield();
			BeforeShow();

			if (_backImage != null)
				_backImage.color = _transparentBackColor;
			if (_dialog != null)
				_dialog.localScale = Vector2.zero;
			if (_cg != null)
				_cg.alpha = 0;

			if (isFirst) {
				await UniTask.Yield();
				await UniTask.Yield();
			}
			EmitOnShow();

			if (_backImage != null)
				_backImage.DOColor(_defaultBackColor, TimeShow).ToUniTask().Forget();
			if (_dialog != null) {
				_dialog.DOScale(Vector2.one, TimeShow).SetEase(_popupSettings.PopupCurveAnimation).OnUpdate(UpdateShow).OnComplete(() => {
					EmitOnShowComplete();
					AfterShow();
				}).ToUniTask().Forget();
			} else {
				EmitOnShowComplete();
				AfterShow();
			}
			if (_cg != null)
				DOTween.To(() => _cg.alpha, x => _cg.alpha = x, 1, TimeShow).ToUniTask().Forget();
			await UniTask.Delay((int)(TimeShow * 1000));
			_changeVisible = false;
		}

		protected virtual void BeforeShow() { }
		protected virtual void UpdateShow() { }
		protected virtual void AfterShow() { }

		public virtual async UniTask Hide() {
			if (_changeVisible)
				return;
			_changeVisible = true;

			BeforeHide();
			EmitOnHide();

			if (_backImage != null)
				_backImage.DOColor(_transparentBackColor, TimeHide).ToUniTask().Forget();
			if (_dialog != null) {
				_dialog.DOScale(Vector2.zero, TimeHide).OnComplete(() => {
					gameObject.SetActive(false);
					EmitOnHideComplete();
					AfterHide();
				}).ToUniTask().Forget();
			} else {
				gameObject.SetActive(false);
				EmitOnHideComplete();
				AfterHide();
			}
			if (_cg != null)
				DOTween.To(() => _cg.alpha, x => _cg.alpha = x, 0, TimeHide).ToUniTask().Forget();
			await UniTask.Delay((int)(TimeHide * 1000));
			_changeVisible = false;
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
