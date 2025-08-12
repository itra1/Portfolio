using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace it.Game.Panels
{
	public class JackpotRoll : MonoBehaviour
	{
		[SerializeField] private RectTransform _content;
		[SerializeField] private TextMeshProUGUI _textPrefab;
		[SerializeField] private AnimationCurve _animationCurve;

		private PoolList<TextMeshProUGUI> _poolerItems;
		private bool _isPlay;
		private int _tagetValue;
		private float _allHeight;
		private float _itemSpace = 4;
		private float _scrollSpeed = 1200;
		private int _currentStep = 0;
		private float _timeStartStop;
		private float _timeStop;

		public void SetData(string value)
		{
			_tagetValue = value == "." || value == "," ? 10 : int.Parse(value);
			_currentStep = 0;
			_content.anchoredPosition = Vector2.zero;
			_timeStop = Random.Range(1, 1.5f);

			if (_poolerItems == null)
				_poolerItems = new PoolList<TextMeshProUGUI>(_textPrefab.gameObject, _content);
			_textPrefab.gameObject.SetActive(false);

			SpawnItems();

			_isPlay = true;
		}

		public void SetSymbol()
		{
			_content.anchoredPosition = Vector2.zero;
			_textPrefab.gameObject.SetActive(true);
			_textPrefab.text = StringConstants.CURRENCY_SYMBOL;

			if (_poolerItems != null)
				_poolerItems.HideAll();
		}

		public void SetClear()
		{
			_content.anchoredPosition = Vector2.zero;
			if (_poolerItems != null)
				_poolerItems.HideAll();
			_textPrefab.gameObject.SetActive(false);
		}

		private void SpawnItems()
		{
			_poolerItems.HideAll();
			_allHeight = 0;

			int count = Random.Range(40, 60);

			int val = _tagetValue;
			int max = val + 2;
			if (max >= 11)
				max = max - 11;

			float height = 0;
			bool fix = false;

			for (int i = 0; i < count || val != max || !fix; i++)
			{
				var itm = _poolerItems.GetItem();
				itm.gameObject.SetActive(true);
				itm.text = val == 10 ? "." : val.ToString();

				RectTransform itmRect = itm.GetComponent<RectTransform>();
				itmRect.anchoredPosition = new Vector2(0, _allHeight);
				_allHeight += itmRect.rect.height + _itemSpace;

				if (i >= count && val == _tagetValue)
				{
					height = _allHeight - (itmRect.rect.height + _itemSpace);
					fix = true;
				}

				val++;
				if (val > 10)
					val = 0;
			}
			_allHeight = height;

		}

		private void Update()
		{
			if (!_isPlay) return;

			if (_currentStep == 0)
			{
				if (_content.anchoredPosition.y > -(_allHeight - (_timeStop * _scrollSpeed)))
					_content.anchoredPosition = new Vector3(0, _content.anchoredPosition.y - _scrollSpeed * Time.deltaTime);
				else
					SetStep(1);
			}

			if (_currentStep == 1)
			{
				float startDistance = _allHeight - (_timeStop * _scrollSpeed);

				float delta = Time.timeSinceLevelLoad - _timeStartStop;

				_content.anchoredPosition = new Vector3(0, -startDistance - _animationCurve.Evaluate(Mathf.Clamp(delta, 0, 2)) * (_timeStop * _scrollSpeed));

				if (delta > 2)
				{
					_isPlay = false;
					_content.anchoredPosition = new Vector3(0, -_allHeight);
				}
			}

		}

		private void SetStep(int st)
		{
			_currentStep = st;

			if (_currentStep == 1)
			{
				_timeStartStop = Time.timeSinceLevelLoad;
			}
		}

	}
}