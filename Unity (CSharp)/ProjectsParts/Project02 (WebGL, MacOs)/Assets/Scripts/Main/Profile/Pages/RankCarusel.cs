using System;
using System.Collections;
using System.Collections.Generic;
using com.ootii.Utilities.Debug;
using UnityEngine;
using DG.Tweening;
using it.Network.Rest;
using Garilla;
using UnityEngine.Events;

namespace it.Main.SinglePages
{

	public class RankCarusel : MonoBehaviour, ISwipe
	{
		//[SerializeField] private DragRankPanel _dragPanel;
		[SerializeField] private RankCard _recordPrefab;
		[SerializeField] private RectTransform _parent;
		[SerializeField] private RectTransform _centerRect;
		[SerializeField] private RectTransform _leftRect;
		[SerializeField] private RectTransform _rightRect;
		[SerializeField] private RectTransform _leftRect2;
		[SerializeField] private RectTransform _rightRect2;
		[SerializeField] private Garilla.SwipeToClose _swipeToClose = Garilla.SwipeToClose.None;
		//[SerializeField] private Rank _rank;

		private List<RankCard> _cards = new List<RankCard>();
		private PoolList<RankCard> _poolerCards;
		private int _index = 0;
		private float _timeChnage = 5;
		private Coroutine _timerCoroutine;
		//private bool _animate;
		private bool _customMuve;

		public bool CustomMuve { get => _customMuve; set => _customMuve = value; }

		public SwipeToClose SwipeType => _swipeToClose;

		public UnityAction<SwipeToClose> OnSwipeEvent => (swipe) =>
		{
			if ((SwipeToClose.Left & swipe) != 0)
			{
				RightButtonTouch();
			}
			if ((SwipeToClose.Right & swipe) != 0)
			{
				LeftButtonTouch();
			}
		};

		private void OnEnable()
		{
			//_dragPanel.RectTransform.anchoredPosition = Vector2.zero;
			//_dragPanel.OnPointerDragAction = () => { DragPanel(); };
			SpawnItems();
			if (_timerCoroutine != null)
				StopCoroutine(_timerCoroutine);
			//_animate = true;
			ConfirmPosition();
			CustomMuve = true;

			if (_swipeToClose != SwipeToClose.None)
				SwipeListenerAdd();

			//PlayWaitChange();
		}

		private void OnDisable()
		{
			SwipeListenerRemove();
		}

		public void SwipeListenerAdd()
		{
			SwipeManager.AddListener(this);
		}
		public void SwipeListenerRemove()
		{
			SwipeManager.RemoveListener(this);
		}

		private void Start()
		{
			RightButtonTouch();
			RightButtonTouch();
		}

		//public void PlayAnimation()
		//{
		//	_animate = true;
		//	PlayWaitChange();
		//}

		//public void StopAnimations()
		//{
		//	_animate = false;
		//	if (_timerCoroutine != null)
		//		StopCoroutine(_timerCoroutine);
		//}
		public void SelectCard(int cardId)
		{
			UpdateRank(cardId);
		}

		private void UpdateRank(int number)
		{
			var newIndex = number;
			var currentIndex = _index;
			var dif = newIndex - currentIndex;

			if (dif > 0)
			{
				for (int i = 0; i < Math.Abs(dif - 1); i++)
				{
					MoveRight();
				}
			}
			else
			{
				for (int i = 0; i < Math.Abs(dif - 1); i++)
				{
					MoveLeft();
				}
			}



		}

		//public void DragPanel(){
		//	float pos = _dragPanel.RectTransform.anchoredPosition.x % 65f;
		//	int index = (int)Mathf.Floor(_dragPanel.RectTransform.anchoredPosition.x / 65f);
		//	int startIndex = Mathf.RoundToInt(index % (float)_cards.Count);
		//	for(int i = startIndex-4; i < startIndex+ 5; i++){
		//		int targetIndex = i < 0 ? _cards.Count + i : i;
		//		it.Logger.Log(startIndex + " : " +  targetIndex + " : " + i);
		//		RectTransform iRect = _cards[targetIndex].GetComponent<RectTransform>();
		//		int delta = i - startIndex;
		//		if (i <= startIndex){
		//			iRect.anchoredPosition = new Vector2(0-(65-pos) -(65* delta), iRect.anchoredPosition.y);
		//		}
		//		else
		//		{
		//			iRect.anchoredPosition = new Vector2(0 + pos +(65 * delta), iRect.anchoredPosition.y);
		//		}
		//	}
		//}

