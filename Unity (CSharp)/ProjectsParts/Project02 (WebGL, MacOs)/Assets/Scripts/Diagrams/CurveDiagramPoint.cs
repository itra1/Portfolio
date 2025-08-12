using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace it.Diagrams
{
	public class CurveDiagramPoint : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
	{
		public UnityEngine.Events.UnityAction OnClick;
		public static CurveDiagramPoint _selectItem;

		[SerializeField] private RectTransform _selectImage;
		[SerializeField] private RectTransform _line;
		[SerializeField] private RectTransform _valueRect;
		[SerializeField] private it.Inputs.CurrencyLabel _valueLabel;

		private bool _isSelect;
		private Vector3 _position;
		private float _value;

		public bool IsSelect
		{
			get => _isSelect; set
			{

				_isSelect = value;

				_selectImage.gameObject.SetActive(_isSelect);
				_line.gameObject.SetActive(_isSelect);
				_valueRect.gameObject.SetActive(_isSelect);

				if (_isSelect)
				{
					var tm = _valueRect.GetComponentInChildren<TextMeshProUGUI>();
					_valueRect.sizeDelta = new Vector2(tm.preferredWidth+25, _valueRect.sizeDelta.y);
				}

			}
		}

		public void SelectSet()
		{
			IsSelect = true;
			_selectItem = this;
		}
		public void SelectUnset()
		{
			IsSelect = false;
			if (_selectItem == this)
				_selectItem = null;
		}

		public void SetData(Vector2 position, float value)
		{
			_valueLabel.SetValue("{0}", value);
			_position = position;
			_value = value;
			_line.sizeDelta = new Vector2(_line.sizeDelta.x, position.y);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			OnClick?.Invoke();

			if (_selectItem == this) return;

			if (_selectItem != null)
			{
				_selectItem.SelectUnset();
			}
			SelectSet();

		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			AppManager.SetPointerCursor();
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			AppManager.SetDefaultCursor();
		}
	}
}