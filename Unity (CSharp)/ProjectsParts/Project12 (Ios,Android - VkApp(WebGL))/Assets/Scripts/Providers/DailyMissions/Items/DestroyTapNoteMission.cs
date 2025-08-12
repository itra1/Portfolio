using Engine.Scripts.Managers;
using Engine.Scripts.Timelines;
using Engine.Scripts.Timelines.Notes;
using Engine.Scripts.Timelines.Notes.Common;
using Game.Scripts.Providers.DailyMissions.Base;
using Game.Scripts.Providers.DailyMissions.Common;
using itra.Attributes;
using Zenject;

namespace Game.Scripts.Providers.DailyMissions.Items
{
	[PrefabName(DailyMissionType.DestroyTapNote)]
	public class DestroyTapNoteMission : Mission
	{
		private IRhythmProcessor _rhythmProcessor;
		private IGameHandler _gameHandler;
		public override string Type => DailyMissionType.DestroyTapNote;

		[Inject]
		private void Build(IRhythmProcessor rhythmProcessor, IGameHandler gameHandler)
		{
			_rhythmProcessor = rhythmProcessor;
			_gameHandler = gameHandler;
		}
		~DestroyTapNoteMission()
		{
			_rhythmProcessor.OnNoteTriggerEvent.RemoveListener(OnNoteTriggerEventListener);
			_gameHandler.OnGameChangeEvent.RemoveListener(OnGameChangeEventListener);
		}
		public override void Initialize()
		{
			base.Initialize();

			_rhythmProcessor.OnNoteTriggerEvent.AddListener(OnNoteTriggerEventListener);
			_gameHandler.OnGameChangeEvent.AddListener(OnGameChangeEventListener);
		}

		private void OnGameChangeEventListener(RhythmTimelineAsset song, bool isStartGame)
		{
			if (!isStartGame)
			{
				_eventData.NeedSave = true;
				_eventData.EventType = MissionEventType.Clear;
				StateChangeEmit();
			}
		}

		private void OnNoteTriggerEventListener(NoteTriggerEventData data)
		{
			if (data.IsLoss)
				return;

			if (data.Note is not TapNote)
				return;

			_eventData.NeedSave = false;
			AddItem(1);
		}
	}
}