		private void SpawnItems()
		{

			if (_poolerCards == null)
				_poolerCards = new PoolList<RankCard>(_recordPrefab, _parent);

			_poolerCards.HideAll();
			_cards.Clear();


			var r = UserController.ReferenceData.ranks;
			it.Logger.Log(r);

			for (int i = 0; i < r.Count; i++)
			{
				var itm = _poolerCards.GetItem();
				itm.SetData(0, r[i]);
				_cards.Add(itm);
			}

		}

		private void ConfirmPosition()
		{

			Color wc = Color.black;
			Color wc1 = wc;
			wc1.a = 0;
			Color wc2 = wc;
			wc2.a = 0.2f;
			Color wc3 = wc;
			wc3.a = 0.5f;

			float dist = (_centerRect.localPosition - _leftRect.localPosition).sqrMagnitude;
			var img0 = _cards[_index];
			//var canv0 = img0.GetComponent<Canvas>();
			var rt0 = img0.GetComponent<RectTransform>();
			var tw0 = rt0.DOLocalMove(_centerRect.localPosition, 0.3f);
			tw0.OnUpdate(() =>
			{
				if (tw0.position > (tw0.fullPosition / 2))
					img0.transform.SetSiblingIndex(img0.transform.parent.childCount - 1);
			});
			rt0.DOScale(_centerRect.localScale, 0.3f);
			//DOTween.To(() => canv0.sortingOrder, (x) => canv0.sortingOrder = x, 2, 0.3f);
			img0.FillImage.DOColor(wc1, 0.3f);

			var img1 = _cards[IncrementValue()];
			//var canv1 = img1.GetComponent<Canvas>();
			var rt1 = img1.GetComponent<RectTransform>();
			var tw1 = rt1.DOLocalMove(_rightRect.localPosition, 0.3f);
			tw1.OnUpdate(() =>
			{
				if (tw1.position > (tw1.fullPosition / 2))
					img1.transform.SetSiblingIndex(img1.transform.parent.childCount - 2);
			});
			rt1.DOScale(_rightRect.localScale, 0.3f);
			//DOTween.To(() => canv1.sortingOrder, (x) => canv1.sortingOrder = x, 1, 0.3f);
			img1.FillImage.DOColor(wc2, 0.3f);

			var img2 = _cards[DecrementValue()];
			//var canv2 = img2.GetComponent<Canvas>();
			var rt2 = img2.GetComponent<RectTransform>();
			var tw2 = rt2.DOLocalMove(_leftRect.localPosition, 0.3f);
			tw2.OnUpdate(() =>
			{
				if (tw2.position > (tw2.fullPosition / 2))
					img2.transform.SetSiblingIndex(img2.transform.parent.childCount - 3);
			});
			rt2.DOScale(_leftRect.localScale, 0.3f);
			//DOTween.To(() => canv2.sortingOrder, (x) => canv2.sortingOrder = x, 1, 0.3f);
			img2.FillImage.DOColor(wc2, 0.3f);

			var img11 = _cards[IncrementValue(2)];
			//var canv1 = img1.GetComponent<Canvas>();
			var rt11 = img11.GetComponent<RectTransform>();
			var tw11 = rt11.DOLocalMove(_rightRect2.localPosition, 0.3f);
			tw11.OnUpdate(() =>
			{
				if (tw11.position > (tw11.fullPosition / 2))
					img11.transform.SetSiblingIndex(img11.transform.parent.childCount - 4);
			});
			rt11.DOScale(_rightRect2.localScale, 0.3f);
			//DOTween.To(() => canv1.sortingOrder, (x) => canv1.sortingOrder = x, 1, 0.3f);
			img11.FillImage.DOColor(wc3, 0.3f);
			DOTween.To(() => img11.CanvasGroup.alpha, (x) => img11.CanvasGroup.alpha = x, 1, 0.3f);

			var img22 = _cards[DecrementValue(2)];
			//var canv2 = img2.GetComponent<Canvas>();
			var rt22 = img22.GetComponent<RectTransform>();
			var tw22 = rt22.DOLocalMove(_leftRect2.localPosition, 0.3f);
			tw22.OnUpdate(() =>
			{
				if (tw22.position > (tw22.fullPosition / 2))
					img22.transform.SetSiblingIndex(img22.transform.parent.childCount - 5);
			});
			rt22.DOScale(_leftRect2.localScale, 0.3f);
			//DOTween.To(() => canv2.sortingOrder, (x) => canv2.sortingOrder = x, 1, 0.3f);
			img22.FillImage.DOColor(wc3, 0.3f);
			DOTween.To(() => img22.CanvasGroup.alpha, (x) => img22.CanvasGroup.alpha = x, 1, 0.3f);



			if (_cards.Count == 6)
			{
				var img3 = _cards[DecrementValue(3)];
				//var canv3 = img3.GetComponent<Canvas>();
				var rt3 = img3.GetComponent<RectTransform>();
				//DOTween.To(() => canv3.sortingOrder, (x) => canv3.sortingOrder = x, 0, 0.3f);
				var tw3 = img3.FillImage.DOColor(wc3, 0.3f);
				DOTween.To(() => img3.CanvasGroup.alpha, (x) => img3.CanvasGroup.alpha = x, 0, 0.3f);
				tw3.OnUpdate(() =>
				{
					if (tw3.position > (tw3.fullPosition / 2))
						img3.transform.SetSiblingIndex(img3.transform.parent.childCount - 6);
				});
			}
			if (_cards.Count >= 7)
			{
				var img3 = _cards[DecrementValue(3)];
				//var canv3 = img3.GetComponent<Canvas>();
				var rt3 = img3.GetComponent<RectTransform>();
				//DOTween.To(() => canv3.sortingOrder, (x) => canv3.sortingOrder = x, 0, 0.3f);
				var tw3 = img3.FillImage.DOColor(wc3, 0.3f);
				DOTween.To(() => img3.CanvasGroup.alpha, (x) => img3.CanvasGroup.alpha = x, 0, 0.3f);
				tw3.OnUpdate(() =>
				{
					if (tw3.position > (tw3.fullPosition / 2))
						img3.transform.SetSiblingIndex(img3.transform.parent.childCount - 6);
				});

				var img4 = _cards[IncrementValue(3)];
				//var canv4 = img4.GetComponent<Canvas>();
				var rt4 = img4.GetComponent<RectTransform>();
				//DOTween.To(() => canv4.sortingOrder, (x) => canv4.sortingOrder = x, 0, 0.3f);
				var tw4 = img4.FillImage.DOColor(wc3, 0.3f);
				DOTween.To(() => img4.CanvasGroup.alpha, (x) => img4.CanvasGroup.alpha = x, 0, 0.3f);
				tw4.OnUpdate(() =>
				{
					if (tw4.position > (tw4.fullPosition / 2))
						img4.transform.SetSiblingIndex(img4.transform.parent.childCount - 6);
				});
			}



		}

