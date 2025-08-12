using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Providers.Ui.Attributes;
using Game.Providers.Ui.Base;
using Game.Providers.Ui.Controllers.Base;
using Game.Providers.Ui.Elements;
using Game.Providers.Ui.Windows.Elements;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Providers.Ui.Controllers
{
	[UiController(WindowPresenterType.Window, WindowPresenterNames.GamePlayDuel)]
	public class GamePlayDuelWindowPresenterController : WindowPresenterController<GamePlayDuelWindowPresenter>
	{
		public UnityAction<string> OnPlayerWeaponSelect;

		protected override void AfterCreateWindow()
		{
			Presenter.WinPanel.gameObject.SetActive(false);
			Presenter.LossPanel.gameObject.SetActive(false);
			base.AfterCreateWindow();

			Presenter.WeaponTool.OnSelectWeapon = (weaponType) => { OnPlayerWeaponSelect?.Invoke(weaponType); };
		}

		public async UniTask ShowWin(UnityAction onMiddleAnimation)
		{
			await AnimationStateResult(Presenter.WinPanel, onMiddleAnimation);
		}

		public async UniTask ShowLoss(UnityAction onMiddleAnimation)
		{
			await AnimationStateResult(Presenter.LossPanel, onMiddleAnimation);
		}

		private async UniTask AnimationStateResult(GamePlayDuelResultPanel targetPanel, UnityAction onMiddleAnimation)
		{
			Color colorWhite = new Color(1, 1, 1, 1);
			Color colorTransparent = new Color(1, 1, 1, 0);
			Color colorLght = targetPanel.LightImage.color;
			colorLght.a = 1;
			Color colorLghtTransparent = colorLght;
			colorLghtTransparent.a = 0;
			targetPanel.TitleImage.rectTransform.localScale = Vector3.zero;
			targetPanel.BackgroundImage.color = colorTransparent;
			targetPanel.LightImage.color = colorLghtTransparent;
			targetPanel.TitleImage.color = colorTransparent;
			targetPanel.gameObject.SetActive(true);
			await targetPanel.BackgroundImage.DOColor(colorWhite, 0.2f).ToUniTask();
			_ = targetPanel.TitleImage.rectTransform.DOScale(Vector3.one, 0.3f);
			_ = targetPanel.LightImage.DOColor(colorLght, 0.3f);
			await targetPanel.TitleImage.DOColor(colorWhite, 0.3f);

			await UniTask.Delay(300);
			onMiddleAnimation?.Invoke();
			_ = targetPanel.TitleImage.rectTransform.DOScale(Vector3.zero, 0.3f);
			_ = targetPanel.LightImage.DOColor(colorLghtTransparent, 0.3f);
			await targetPanel.TitleImage.DOColor(colorTransparent, 0.3f);
			await targetPanel.BackgroundImage.DOColor(colorTransparent, 0.2f).ToUniTask();
			await UniTask.Delay(100);
			targetPanel.gameObject.SetActive(false);
		}

		public void SetBotAvatar(Texture2D avatar) => Presenter.BotAvatarImage.texture = avatar;

		public void SetVisibleRoundTimer(bool isVisible) =>
			Presenter.TimerLabel.gameObject.SetActive(isVisible);

		public void SetRoundTimer(string value) =>
			Presenter.TimerLabel.text = value;

		public void SetRoundLabel(int currentStage, int allStage) =>
			Presenter.RoundLabel.text = $"Round {currentStage}/{allStage}";

		public void SetPlayerWinLabel(int value) =>
			Presenter.PlayerWinLabel.text = value.ToString();

		public void SetBotWinLabel(int value) =>
			Presenter.BotWinLabel.text = value.ToString();

		public void SetPlayerHitLabel(int value, int maxValue) =>
		Presenter.PlayerHitIndicator.ChangeCount(value, maxValue);

		public void SetBotHitLabel(int value, int maxValue) =>
		Presenter.BotHitIndicator.ChangeCount(value, maxValue);

	}
}
