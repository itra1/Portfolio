using Game.Scripts.Providers.Saves;
using UnityEngine.Events;

namespace Game.Scripts.Providers.Profiles.Handlers
{
	public class ProfileStarsHandler : IProfileStarsHandler
	{
		public UnityEvent<int, int> OnStarsChange { get; set; } = new();

		private IProfileProvider _profileProvider;
		private ISaveHandler _saveHandler;

		public ProfileStarsHandler(IProfileProvider profileProvider, ISaveHandler saveHandler)
		{
			_profileProvider = profileProvider;
			_saveHandler = saveHandler;
		}

		public void AddStars(int stars)
		{
			_profileProvider.Profile.Stars += stars;

			OnStarsChange?.Invoke(stars, _profileProvider.Profile.Stars);

			_ = _saveHandler.Save();
		}
	}
}
