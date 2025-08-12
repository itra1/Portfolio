using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

namespace it.UI.Elements
{
	public class TextMeshProLinkHandler : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
	{
		public DataEvent[] Events;

		[SerializeField] private TextMeshProUGUI _tGui;
		[SerializeField] private int _anyClick = -1;

		private TMP_LinkInfo? _hoverLink;
		private bool _isHove;

		[System.Serializable]
		public struct DataEvent
		{
			public string Name;
			public UnityEvent OnClick;
		}

		private void OnEnable()
		{
			if (_tGui == null)
				_tGui = GetComponent<TextMeshProUGUI>();
		}

		public void OnPointerClick(PointerEventData eventData)
		{

			if (_tGui == null) return;

			int linkIndex = TMP_TextUtilities.FindIntersectingLink(_tGui, Input.mousePosition, Camera.allCameras[0]);

			if (linkIndex == -1)
			{
				if (_anyClick != -1)
					ConfirmClick(_anyClick);
				return;
			}
			ConfirmClick(linkIndex);
		}

		private void ConfirmClick(int index)
		{


			TMP_LinkInfo link = _tGui.textInfo.linkInfo[index];

			for (int i = 0; i < Events.Length; i++)
			{
				if (Events[i].Name == link.GetLinkID())
					Events[i].OnClick?.Invoke();
			}
		}

		private void FixedUpdate()
		{
			if (!_isHove) return;
			if (_tGui == null) return;

			int linkIndex = TMP_TextUtilities.FindIntersectingLink(_tGui, Input.mousePosition, Camera.allCameras[0]);

			if (linkIndex == -1)
			{
				UnsetPointer();
				return;
			}
			SetPointer();
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			_isHove = true;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			UnsetPointer();
			_isHove = false;
		}

		private void SetPointer()
		{
			AppManager.SetPointerCursor();
		}

		private void UnsetPointer()
		{
			AppManager.SetDefaultCursor();
		}

	}
}