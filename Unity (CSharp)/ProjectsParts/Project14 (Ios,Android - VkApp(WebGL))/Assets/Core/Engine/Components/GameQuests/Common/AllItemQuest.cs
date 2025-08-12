
using Assets.Core.Engine.Signals;

namespace Core.Engine.Components.GameQuests
{
/// <summary>
/// Квест по прохождению уровня полностью
/// </summary>
	[GameQuestType(GameQuestType.AllItems)]
	public class AllItemQuest : GameQuest
	{
		public override void LevelStart()
		{
			base.LevelStart();
		}

		protected override void AfterInject()
		{
			_signalBus.Subscribe<GameItemsAllDestroySignal>(AllItemsDestroy);
		}

		protected override void AfterDispose()
		{
			base.AfterDispose();
			_signalBus.Unsubscribe<GameItemsAllDestroySignal>(AllItemsDestroy);
		}

		private void AllItemsDestroy(GameItemsAllDestroySignal signal)
		{
			LevelComplete();
		}
	}
}
