using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Base;
using Game.Providers.Smiles.Handlers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Providers.Ui.Elements
{
	public class AvatarSmile : MonoBehaviour, IInjection
	{
		[SerializeField] private bool _isPlayer;
		[SerializeField] private Image _smileAvatar;

		private ISmileHndler _smileHelper;

		private bool _isVisible = false;

		[Inject]
		private void Constructor(ISmileHndler smileHelper)
		{
			_smileHelper = smileHelper;

			_smileAvatar.rectTransform.localScale = Vector3.zero;
			_smileHelper.OnSmileHandler.AddListener(SmileEvent);
		}

		private void OnEnable()
		{
			_smileAvatar.rectTransform.localScale = Vector3.zero;
			_isVisible = false;
		}

		private void SmileEvent(bool isPlayer, Sprite sprite)
		{
			if (_isPlayer != isPlayer)
				return;

			if (_isVisible)
				return;

			_smileAvatar.sprite = sprite;

			_ = SmileAnimation();
		}

		private async UniTask SmileAnimation()
		{
			_isVisible = true;

			await DOTween.To(() => _smileAvatar.rectTransform.localScale, (x) => _smileAvatar.rectTransform.localScale = x, Vector3.one * 1.6f, 0.3f).ToUniTask();
			await DOTween.To(() => _smileAvatar.rectTransform.localScale, (x) => _smileAvatar.rectTransform.localScale = x, Vector3.one, 0.1f).ToUniTask();

			await UniTask.Delay(700);

			await DOTween.To(() => _smileAvatar.rectTransform.localScale, (x) => _smileAvatar.rectTransform.localScale = x, Vector3.zero, 0.2f).ToUniTask();

			_isVisible = false;
		}

		private struct VisibleStep
		{
			public const string NoVisible = "NoVisible";
			public const string AnimVisible = "AnimVisible";
			public const string Visible = "Visible";
			public const string AnimHide = "AnimHide";
		}
	}
}
