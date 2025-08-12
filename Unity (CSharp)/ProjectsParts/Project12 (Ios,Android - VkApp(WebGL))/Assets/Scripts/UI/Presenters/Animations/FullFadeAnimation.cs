using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Game.Scripts.UI.Presenters.Animations
{
	public class FullFadeAnimation : MonoBehaviour, IShowPresenterAnimation, IHidePresenterAnimation
	{
		[SerializeField] private GameObject _body;
		[SerializeField] private float _timeAnimation = 0.4f;
		[SerializeField] private bool _showAnimation = true;
		[SerializeField] private bool _hideAnimation = true;

		private CanvasGroup _cg;

		public async UniTask ShowAnimation()
		{
			InitComponents();

			if (!_showAnimation)
			{
				_cg.alpha = 1;
				gameObject.SetActive(true);
				return;
			}

			_cg.alpha = 0;
			_cg.blocksRaycasts = false;
			gameObject.SetActive(true);
			await DOTween.To(() => _cg.alpha,
			(x) => _cg.alpha = x,
			1,
			_timeAnimation).ToUniTask();
			_cg.blocksRaycasts = true;
		}

		public async UniTask HideAnimation()
		{
			InitComponents();

			if (!_hideAnimation)
			{
				_cg.alpha = 0;
				gameObject.SetActive(false);
				return;
			}

			_cg.blocksRaycasts = false;
			_cg.alpha = 1;
			await DOTween.To(() => _cg.alpha,
			(x) => _cg.alpha = x,
			0,
			_timeAnimation).ToUniTask();
			_cg.blocksRaycasts = true;
			gameObject.SetActive(false);
		}

		private void InitComponents()
		{
			if (_cg == null)
				_cg = gameObject.GetOrAddComponent<CanvasGroup>();
		}
	}
}
