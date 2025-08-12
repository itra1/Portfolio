using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace it.Game.Panels
{
	//[ExecuteInEditMode]
	public class PrizePanel : MonoBehaviour
	{
		[SerializeField] private RectTransform _bodyRect;
		[SerializeField] private Image _backLight;
		[SerializeField] private RectTransform _lightMaskRect;
		[SerializeField] private Vector2 _bodyPositionsStars;
		[SerializeField] private RectTransform _lighting;
		[SerializeField] private RectTransform _contentScroll;
		[SerializeField] private PrizeItem _poolPrefab;
		[SerializeField] private AnimationCurve _animationCurve;
		[SerializeField] private RectTransform _starsRect;

		private List<decimal> _valueList = new List<decimal>();
		
		private PoolList<PrizeItem> _poolerItems;
		private CanvasGroup _cg;
		private bool _isPlay = false;
		private float _itemSpace = 7;
		private float _scrollSpeed = 1200;

		private float _allHeight;
		private int _currentStep = 0;
		private float _timeStartStop;
		private decimal _targetValue;
		private decimal _bigBlind;


		public void SetValue(List<decimal> spineVariants, decimal targetValue){
			_targetValue = targetValue;
			//_bigBlind = bb;

			_valueList = new List<decimal>(spineVariants);
			//_valueList.Add(_bigBlind * 8);
			//_valueList.Add(_bigBlind * 20);
			//_valueList.Add(_bigBlind * 40);
			//_valueList.Add(_bigBlind * 80);
			//_valueList.Add(_bigBlind * 800);

		}

		public void OnEnable()
		{
			if (_poolerItems == null)
				_poolerItems = new PoolList<PrizeItem>(_poolPrefab.gameObject, _contentScroll);
			_cg = GetComponent<CanvasGroup>();
			_cg.alpha = 1;
			_starsRect.rotation = Quaternion.Euler(0, 0, 0);
			_contentScroll.anchoredPosition = Vector2.zero;
			_currentStep = 0;
			_isPlay = true;
			_backLight.gameObject.SetActive(false);
			SpawnItems();
			GetComponent<Animator>().SetTrigger("visible");
		}

		private void SpawnItems()
		{
			_poolerItems.HideAll();

			RectTransform itemRect = _poolPrefab.GetComponent<RectTransform>();

			_allHeight = -itemRect.rect.height - _itemSpace;

			for (int i = 0; i < 50; i++)
			{
				var itm = _poolerItems.GetItem();
				itm.gameObject.SetActive(true);
				itm.SetValue(_valueList[Random.Range(0, _valueList.Count)]);

				RectTransform itmRect = itm.GetComponent<RectTransform>();
				itmRect.anchoredPosition = new Vector2(0, _allHeight);
				_allHeight += itmRect.rect.height + _itemSpace;

				if(i == 50-2) itm.SetValue(_targetValue);
			}
			_allHeight -= (_poolPrefab.GetComponent<RectTransform>().rect.height + _itemSpace) * 2;
		}

		[ContextMenu("Save")]
		private void SavePositions()
		{
			_bodyPositionsStars = _lighting.anchoredPosition;
		}

		private void Update()
		{
			_lighting.anchoredPosition = _lightMaskRect.InverseTransformPoint(_bodyRect.TransformPoint(new Vector3(_bodyPositionsStars.x + _lightMaskRect.rect.width/2, _bodyPositionsStars.y)));
			if (!_isPlay) return;

			if (_currentStep == 0)
			{
				if (_contentScroll.anchoredPosition.y > -(_allHeight - (1.0f * _scrollSpeed)))
					_contentScroll.anchoredPosition = new Vector3(0, _contentScroll.anchoredPosition.y - _scrollSpeed * Time.deltaTime);
				else
					SetStep(1);
			}

			if (_currentStep == 1)
			{
				float startDistance = _allHeight - (1.0f * _scrollSpeed);

				float delta = Time.timeSinceLevelLoad - _timeStartStop;

				_contentScroll.anchoredPosition = new Vector3(0, -startDistance - _animationCurve.Evaluate(Mathf.Clamp(delta, 0, 2))* (1.0f * _scrollSpeed));

				if (delta > 2)
				{
					_isPlay = false;
					DOTween.To(() => _cg.alpha, (x) => _cg.alpha = x, 0, 0.5f).SetDelay(1.5f).OnComplete(()=> { gameObject.SetActive(false); });
				}
			}

		}

		private void SetStep(int st)
		{
			_currentStep = st;

			if (_currentStep == 1)
			{
				_timeStartStop = Time.timeSinceLevelLoad;

				Color wh = Color.white;
				wh.a = 0;
				_backLight.color = wh;
				_backLight.gameObject.SetActive(true);
				_backLight.DOColor(Color.white, 1f);
			}
		}

		[ContextMenu("Play")]
		public void Play()
		{
			_isPlay = !_isPlay;
		}

	}
}