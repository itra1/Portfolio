using Engine.Scripts.Managers;
using Engine.Scripts.Timelines;
using Game.Scripts.Providers.DailyMissions.Base;
using Game.Scripts.Providers.DailyMissions.Common;
using itra.Attributes;
using Zenject;

namespace Game.Scripts.Providers.DailyMissions.Items
{
	[PrefabName(DailyMissionType.CompleteTrack)]
	public class CompleteTrackMission : Mission
	{
		private IGameHandler _gameHandler;
		public override string Type => DailyMissionType.CompleteTrack;

		[Inject]
		private void Build(IGameHandler gameHandler)
		{
			_gameHandler = gameHandler;
		}
		~CompleteTrackMission()
		{
			_gameHandler.OnGameChangeEvent.RemoveListener(OnGameChangeEventListener);
		}
		public override void Initialize()
		{
			base.Initialize();

			_gameHandler.OnGameChangeEvent.AddListener(OnGameChangeEventListener);
		}

		private void OnGameChangeEventListener(RhythmTimelineAsset song, bool isStartGame)
		{
			if (isStartGame)
				return;

			_eventData.NeedSave = true;
			AddItem(1);
		}
	}
}
