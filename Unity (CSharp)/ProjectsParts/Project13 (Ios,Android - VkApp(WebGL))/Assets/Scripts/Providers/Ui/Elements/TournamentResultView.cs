using System;
using Game.Game.Settings;
using Game.Providers.Avatars;
using Game.Providers.Battles;
using Game.Providers.Battles.Settings;
using Game.Providers.Profile;
using Game.Providers.Profile.Handlers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Providers.Ui.Elements
{
	public class TournamentResultView : MonoBehaviour, IPoolable<BattleResult, IMemoryPool>
	{
		[SerializeField] private TMP_Text _nicknameLabel;
		[SerializeField] private TMP_Text _countLabel;
		[SerializeField] private TMP_Text _feeLabel;
		[SerializeField] private TMP_Text _winLabel;
		[SerializeField] private TMP_Text _timeOldLabel;
		[SerializeField] private TMP_Text _opponentScore;
		[SerializeField] private Image _winResourceIcone;
		[SerializeField] private Image _feeResourceIcone;
		[SerializeField] private RectTransform _feeParent;
		[SerializeField] private RectTransform _winFlag;
		[SerializeField] private RectTransform _lossFlag;
		[SerializeField] private RectTransform _opponentScorePanel;
		[SerializeField] private Color _winCountcolor;
		[SerializeField] private Color _lossCountcolor;
		[SerializeField] private Button _claimButton;
		[SerializeField] private RectTransform _avatarBlock;
		[SerializeField] private RawImage _avatarImage;
		[SerializeField] private Image _waitAvatarImage;

		private IMemoryPool _pool;
		private BattleResult _tournamentResult;
		private IProfileProvider _profileProvider;
		private ResourcesIconsSettings _resourcesIconsSettings;
		private PlayerResourcesHandler _resourcesHandler;
		private IBattleProvider _tournamentProvider;
		private IAvatarsProvider _avatarsProvider;

		[Inject]
		public void Constructor(IProfileProvider profileProvider, ResourcesIconsSettings resourcesIconsSettings, PlayerResourcesHandler resourcesHandler, IBattleProvider tournamentProvider, IAvatarsProvider avatarsProvider)
		{
			_profileProvider = profileProvider;
			_resourcesIconsSettings = resourcesIconsSettings;
			_resourcesHandler = resourcesHandler;
			_tournamentProvider = tournamentProvider;
			_avatarsProvider = avatarsProvider;
		}

		public void OnDespawned()
		{

		}
		public void Despawned()
		{
			_pool.Despawn(this);
		}

		public void OnSpawned(BattleResult tournamentResult, IMemoryPool pool)
		{
			_pool = pool;
			_tournamentResult = tournamentResult;
			SetTournamentResult();
		}

		private void OnTournamentCompleteSignal()
		{
			SetTournamentResult();
		}

		public void SetTournamentResult()
		{
			var isWin = _tournamentResult.IsWin;
			var isComplete = _tournamentResult.IsComplete;

			var reward = _resourcesIconsSettings.Resource.Find(x => x.Name == _tournamentResult.Tournament.Reward.Type);
			var fee = _resourcesIconsSettings.Resource.Find(x => x.Name == _tournamentResult.Tournament.Fee.Type);

			_countLabel.text = _tournamentResult.Points.ToString();
			_winFlag.gameObject.SetActive(isComplete && isWin);
			_lossFlag.gameObject.SetActive(isComplete && !isWin);
			_nicknameLabel.text = !isComplete ? "Waiting for results" :
				isWin ? _profileProvider.Name : _tournamentResult.WinPlayer;
			_opponentScorePanel.gameObject.SetActive(isComplete);
			_opponentScore.gameObject.SetActive(isComplete);
			_opponentScore.text = _tournamentResult.OpponentPoints.ToString();
			_timeOldLabel.gameObject.SetActive(isComplete);
			_countLabel.text = _tournamentResult.Points.ToString();
			_timeOldLabel.text = "";
			_feeResourceIcone.sprite = fee.Icone;
			_feeLabel.text = _tournamentResult.Tournament.Fee.ValueToString;
			_feeLabel.color = Color.white;
			_winResourceIcone.sprite = reward.Icone;
			_winResourceIcone.gameObject.SetActive(isComplete);
			_feeResourceIcone.gameObject.SetActive(!isComplete);
			_feeParent.gameObject.SetActive(!isComplete);
			_claimButton.gameObject.SetActive(_tournamentResult.ExistsReward);
			_avatarBlock.gameObject.SetActive(isComplete);
			_avatarImage.texture = _avatarsProvider.GetTexture(isWin ? _profileProvider.Avatar : _tournamentResult.OpponentAvatar);
			_waitAvatarImage.gameObject.SetActive(!isComplete);
			if (isComplete)
			{
				//_timeOldLabel.text = _tournamentResult.DateComplete.ToString();
				_winLabel.color = isWin ? _winCountcolor : _lossCountcolor;
				_winLabel.text = isWin ? "+" + _tournamentResult.Tournament.Reward.ValueToString : "-" + _tournamentResult.Tournament.Fee.ValueToString;

				var timeOldComplete = (DateTime.Now - _tournamentResult.TimeComplete);

				if (timeOldComplete.TotalMinutes < 60)
					_timeOldLabel.text = $"{Mathf.Max(1, (int) timeOldComplete.TotalMinutes)} min. old";
				else if (timeOldComplete.TotalHours < 24)
					_timeOldLabel.text = $"{Mathf.Max(1, (int) timeOldComplete.TotalHours)} hour old";
				else
					_timeOldLabel.text = $"{Mathf.Max(1, (int) timeOldComplete.TotalDays)} hour old";
			}
			else
			{

			}
		}

		public void ClaimButtonTouch()
		{
			_ = _resourcesHandler.GetHandler(_tournamentResult.Tournament.Reward.Type).AddValue(_tournamentResult.Tournament.Reward.Value, transform as RectTransform);
			_tournamentResult.IsGetReward = true;
			_tournamentProvider.ResultsChangeEmit();
			SetTournamentResult();
		}

		public class Factory : PlaceholderFactory<BattleResult, TournamentResultView> { }
	}
}
