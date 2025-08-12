using Core.Engine.App.Base.Attributes;
using Core.Engine.Components.Audio;
using Core.Engine.Settings.Common;
using Core.Engine.Signals;
using UnityEngine;
using Zenject;

namespace Core.Engine.uGUI.Screens {
	[PrefabName(ScreenTypes.FirstPage)]
	public class FirstPageScreen : Screen, IFirstMenuScreen {
		private IScreensProvider _screenProvider;
		private string _policyUrl;

		[Inject]
		public void Initiate(IScreensProvider screenProvider
		, IPolicyUrl policy) {
			_screenProvider = screenProvider;
			_policyUrl = policy.PolicyUrl;
		}

		/// <summary>
		/// Кнопка плей
		/// </summary>
		public void PlayButtonTouch() {
			PlayAudio.PlaySound("click");
			//_signalBus.Fire(new UGUIButtonClickSignal() { Name = ButtonTypes.PlayGame });
			_signalBus.Fire<PlayGameSignal>();
		}

		/// <summary>
		/// Кнопка настроек
		/// </summary>
		public void SettingsButtonTouch() {
			PlayAudio.PlaySound("click");
			_signalBus.Fire(new UGUIButtonClickSignal() { Name = ButtonTypes.SettingsOpen });
		}

		/// <summary>
		/// Кнопка дневного бонуса
		/// </summary>
		public void DailyButtonTouch() {
			PlayAudio.PlaySound("click");
			_signalBus.Fire(new UGUIButtonClickSignal() { Name = ButtonTypes.DailyBonusOpen });
		}

		/// <summary>
		/// Кнопка политики
		/// </summary>
		public void PolicyButtonTouch() {
			PlayAudio.PlaySound("click");
			Application.OpenURL(_policyUrl);
		}
		/// <summary>
		/// Магазин
		/// </summary>
		public void ShopButtonTouch() {
			PlayAudio.PlaySound("click");
			_signalBus.Fire(new UGUIButtonClickSignal() { Name = ButtonTypes.Shop });
		}
		/// <summary>
		/// Скины
		/// </summary>
		public void SkinsButtonTouch() {
			PlayAudio.PlaySound("click");
			_signalBus.Fire(new UGUIButtonClickSignal() { Name = ButtonTypes.Skins });
		}

		public void LeaderboardButtonTouch() {

			PlayAudio.PlaySound("click");
			_ = _screenProvider.OpenWindow(ScreenTypes.Leaderboard);
		}

	}
}