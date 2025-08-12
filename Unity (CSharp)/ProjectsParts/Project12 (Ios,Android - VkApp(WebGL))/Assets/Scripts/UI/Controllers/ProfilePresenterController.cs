using Cysharp.Threading.Tasks;
using Game.Scripts.Providers.Avatars;
using Game.Scripts.Providers.Profiles;
using Game.Scripts.Providers.Profiles.Common;
using Game.Scripts.UI.Attributes;
using Game.Scripts.UI.Base;
using Game.Scripts.UI.Controllers.Base;
using Game.Scripts.UI.Presenters;
using Zenject;

namespace Game.Scripts.UI.Controllers
{
	[UiController(WindowPresenterType.Window, WindowPresenterNames.Profile)]
	public class ProfilePresenterController : WindowPresenterController<ProfilePresenter>
	{
		private IProfileProvider _profileProvider;
		private IAvatarsProvider _avatarsProvider;
		public override bool AddInNavigationStack => true;

		[Inject]
		private void Build(IProfileProvider profileProvider,
		IAvatarsProvider avatarsProvider)
		{
			_profileProvider = profileProvider;
			_avatarsProvider = avatarsProvider;
		}

		protected override void AfterCreateWindow()
		{
			base.AfterCreateWindow();
			Presenter.OnSaveEvent.AddListener(SaveEvent);
			Presenter.OnCloseEvent.AddListener(OnClose);
		}

		public override async UniTask<bool> Open()
		{
			if (Presenter == null)
				await LoadPresenter();

			Presenter.SetAvatars(_avatarsProvider.MaleAvatars, _avatarsProvider.FemaleAvatars);
			Presenter.SetProfile(_profileProvider.Profile);

			if (!await base.Open())
				return false;

			_profileProvider.OnProfileChange.AddListener(ProfileChangeEvent);

			return true;
		}

		public override async UniTask<bool> Close()
		{
			_profileProvider.OnProfileChange.RemoveListener(ProfileChangeEvent);

			if (!await base.Close())
				return false;

			return true;
		}

		private void ProfileChangeEvent(IProfile profile)
		{
			Presenter.SetProfile(profile);
		}

		private void OnClose()
		{
			//_ = UiNavigator.BackNavigation();
			_ = Close();
			//_ = _sourceOpen.Open(null);
		}

		private void SaveEvent(string avatarUuid)
		{
			_profileProvider.SetAvatar(avatarUuid);
			//OnClose();
		}
	}
}
