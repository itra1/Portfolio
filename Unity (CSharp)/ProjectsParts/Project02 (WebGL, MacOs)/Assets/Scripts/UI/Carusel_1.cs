using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace Garilla.Main
{
	public class Carusel_1 : Carusel
	{
		protected override void FirstInit()
		{
			base.FirstInit();
			ConfirmPosition();
		}

		protected override void ConfirmPosition()
		{
			base.ConfirmPosition();


			Color wc = Color.white;
			Color wct = wc;
			wct.a = 0;

			float dist = (_centerRect.localPosition - _leftRect.localPosition).sqrMagnitude;
			var obj0 = _banners[_index];
			var img0 = obj0.Banner;
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
			obj0.Image.DOColor(wc, 0.3f);

			var obj1 = _banners[IncrementValue()];
			var img1 = obj1.Banner;
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
			obj1.Image.DOColor(wc, 0.3f);

			var obj2 = _banners[DecrementValue()];
			var img2 = obj2.Banner;
			//var canv2 = img2.GetComponent<Canvas>();
			var rt2 = img2.GetComponent<RectTransform>();
			var tw2 = rt2.DOLocalMove(_leftRect.localPosition, 0.3f);
			tw2.OnUpdate(() =>
			{
				if (tw2.position > (tw2.fullPosition / 2))
					img2.transform.SetSiblingIndex(img2.transform.parent.childCount - 2);
			});
			rt2.DOScale(_leftRect.localScale, 0.3f);
			//DOTween.To(() => canv2.sortingOrder, (x) => canv2.sortingOrder = x, 1, 0.3f);
			obj2.Image.DOColor(wc, 0.3f);

			if (_banners.Count == 4)
			{
				var obj3 = _banners[DecrementValue(2)];
				var img3 = obj3.Banner;
				//var canv3 = img3.GetComponent<Canvas>();
				var rt3 = img3.GetComponent<RectTransform>();
				//DOTween.To(() => canv3.sortingOrder, (x) => canv3.sortingOrder = x, 0, 0.3f);
				var tw3 = obj3.Image.DOColor(wct, 0.3f);
				tw3.OnUpdate(() =>
				{
					if (tw3.position > (tw3.fullPosition / 2))
						img3.transform.SetSiblingIndex(img3.transform.parent.childCount - 3);
				});
			}
			if (_banners.Count >= 5)
			{
				var obj3 = _banners[DecrementValue(2)];
				var img3 = obj3.Banner;
				//var canv3 = img3.GetComponent<Canvas>();
				var rt3 = img3.GetComponent<RectTransform>();
				//DOTween.To(() => canv3.sortingOrder, (x) => canv3.sortingOrder = x, 0, 0.3f);
				var tw3 = obj3.Image.DOColor(wct, 0.3f);
				tw3.OnUpdate(() =>
				{
					if (tw3.position > (tw3.fullPosition / 2))
						img3.transform.SetSiblingIndex(img3.transform.parent.childCount - 3);
				});

				var obj4 = _banners[IncrementValue(2)];
				var img4 = obj4.Banner;
				//var canv4 = img4.GetComponent<Canvas>();
				var rt4 = img4.GetComponent<RectTransform>();
				//DOTween.To(() => canv4.sortingOrder, (x) => canv4.sortingOrder = x, 0, 0.3f);
				var tw4 = obj4.Image.DOColor(wct, 0.3f);
				tw4.OnUpdate(() =>
				{
					if (tw4.position > (tw4.fullPosition / 2))
						img4.transform.SetSiblingIndex(img4.transform.parent.childCount - 3);
				});
			}

		}

	}
}