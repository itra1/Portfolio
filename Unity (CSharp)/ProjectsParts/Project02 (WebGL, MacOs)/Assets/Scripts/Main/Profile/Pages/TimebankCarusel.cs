using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Garilla;
using it;
using it.Main;

namespace Garilla.Main
{
	public class TimebankCarusel : MonoBehaviour
	{
		[SerializeField] private TimebankProduct[] _banners;
		[SerializeField] private RectTransform _leftRect;
		[SerializeField] private RectTransform _rightRect;
		[SerializeField] private RectTransform _centerRect1;
		[SerializeField] private RectTransform _centerRect2;
		[SerializeField] private RectTransform _bubushkaParent;
		[SerializeField] private Bubushka _bubushkaPrefab;
		[SerializeField] private float _alphaBack;

		private List<Bubushka> _bubushkaList = new List<Bubushka>();
		private int _index = 0;

		private float _timeChnage = 5;
		private Coroutine _timerCoroutine;


		private void OnEnable()
		{
			for (int i = 0; i < _banners.Length; i++)
			{
				_banners[i].OnBuy = (val) =>
				{
					BuyProduct(val);
				};
			}
			_banners[0].SetData("30", UserController.ReferenceData.timebank_price["30"]);
			_banners[1].SetData("60", UserController.ReferenceData.timebank_price["60"]);
			_banners[2].SetData("100", UserController.ReferenceData.timebank_price["100"]);
			_banners[3].SetData("200", UserController.ReferenceData.timebank_price["200"]);
#if UNITY_STANDALONE
			BubushkasSpawn();
			ConfirmPosition();
#endif

			if (_timerCoroutine != null)
				StopCoroutine(_timerCoroutine);

#if UNITY_STANDALONE
			PlayWaitChange();
#endif
		}


