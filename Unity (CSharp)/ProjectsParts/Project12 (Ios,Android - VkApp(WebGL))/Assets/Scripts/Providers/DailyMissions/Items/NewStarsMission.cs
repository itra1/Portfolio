using Game.Scripts.Providers.DailyMissions.Base;
using Game.Scripts.Providers.DailyMissions.Common;
using Game.Scripts.Providers.Profiles.Handlers;
using itra.Attributes;
using Zenject;

namespace Game.Scripts.Providers.DailyMissions.Items
{
	[PrefabName(DailyMissionType.NewStars)]
	public class NewStarsMission : Mission
	{
		private IProfileStarsHandler _profileStarsHandler;

		public override string Type => DailyMissionType.NewStars;

		[Inject]
		private void Build(IProfileStarsHandler profileStarsHandler)
		{
			_profileStarsHandler = profileStarsHandler;
		}

		~NewStarsMission()
		{
			_profileStarsHandler.OnStarsChange.RemoveListener(OnAddNewStarsEventListener);
		}

		public override void Initialize()
		{
			base.Initialize();

			_profileStarsHandler.OnStarsChange.AddListener(OnAddNewStarsEventListener);
		}

		private void OnAddNewStarsEventListener(int newStars, int allStars)
		{
			AddItem(newStars);
		}
	}
}
