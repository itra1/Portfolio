
using System.Collections.Generic;

using Zenject;

namespace Core.Engine.Components.GameQuests
{
	/// <summary>
	/// Кветы уровней
	/// </summary>
	public class GameQuestsProvider : IGameQuestProvider, ITickable
	{
		private readonly SignalBus _signalBus;
		private Dictionary<int, GameQuest> _questList = new();
		public GameQuest ActiveQuest { get; private set; }

		public GameQuestsProvider(SignalBus signalBus)
		{
			_signalBus = signalBus;
		}

		public void SetActiveQuest(int level)
		{
			if (_questList.ContainsKey(level))
			{
				SetActiveQuest(_questList[level]);
			}
		}

		public void SetActiveQuest(GameQuest quest)
		{
			ActiveQuest = quest;
			_signalBus.Fire(new ActiveQuestSetSignal() { Quest = ActiveQuest });
		}

		public void Clear()
		{
			ActiveQuest?.Dispose();
				ActiveQuest = null;
		}

		public void Tick()
		{
			ActiveQuest?.Tick();
		}
	}
}
