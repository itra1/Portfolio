using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Scripts.UI.Presenters.Base;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Presenters
{
	public class RewardFakePresenter : WindowPresenter
	{
		[HideInInspector] public UnityEvent ClaimTouchEvent = new();
		[HideInInspector] public UnityEvent CloseEvent = new();

		[SerializeField] private CanvasGroup _baseCG;
		[SerializeField] private Image _background;
		[SerializeField] private Transform _startParticles;
		[SerializeField] private Transform _groundParticles;
		[SerializeField] private ParticleSystem _getParticles;
		[SerializeField] private Button _claimButton;
		[SerializeField] private CanvasGroup _titleCG;
		[SerializeField] private CanvasGroup _claimButtonCG;
		[SerializeField] private Transform _productTransform;
		[SerializeField] private CanvasGroup _productCG;

		private ParticleSystem[] _groundParticlesSystems;
		private bool _isReward;

		[Inject]
		private void Constructor()
		{
			_claimButton.onClick.RemoveAllListeners();
			_claimButton.onClick.AddListener(ClaimButtonTouch);

			_groundParticlesSystems = _groundParticles.GetComponentsInChildren<ParticleSystem>(true);
		}

		private void ClaimButtonTouch()
		{
			if (_isReward)
				return;
			_isReward = true;
			_ = ClaimAnimation();
		}

		public override async UniTask Show()
		{
			_isReward = false;
			_baseCG.alpha = 1;
			_titleCG.alpha = 0;
			_claimButtonCG.alpha = 0;
			_productCG.alpha = 0;

			_startParticles.gameObject.SetActive(false);
			_groundParticles.gameObject.SetActive(false);
			_background.color = new Color(1, 1, 1, 0);
			_ = base.Show();
			_ = DOTween.To(() => _background.color, (x) => _background.color = x, Color.white, 0.5f);
			_getParticles.Play();
			SetGroundMaxParticles(0);
			_ = DOTween.To(() => _groundParticlesSystems[0].main.maxParticles, (x) => SetGroundMaxParticles(x), 1000, 0.5f);
			_groundParticles.gameObject.SetActive(true);
			_ = GroundParticleAnim();
			await UniTask.Delay(500);

			_productTransform.localScale = Vector3.one * 4;
			_ = DOTween.To(() => _productCG.alpha, (x) => _productCG.alpha = x, 1, 0.5f);
			_ = DOTween.To(() => _productTransform.localScale, (x) => _productTransform.localScale = x, Vector3.one, 0.5f);
			await UniTask.Delay(500);
			_ = DOTween.To(() => _titleCG.alpha, (x) => _titleCG.alpha = x, 1, 0.5f);
			_ = DOTween.To(() => _claimButtonCG.alpha, (x) => _claimButtonCG.alpha = x, 1, 0.5f);
		}

		private async UniTask GroundParticleAnim()
		{
			_ = DOTween.To(() => _groundParticlesSystems[0].main.maxParticles, (x) => SetGroundMaxParticles(x), 1000, 0.3f);
			await UniTask.Delay(300);
			_ = DOTween.To(() => _groundParticlesSystems[0].main.maxParticles, (x) => SetGroundMaxParticles(x), 0, 1);
		}

		private void SetGroundMaxParticles(int value)
		{
			for (var i = 0; i < _groundParticlesSystems.Length; i++)
			{
				var particles = _groundParticlesSystems[i].main;
				particles.maxParticles = value;
			}
		}

		private async UniTask ClaimAnimation()
		{
			//_getParticles.Play();
			//await UniTask.Delay(1000);
			ClaimTouchEvent?.Invoke();
			_ = DOTween.To(() => _baseCG.alpha, (x) => _baseCG.alpha = x, 0, 0.5f);
			await UniTask.Delay(500);
			CloseEvent?.Invoke();
		}
	}
}
