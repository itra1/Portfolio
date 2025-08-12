using UnityEngine;
using UnityEngine.Events;

namespace UGui.Screens.Base
{
	public class Screen: MonoBehaviour, IScreen
	{
		private readonly UnityEvent _OnHide = new();

		public virtual void Show()
		{
			gameObject.SetActive(true);
		}

		public virtual void Hide()
		{
			EmitOnHide();
			gameObject.SetActive(false);
		}

		protected void EmitOnHide()
		{
			_OnHide?.Invoke();
			_OnHide?.RemoveAllListeners();
		}

		public IScreen OnHide(UnityAction callback)
		{
			_OnHide.AddListener(callback);
			return this;
		}
	}
}
