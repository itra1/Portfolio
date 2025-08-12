using DG.Tweening;
using Game.Game.SpecialPoints;
using StringDrop;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Custom
{
	public class SpecialPointUi : MonoBehaviour
	{
		[SerializeField, StringDropList(typeof(SpecialPointNames))] private string _name;
		[SerializeField] private RectMask2D _mask;
		[SerializeField] private RectTransform _active;
		[SerializeField] private RectTransform _backBlue;
		[SerializeField] private RectTransform _backGray;
		[SerializeField] private RectTransform _rectTransform;

		private Rect _rect;
		private Vector4 _padding;
		private float _lastValue;

		public string Name { get => _name; set => _name = value; }

		public void SetFill(float value)
		{
			if (value == 1 && _lastValue != value)
			{
				_ = DOTween.Sequence()
				.Append(_rectTransform.DOScale(Vector3.one * 1.8f, 0.15f))
				.Append(_rectTransform.DOScale(Vector3.one, 0.15f));
			}
		}
		public void SetFillValue(float value)
		{
			_rect = _rectTransform.rect;
			Vector4 padding = _mask.padding;
			padding.z = _rect.width * (1 - value);
			_mask.padding = padding;

			SetFill(value);
			_lastValue = value;
		}

		public void SetBlueBack()
		{
			_backBlue.gameObject.SetActive(true);
			_backGray.gameObject.SetActive(false);
		}

		public void SetGrayBack()
		{
			_backBlue.gameObject.SetActive(false);
			_backGray.gameObject.SetActive(true);
		}

		public void SetActive()
		{
			_mask.padding = Vector4.one;
		}
	}
}
