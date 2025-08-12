using Cysharp.Threading.Tasks;
using Game.Scripts.Providers.Profiles.Settings;
using Game.Scripts.Providers.Saves;
using Game.Scripts.Managers.Dialogs;
using Game.Scripts.UI;
using Game.Scripts.UI.Controllers;
using UnityEngine.Events;

namespace Game.Scripts.Providers.Profiles.Handlers
{
	/// <summary>
	/// Обрабатывает получение нового уровня на основе полученных звезд
	/// </summary>
	public class ProfileLevelHandler : IProfileLevelHandler
	{
		public UnityEvent<int> OnLevelChangeEvent { get; } = new();

		private readonly IProfileProvider _profileProvider;
		private readonly IProfileLevelSettings _profileLevelSettings;
		private readonly IProfileStarsHandler _profileStarsHandler;
		private readonly IDialogVisibleOrderHelper _dialogOrderHelper;
		private readonly IUiNavigator _uiNavigator;
		private readonly ISaveHandler _saveHandler;

		public ProfileLevelHandler(
			IProfileProvider profileProvider,
			IProfileLevelSettings profileLevelSettings,
			IProfileStarsHandler profileStarsHandler,
			IDialogVisibleOrderHelper dialogOrderHelper,
			IUiNavigator uiNavigator,
			ISaveHandler saveHandler
		)
		{
			_profileProvider = profileProvider;
			_profileLevelSettings = profileLevelSettings;
			_profileStarsHandler = profileStarsHandler;
			_dialogOrderHelper = dialogOrderHelper;
			_uiNavigator = uiNavigator;
			_saveHandler = saveHandler;

			_profileStarsHandler.OnStarsChange.AddListener(StarsChangeEvent);
		}

		private void StarsChangeEvent(int newStars, int sllStars)
		{
			_profileProvider.Profile.StarsInLevel += newStars;
			var newLevel = _profileProvider.Profile.StarsInLevel / _profileLevelSettings.StarsPerLevel;

			if (newLevel > 0)
			{
				AddLevels(newLevel);
				_profileProvider.Profile.StarsInLevel %= _profileLevelSettings.StarsPerLevel;
			}
		}

		public void AddOneLevel()
		{
			AddLevels(1);
		}

		private void AddLevels(int level)
		{
			_profileProvider.Profile.Level += level;
			OnLevelChangeEvent?.Invoke(_profileProvider.Profile.Level);
			_dialogOrderHelper.AddJob(NewLevel);
			_ = _saveHandler.Save();
		}

		private async UniTask NewLevel()
		{
			var dialogController = _uiNavigator.GetController<LevelUpPresenterController>();

			_ = await dialogController.Open();

			dialogController.SetLevel(_profileProvider.Profile.Level);

			await UniTask.WaitUntil(() => dialogController.IsOpen);
			await UniTask.WaitUntil(() => !dialogController.IsOpen);
		}
	}
}
