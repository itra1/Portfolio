using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;
using it.Helpers;

namespace Garilla.Promotions
{
	public class PromotionIconeIncrement : MonoBehaviour
	{
		[SerializeField] private RectTransform _content;
		[SerializeField] private TextMeshProUGUI _text;

		private int _maxValue;
		private decimal _increment;

		private void OnEnable()
		{
		}

		public void SetData(int maxValue, decimal increment)
		{
			_maxValue = maxValue;
			_increment = increment;
			_text.text = _maxValue.ToString();
		}

		public IEnumerator Show()
		{
			if (_maxValue > 0)
				yield return ShowWithIncrement();
			else
				yield return ShowWithoutIncrement();
		}

		private IEnumerator ShowWithIncrement()
		{
			_content.localScale = Vector2.zero;
			_content.DOScale(Vector2.one, 0.3f);
			yield return new WaitForSeconds(1.3f);

			if (_increment == -1)
			{
				_content.DOScale(Vector2.zero, 0.3f);
				yield return new WaitForSeconds(0.3f);
				yield break;
			}

			Animator animator = _content.GetComponent<Animator>();
			animator.SetTrigger("change");
			_text.rectTransform.DOScale(Vector2.zero, 0.15f).OnComplete(() =>
			{
				_text.text = "+" + ((int)_increment).CurrencyString(true);
				_text.rectTransform.DOScale(Vector2.one, 0.15f);
				_content.DOScale(Vector2.zero, 0.3f).SetDelay(1.5f);
			}).SetDelay(0.1f);
			yield return new WaitForSeconds(1.9f);
		}

		private IEnumerator ShowWithoutIncrement()
		{
			Animator animator = _content.GetComponent<Animator>();
			animator.SetTrigger("award");
			_text.text = "+" + ((int)_increment).CurrencyString(true);
			_text.rectTransform.localScale = Vector2.one;

			_content.localScale = Vector2.zero;
			_content.DOScale(Vector2.one, 0.3f);
			yield return new WaitForSeconds(1.5f);
			_content.DOScale(Vector2.zero, 0.3f);
			yield return new WaitForSeconds(0.3f);

		}

	}
}