		private void BuyProduct(string productName)
		{

			if (!UserController.ReferenceData.timebank_price.ContainsKey(productName))
				return;

			var product = UserController.ReferenceData.timebank_price[productName];
//#if UNITY_ANDROID

//            it.Api.UserApi.BuyTimabank(float.Parse(productName), (result) =>
//            {

//                UserController.Instance.UpdateUser(result.user);

//            }, (error) =>
//            {


//            });
//#endif

//#if UNITY_STANDALONE
            var popup = it.Main.PopupController.Instance.ShowPopup<it.Popups.ConfirmPopup>(PopupType.Confirm);

			popup.SetDescriptionString("Are you certain that you want to buy Time Bank?");
			popup.OnConfirm = () =>
			{
				it.Api.UserApi.BuyTimabank(float.Parse(productName), (result) =>
				{
					
					UserController.Instance.UpdateUser(result.user);

				}, (error) =>
				{


				});
			};

			popup.OnCancel = () =>
			{

			};
//#endif

        }
#if UNITY_STANDALONE
		private void BubushkasSpawn()
		{
			_bubushkaPrefab.gameObject.SetActive(false);
			_bubushkaList.ForEach(x => x.gameObject.SetActive(false));

			int count = _banners.Length / 2;

			for (int i = 0; i < count; i++)
			{

				var cm = _bubushkaList.Find(x => !x.gameObject.activeInHierarchy);

				if (cm == null)
				{
					GameObject go = Instantiate(_bubushkaPrefab.gameObject, _bubushkaParent);
					cm = go.GetComponent<Bubushka>();
					_bubushkaList.Add(cm);
				}

				cm.Fill(false);
				cm.gameObject.SetActive(true);

			}
			_bubushkaParent.sizeDelta = new Vector2(15f * count, _bubushkaParent.sizeDelta.y);
		}
		private void ConfirmPosition()
		{
			UnfillBubushka();
			_bubushkaList[(int)Mathf.Ceil(_index/2)].Fill(true);

			Color wc = Color.white;
			Color wct = wc;
			wct.a = 0;

			float dist = (_centerRect1.localPosition - _leftRect.localPosition).sqrMagnitude;
			var img0 = _banners[_index];
			//var canv0 = img0.GetComponent<Canvas>();
			var rt0 = img0.GetComponent<RectTransform>();
			var tw0 = rt0.DOLocalMove(_centerRect1.localPosition, 0.3f);
			tw0.OnUpdate(() =>
			{
				if (tw0.position > (tw0.fullPosition / 2))
					img0.transform.SetSiblingIndex(img0.transform.parent.childCount - 1);
			});
			rt0.DOScale(_centerRect1.localScale, 0.3f);
			//DOTween.To(() => canv0.sortingOrder, (x) => canv0.sortingOrder = x, 2, 0.3f);
			//img0.DOColor(wc, 0.3f);
			var cg0 = img0.GetComponent<CanvasGroup>();
			DOTween.To(() => cg0.alpha, (x) => cg0.alpha = x, 1, 0.3f);

			var img1 = _banners[IncrementValue()];
			//var canv1 = img1.GetComponent<Canvas>();
			var rt1 = img1.GetComponent<RectTransform>();
			var tw1 = rt1.DOLocalMove(_centerRect2.localPosition, 0.3f);
			tw1.OnUpdate(() =>
			{
				if (tw1.position > (tw1.fullPosition / 2))
					img1.transform.SetSiblingIndex(img1.transform.parent.childCount - 2);
			});
			rt1.DOScale(_centerRect2.localScale, 0.3f);
			//DOTween.To(() => canv1.sortingOrder, (x) => canv1.sortingOrder = x, 1, 0.3f);
			//img1.DOColor(_colorBack, 0.3f);
			var cg1 = img1.GetComponent<CanvasGroup>();
			DOTween.To(() => cg1.alpha, (x) => cg1.alpha = x, 1, 0.3f);

			var img2 = _banners[IncrementValue(2)];
			//var canv2 = img2.GetComponent<Canvas>();
			var rt2 = img2.GetComponent<RectTransform>();
			var tw2 = rt2.DOLocalMove(_rightRect.localPosition, 0.3f);
			tw2.OnUpdate(() =>
			{
				if (tw2.position > (tw2.fullPosition / 2))
					img2.transform.SetSiblingIndex(img2.transform.parent.childCount - 3);
			});
			rt2.DOScale(_rightRect.localScale, 0.3f);
			//DOTween.To(() => canv2.sortingOrder, (x) => canv2.sortingOrder = x, 1, 0.3f);
			//img2.DOColor(_colorBack, 0.3f);
			var cg2 = img2.GetComponent<CanvasGroup>();
			DOTween.To(() => cg2.alpha, (x) => cg2.alpha = x, _alphaBack, 0.3f);

			var img3 = _banners[IncrementValue(3)];
			//var canv2 = img2.GetComponent<Canvas>();
			var rt3 = img3.GetComponent<RectTransform>();
			var tw3 = rt3.DOLocalMove(_leftRect.localPosition, 0.3f);
			tw3.OnUpdate(() =>
			{
				if (tw3.position > (tw3.fullPosition / 2))
					img3.transform.SetSiblingIndex(img3.transform.parent.childCount - 3);
			});
			rt3.DOScale(_leftRect.localScale, 0.3f);
			//DOTween.To(() => canv2.sortingOrder, (x) => canv2.sortingOrder = x, 1, 0.3f);
			//img2.DOColor(_colorBack, 0.3f);
			var cg3 = img3.GetComponent<CanvasGroup>();
			DOTween.To(() => cg3.alpha, (x) => cg3.alpha = x, _alphaBack, 0.3f);


			//if (_banners.Length == 4)
			//{
			//	var img3 = _banners[DecrementValue(2)].Banner;
			//	//var canv3 = img3.GetComponent<Canvas>();
			//	var rt3 = img3.GetComponent<RectTransform>();
			//	//DOTween.To(() => canv3.sortingOrder, (x) => canv3.sortingOrder = x, 0, 0.3f);
			//	var tw3 = img3.DOColor(wct, 0.3f);
			//	tw3.OnUpdate(() =>
			//	{
			//		if (tw3.position > (tw3.fullPosition / 2))
			//			img3.transform.SetSiblingIndex(img3.transform.parent.childCount - 3);
			//	});
			//}
			//if (_banners.Length >= 5)
			//{
			//	var img3 = _banners[DecrementValue(2)].Banner;
			//	//var canv3 = img3.GetComponent<Canvas>();
			//	var rt3 = img3.GetComponent<RectTransform>();
			//	//DOTween.To(() => canv3.sortingOrder, (x) => canv3.sortingOrder = x, 0, 0.3f);
			//	var tw3 = img3.DOColor(wct, 0.3f);
			//	tw3.OnUpdate(() =>
			//	{
			//		if (tw3.position > (tw3.fullPosition / 2))
			//			img3.transform.SetSiblingIndex(img3.transform.parent.childCount - 3);
			//	});

			//	var img4 = _banners[IncrementValue(2)].Banner;
			//	//var canv4 = img4.GetComponent<Canvas>();
			//	var rt4 = img4.GetComponent<RectTransform>();
			//	//DOTween.To(() => canv4.sortingOrder, (x) => canv4.sortingOrder = x, 0, 0.3f);
			//	var tw4 = img4.DOColor(wct, 0.3f);
			//	tw4.OnUpdate(() =>
			//	{
			//		if (tw4.position > (tw4.fullPosition / 2))
			//			img4.transform.SetSiblingIndex(img4.transform.parent.childCount - 3);
			//	});
			//}

		}
		private void UnfillBubushka()
		{
			_bubushkaList.ForEach(x => x.Fill(false));
		}

		private int IncrementValue(int inc = 1)
		{
			int val = _index;
			for (int i = 0; i < inc; i++)
			{
				val++;
				if (val >= _banners.Length) val = 0;
			}
			return val;
		}

		private int DecrementValue(int inc = 1)
		{
			int val = _index;
			for (int i = 0; i < inc; i++)
			{
				val--;
				if (val < 0) val = _banners.Length - 1;
			}
			return val;
		}

		private void PlayWaitChange()
		{
			_timerCoroutine = StartCoroutine(TimerChnage());
		}
		private IEnumerator TimerChnage()
		{
			yield return new WaitForSeconds(_timeChnage);
			RightButton();
		}

		public void LeftButton()
		{
			if (_timerCoroutine != null)
				StopCoroutine(_timerCoroutine);

			_index = DecrementValue(2);
			ConfirmPosition();

			PlayWaitChange();
		}

		public void RightButton()
		{
			if (_timerCoroutine != null)
				StopCoroutine(_timerCoroutine);

			_index = IncrementValue(2);
			ConfirmPosition();

			PlayWaitChange();
		}

#endif
    }

}