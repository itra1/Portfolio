using System.Collections.Generic;
using Engine.Scripts.Managers;
using Engine.Scripts.Timelines;
using Game.Scripts.Providers.DailyMissions.Base;
using Game.Scripts.Providers.DailyMissions.Common;
using Game.Scripts.Providers.DailyMissions.Save;
using itra.Attributes;
using Zenject;

namespace Game.Scripts.Providers.DailyMissions.Items
{
	[PrefabName(DailyMissionType.SongExplorer)]
	public class SongExplorerMission : Mission
	{
		private IGameHandler _gameHandler;
		public override string Type => DailyMissionType.SongExplorer;

		[Inject]
		private void Build(IGameHandler gameHandler)
		{
			_gameHandler = gameHandler;
		}
		~SongExplorerMission()
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
			if (!isStartGame)
				return;

			if (_saveData is SongExplorerMissionSaveItem save)
			{
				if (save.SongList.Contains(song.Uuid))
					return;

				save.SongList.Add(song.Uuid);
				_saveData.Count = save.SongList.Count;
				_eventData.EventType = MissionEventType.CountChange;
			}

			_eventData.NeedSave = true;
			StateChangeEmit();
		}

		public override void SetSaveData(string data)
		{
			_saveData = Newtonsoft.Json.JsonConvert.DeserializeObject<SongExplorerMissionSaveItem>(data);
		}

		protected override void MakeSaveData()
		{
			_saveData = new SongExplorerMissionSaveItem()
			{
				Count = 0,
				Type = Type,
				Rewarded = false,
				SongList = new()
			};
		}

		public class SongExplorerMissionSaveItem : DailyMissionsSaveItem
		{
			public List<string> SongList = new();
		}
	}
}
