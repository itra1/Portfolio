using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace it.UI.Elements
{

	public abstract class ButtonUI : Selectable, IPointerClickHandler, ISubmitHandler, IButton, IPointerDownHandler
	{
		public UnityEngine.Events.UnityEvent OnClick;
		public UnityEngine.Events.UnityEvent OnPointerUpEvent;
		public UnityEngine.Events.UnityEvent OnPointerDownEvent;

		[SerializeField] private bool _isPointer = true;
		protected bool _isHover = true;

		public UnityEvent OnClickPointer { get => OnClick; set => OnClick = value; }

		protected override void Awake()
		{
			base.Awake();
			Init();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
		}

		protected virtual void Init()
		{
		}

		public override void OnPointerUp(PointerEventData eventData)
		{
			base.OnPointerUp(eventData);
			OnPointerUpEvent?.Invoke();
		}

		public override void OnPointerDown(PointerEventData eventData)
		{
			base.OnPointerDown(eventData);
			OnPointerDownEvent?.Invoke();
		}

		public virtual void Click()
		{
			if (!interactable) return;
			OnClick?.Invoke();
		}

		public void OnSubmit(BaseEventData eventData)
		{
			Click();
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			Click();
		}

		protected override void DoStateTransition(SelectionState state, bool instant)
		{
			//base.DoStateTransition(state, instant);

			//if(!interactable){
			//	NoInteractiveState();
			//	return;
			//}

			if (this == null || base.gameObject == null) return;

			if (base.gameObject.activeInHierarchy)
			{
				switch (state)
				{
					case SelectionState.Normal:
						if (_isPointer) AppManager.SetDefaultCursor();
						_isHover = false;
						NormalState();
						break;
					case SelectionState.Highlighted:
						if (_isPointer) AppManager.SetPointerCursor();
						_isHover = true;
						HighlightedState();
						break;
					case SelectionState.Pressed:
						if (_isPointer) AppManager.SetPointerCursor();
						_isHover = false;
						DownState();
						break;
					case SelectionState.Selected:
						if (_isPointer) AppManager.SetDefaultCursor();
						_isHover = false;
						SelectState();
						break;
					case SelectionState.Disabled:
						_isHover = false;
						DisableState();
						break;
					default:
						NormalState();
						break;
				}
			}

		}

		public virtual void DownState()
		{

		}

		public virtual void NormalState()
		{

		}

		public virtual void HighlightedState()
		{

		}

		public virtual void SelectState()
		{

		}
		public virtual void DisableState()
		{

		}
		public virtual void NoInteractiveState()
		{

		}

	}
}