using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using com.ootii.Geometry;
using TMPro;

namespace it.Popups
{
	public abstract class PopupBase : MonoBehaviour
	{
		public event UnityEngine.Events.UnityAction<PopupBase> OnClose;

		protected bool IsFocus { get => _isFocus; set => _isFocus = value; }
		public PopupType Type { get => _type; set => _type = value; }
		public bool DarkBlack { get => _darkBlack; set => _darkBlack = value; }
		public int Priority => _priority;

		[SerializeField] private PopupType _type;
		[SerializeField] private int _priority;
		[SerializeField] protected GameObject _popupLocked;
		[SerializeField] private bool _darkBlack = true;
		[SerializeField] private bool _moveToCursor = false;

		protected bool _isFocus;
		protected bool _isLock;
		protected CanvasGroup _canvasGroup;
		protected Transform _dialog;
		private Tween _closeScaleAnimation;
		private Tween _closeHideAnimation;

		protected virtual void Awake()
		{

			var inputs = GetComponentsInChildren<TMP_InputField>(true);

			for (int i = 0; i < inputs.Length; i++)
			{
				var elem = inputs[i];

				//var comp = elem.gameObject.GetOrAddComponent<NeoludicGames.PopInput.WebGLTMPInputFieldHelper>();
				//comp.helperType = NeoludicGames.PopInput.BaseWebGLInputFieldHelper.WebGlInputHelperType.TouchDevicesOnly;

#if UNITY_WEBGL

				//elem.gameObject.GetOrAddComponent<WebGLSupport.WebGLInput>();
				elem.gameObject.GetOrAddComponent<InputVisibleKeyboard>();

				#endif

				//elem.onSelect.AddListener(_ =>
				//{
				//	TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, true, true);
				//});
				//elem.onEndEdit.AddListener(_ =>
				//{
				//	TouchScreenKeyboard.("", TouchScreenKeyboardType.Default, false, false, true, true);
				//});

			}

		}

		private void OnDrawGizmosSelected()
		{
			Rename();
		}
		[ContextMenu("Rename")]
		public void Rename()
		{
			string targetName = Type.ToString() + "Popup";
			if (gameObject.name != targetName)
				gameObject.name = targetName;
		}

		protected virtual void OnEnable()
		{
			var btn = gameObject.GetComponentInChildren<CloseButton>(true);

			IButton ibtnClose = null;
			if (btn != null)
				ibtnClose = btn.GetComponent<IButton>();
			else
			{
				var backButton = gameObject.GetComponentInChildren<BackButton>(true);
				if (backButton != null)
					ibtnClose = backButton.GetComponent<IButton>();
			}



			if (ibtnClose != null)
			{
				ibtnClose.OnClickPointer.RemoveAllListeners();
				ibtnClose.OnClickPointer.AddListener(() =>
				{
					Hide();
				});
			}

			if (btn == null)
			{

			}

			//if(_moveToCursor){
			//	if (_dialog.GetComponent<MoveRectToPointer>() == null)
			//		_dialog.gameObject.AddComponent<MoveRectToPointer>();
			//}


			EnableInit();
			Localize();

		}

		protected virtual void OnDisable()
		{

		}

		protected virtual void EnableInit()
		{
			Lock(false);
		}

		protected virtual void Localize()
		{

		}

		public virtual void Show(bool force = false)
		{
			if (!force && gameObject.activeInHierarchy) return;
			if (_dialog == null)
				_dialog = transform.Find("Dialog");
			if (_dialog == null) _dialog = transform.Find("DialogLogin");
			if (_dialog != null && _canvasGroup == null) _canvasGroup = _dialog.gameObject.GetOrAddComponent<CanvasGroup>();

			if (_closeScaleAnimation != null)
				_closeScaleAnimation.Kill();
			if (_closeHideAnimation != null)
				_closeHideAnimation.Kill();

			_dialog.localScale = Vector3.one * 0.7f;
			_canvasGroup.alpha = 0;

			gameObject.SetActive(true);

			_dialog.DOScale(Vector3.one, 0.3f);
			DOTween.To(() => _canvasGroup.alpha, (x) => _canvasGroup.alpha = x, 1, 0.3f);
		}
		public virtual void Hide()
		{
			Hide(null);
		}
		public virtual void Hide(UnityEngine.Events.UnityAction OnComplete)
		{
			if (gameObject == null || !gameObject.activeInHierarchy) return;
			if (_canvasGroup == null) _canvasGroup = GetComponentInChildren<CanvasGroup>();
			if (_dialog == null) _dialog = transform.Find("Dialog");
			if (_dialog == null) _dialog = transform.Find("DialogLogin");

			if (_closeHideAnimation != null && _closeHideAnimation.active) return;

			_closeScaleAnimation = _dialog.DOScale(Vector3.one * 0.7f, 0.3f);
			if (_canvasGroup != null)
				_closeHideAnimation = DOTween.To(() => _canvasGroup.alpha, (x) => _canvasGroup.alpha = x, 0, 0.3f)
				.OnComplete(() =>
				{
					OnComplete?.Invoke();
					gameObject.SetActive(false);
					OnClose?.Invoke(this);
				});
		}
		public void Focus()
		{
			if (!gameObject.activeInHierarchy) return;

			//it.Main.PopupController.Instance.BackBlack.transform.SetAsLastSibling();
			transform.SetAsLastSibling();
		}
		private void Update()
		{
			if (Input.GetKeyUp(KeyCode.Tab))
			{
				if (EventSystem.current.currentSelectedGameObject != null)
				{
					Selectable s = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
					if (s != null)
					{
						var elem = s.FindSelectableOnDown();
						if (elem != null) elem.Select();
					}
				}
			}

		}

		public virtual void Lock(bool isLock)
		{
			if (_popupLocked == null) return;

			_isLock = isLock;
			_popupLocked.SetActive(_isLock);
		}
	}
}