		//private IEnumerator TimerChnage()
		//{
		//	yield return new WaitForSeconds(_timeChnage);
		//	RightButton();
		//}

		//private void PlayWaitChange()
		//{
		//if (!_animate) return;
		//	_timerCoroutine = StartCoroutine(TimerChnage());
		//}
		public void MoveLeft()
		{
			it.Logger.Log("move left");
			//if (_timerCoroutine != null)
			//	StopCoroutine(_timerCoroutine);
			_index = DecrementValue();
			ConfirmPosition();

			//PlayWaitChange();
		}

		public void MoveRight()
		{
			it.Logger.Log("move right");
			//if (_timerCoroutine != null)
			//	StopCoroutine(_timerCoroutine);

			_index = IncrementValue();
			ConfirmPosition();

			//PlayWaitChange();
		}

		public void LeftButtonTouch()
		{

			if (!CustomMuve) return;
			MoveLeft();
		}

		public void RightButtonTouch()
		{

			if (!CustomMuve) return;
			MoveRight();
		}

		private int IncrementValue(int inc = 1)
		{
			int val = _index;
			for (int i = 0; i < inc; i++)
			{
				val++;
				if (val >= _cards.Count) val = 0;
			}
			return val;
		}

		private int DecrementValue(int inc = 1)
		{
			int val = _index;
			for (int i = 0; i < inc; i++)
			{
				val--;
				if (val < 0) val = _cards.Count - 1;
			}
			return val;
		}




	}
}