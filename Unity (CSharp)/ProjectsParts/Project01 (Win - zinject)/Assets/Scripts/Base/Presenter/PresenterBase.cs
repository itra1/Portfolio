using System;
using UnityEngine;
using Utils;
using Debug = Core.Logging.Debug;

namespace Base.Presenter
{
	[DisallowMultipleComponent, RequireComponent(typeof(RectTransform))]
	public abstract class PresenterBase : MonoBehaviour
	{
		protected const string TypeNamePrefix = "Presenter";
		
		[SerializeField] private RectTransform _content;
		
		private RectTransform _rectTransform;
		
		public event EventHandler Shown;
		public event EventHandler Hidden;
		public event EventHandler Unloaded;
		
		public RectTransform Content => _content;
		
		public RectTransform RectTransform => 
			_rectTransform == null ? _rectTransform = GetComponent<RectTransform>() : _rectTransform;
		
		protected RectTransform Parent => RectTransform.parent as RectTransform;
		
		public bool Visible { get; protected set; }
		
		protected void SetName(string value) => gameObject.name = value;
		
		public virtual void SetParentOnInitialize(RectTransform parent)
		{
			if (Parent != parent)
				SetParent(parent);
		}
		
		public virtual void AlignToParent()
		{
			var rectTransform = RectTransform;
			
			rectTransform.ResetAnchors(Vector2.one * 0.5f);
			rectTransform.Reset();
		}
		
		public virtual bool Show()
		{
			if (Visible)
			{
				Debug.LogWarning($"An attempt was detected to show an already visible presenter named \"{gameObject.name}\"");
				return false;
			}
			
			Visible = true;
			gameObject.SetActive(true);
			DispatchShownEvent();
			return true;
		}
		
		public virtual bool Hide()
		{
			try
			{
				if (!Visible)
				{
					Debug.LogWarning($"An attempt was detected to hide an already invisible presenter named \"{gameObject.name}\"");
					return false;
				}
				
				Visible = false;
				gameObject.SetActive(false);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
			finally
			{
				DispatchHiddenEvent();
			}
		}
		
		public virtual void Unload()
		{
			Visible = false;
			
			try
			{
				_rectTransform = null;
				Destroy(gameObject);
			}
			catch (Exception)
			{
				// ignored
			}
			finally
			{
				DispatchUnloadedEvent();
			}
		}
		
		protected void SetParent(RectTransform parent) => RectTransform.SetParent(parent);
		
		protected void DispatchShownEvent() => Shown?.Invoke(this, EventArgs.Empty);
		protected void DispatchHiddenEvent() => Hidden?.Invoke(this, EventArgs.Empty);
		private void DispatchUnloadedEvent() => Unloaded?.Invoke(this, EventArgs.Empty);
	}
}