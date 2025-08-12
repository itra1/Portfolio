using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Garilla.Main
{
	public class Carusel_2 : Carusel
	{
		[SerializeField] private RectTransform _rightButton;
		[SerializeField] private RectTransform _leftButton;
		private RectTransform _baseRect;
		private CanvasGroup _rCg;
		private CanvasGroup _lCg;

		//public void ButtonsVisible(bool isVisible)
		//{
		//	if (_rCg == null)
		//		_rCg = _rightButton.GetComponent<CanvasGroup>();
		//	if (_lCg == null)
		//		_lCg = _leftButton.GetComponent<CanvasGroup>();

		//	DOTween.To(() => _rCg.alpha, (x) => _rCg.alpha = x, (isVisible ? 1 : 0), 0.3f);
		//	DOTween.To(() => _lCg.alpha, (x) => _lCg.alpha = x, (isVisible ? 1 : 0), 0.3f);

		//}
		protected override void FirstInit()
		{
			base.FirstInit();
			//_baseRect = _banners[0].Banner.GetComponent<RectTransform>();
			_baseRect = _parentImages;
			_banners[_index].Banner.SetAsLastSibling();
			int indIndex = IncrementValue();
			int decIndex = DecrementValue();
			_banners[indIndex].Banner.anchoredPosition = new Vector2(_parentImages.rect.width+15, _banners[indIndex].Banner.anchoredPosition.y);
			_banners[decIndex].Banner.anchoredPosition = new Vector2(-_parentImages.rect.width-15, _banners[decIndex].Banner.anchoredPosition.y);
			//ButtonsVisible(false);
		}

		protected override void ConfirmPosition()
		{
			base.ConfirmPosition();

			_banners[_index].Banner.DOAnchorPos(Vector2.zero, 0.3f).OnComplete(()=> {
				int indIndex = IncrementValue();
				int decIndex = DecrementValue();
				_banners[indIndex].Banner.anchoredPosition = new Vector2(_parentImages.rect.width + 15, _banners[indIndex].Banner.anchoredPosition.y);
				_banners[decIndex].Banner.anchoredPosition = new Vector2(-_parentImages.rect.width - 15, _banners[decIndex].Banner.anchoredPosition.y);
			});
			_banners[_index].Banner.transform.SetAsLastSibling();
		}
	}
}