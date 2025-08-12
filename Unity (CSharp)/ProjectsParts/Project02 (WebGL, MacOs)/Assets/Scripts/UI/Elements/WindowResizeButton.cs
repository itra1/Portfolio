using UnityEngine;
using UnityEngine.EventSystems;

using Sett = it.Settings;

namespace it.UI.Elements
{
	public class WindowResizeButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
	{
		private bool _isHover;
		private bool _isDown;
		public void OnPointerDown(PointerEventData eventData)
		{
#if UNITY_STANDALONE
			StandaloneController.Instance.WindowResizeStart();
#endif
			_isDown = true;
			Confirm();
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			_isHover = true;
			Confirm();
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			_isHover = false;
			Confirm();
		}

		public void OnPointerUp(PointerEventData eventData)
		{
#if UNITY_STANDALONE
			StandaloneController.Instance.WindowResizeEnd();
#endif
			_isDown = false;
			Confirm();
		}

		private void Confirm()
		{
			if (_isHover || _isDown)
			{
				Cursor.SetCursor(Sett.AppSettings.ResizeCursor, new Vector2(7, 7), CursorMode.Auto);
			}
			else
			{
				Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
			}
		}
	}
}