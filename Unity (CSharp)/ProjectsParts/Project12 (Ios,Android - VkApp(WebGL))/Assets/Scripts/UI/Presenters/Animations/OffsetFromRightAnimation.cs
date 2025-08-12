using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Game.Scripts.UI.Presenters.Animations
{
	public class OffsetFromRightAnimation : MonoBehaviour, IShowPresenterAnimation, IHidePresenterAnimation
	{
		[SerializeField] private RectTransform _body;
		[SerializeField] private float _timeAnimation = 0.3f;

		public async UniTask ShowAnimation()
		{
			_body.anchoredPosition = new(_body.rect.width, 0);
			gameObject.SetActive(true);
			await DOTween.To(() => _body.anchoredPosition,
			(x) => _body.anchoredPosition = x,
			Vector2.zero,
			_timeAnimation).ToUniTask();
		}
		public async UniTask HideAnimation()
		{
			await DOTween.To(() => _body.anchoredPosition,
			(x) => _body.anchoredPosition = x,
			new(_body.rect.width, 0),
			_timeAnimation).ToUniTask();
			gameObject.SetActive(false);
		}
	}
}
