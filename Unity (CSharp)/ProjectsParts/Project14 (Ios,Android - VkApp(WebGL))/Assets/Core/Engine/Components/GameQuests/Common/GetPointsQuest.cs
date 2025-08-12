using Assets.Core.Engine.Signals;

using Core.Engine.Signals;

namespace Core.Engine.Components.GameQuests
{
	/// <summary>
	/// Квест по зарабатыванию определенного количества очков
	/// </summary>
	[GameQuestType(GameQuestType.GetPoints)]
	public class GetPointsQuest : GameQuest
	{
		/// <summary>
		/// Целевое количество элементов
		/// </summary>
		public int TargetPoints { get; set; }

		public override void LevelStart()
		{
			base.LevelStart();
		}

		protected override void AfterInject()
		{
			_signalBus.Subscribe<GamePointChangeSignal>(OnGamePointChangeSignal);
		}

		protected override void AfterDispose()
		{
			base.AfterDispose();
			_signalBus.Unsubscribe<GamePointChangeSignal>(OnGamePointChangeSignal);
		}

		private void OnGamePointChangeSignal(GamePointChangeSignal signal)
		{
			if (TargetPoints <= signal.Value)
				LevelComplete();
		}
	}
}
