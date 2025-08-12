using Game.Game.Common;
using Game.Providers.Avatars;
using Game.Providers.Battles.Components;
using Game.Providers.Profile;
using Game.Providers.Ui.Presenters.Base;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace Game.Providers.Ui.Presenters
{
	public class GameDuelResultWindowPresenter : WindowPresenter
	{
		[HideInInspector] public UnityEvent OnExit = new();
		[HideInInspector] public UnityEvent OnRematch = new();

		[SerializeField] private Button _rematchButton;
		[SerializeField] private Button _exitButton;

		[SerializeField] private PlayerBlock _playerBlock;
		[SerializeField] private PlayerBlock _opponentBlock;

		[SerializeField] private ColorsData _winColors;
		[SerializeField] private ColorsData _lossColors;

		private GameSession _gameSession;
		private IProfileProvider _profileProvider;
		private IAvatarsProvider _avatarsProvider;
		private BattleDuelState _state;

		[Inject]
		private void Build(GameSession gameSession, IProfileProvider profileProvider, IAvatarsProvider avatarsProvider)
		{
			_gameSession = gameSession;
			_profileProvider = profileProvider;
			_avatarsProvider = avatarsProvider;
			_exitButton.onClick.RemoveAllListeners();
			_exitButton.onClick.AddListener(ExitButtonTouch);
			_rematchButton.onClick.RemoveAllListeners();
			_rematchButton.onClick.AddListener(RematchButtonTouch);
		}

		private void RematchButtonTouch() => OnRematch?.Invoke();
		private void ExitButtonTouch() => OnExit?.Invoke();

		public void SetState(BattleDuelState state)
		{
			_state = state;

			FillData(_playerBlock,
			_state.IsPlayerWin ? _winColors : _lossColors,
			_avatarsProvider.GetTexture(_profileProvider.Avatar),
			_state.PlayerState.WinCount,
			_state.PlayerState.WinCoins
			);

			FillData(_opponentBlock,
			_state.IsPlayerWin ? _lossColors : _winColors,
			_state.BotState.Avatar,
			_state.BotState.WinCount,
			_state.BotState.WinCoins
			);
		}

		public override void Show()
		{
			base.Show();
		}

		public void FillData(
			PlayerBlock block,
			ColorsData colors,
			Texture2D avatar,
			int winCount,
			int coins
		)
		{
			block.TitleLabel.text = colors.Title;
			block.TitleLabel.fontMaterial = colors.TitleMaterial;
			block.Avatar.texture = avatar;
			block.Border.sprite = colors.AvatarBorderSprite;
			block.WinLabel.text = winCount.ToString();
			block.RewardBack.sprite = colors.CoinsBack;
			block.RewardLabel.text = $"<sprite=0><size=32>{coins}";
		}

		[System.Serializable]
		public struct PlayerBlock
		{
			public TMP_Text TitleLabel;
			public RawImage Avatar;
			public Image Border;
			public TMP_Text WinLabel;
			public Image RewardBack;
			public TMP_Text RewardLabel;
		}

		[System.Serializable]
		public struct ColorsData
		{
			public string Title;
			public Material TitleMaterial;
			public Sprite AvatarBorderSprite;
			public Sprite CoinsBack;
		}
	}
}
