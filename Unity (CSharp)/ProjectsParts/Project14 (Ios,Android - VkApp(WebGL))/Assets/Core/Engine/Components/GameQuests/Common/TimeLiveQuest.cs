using Assets.Core.Engine.Signals;

using UnityEngine;

namespace Core.Engine.Components.GameQuests
{
	/// <summary>
	/// Выжать время
	/// </summary>
	[GameQuestType(GameQuestType.TimeLive)]
	public class TimeLiveQuest : GameQuest
	{
		/// <summary>
		/// Целевое количество секунд
		/// </summary>
		public float TargetSeconds { get; set; }
		private float _startSeconds = 0;
		public float CurrentSeconds => Time.realtimeSinceStartup - _startSeconds;
		public float SecondLeft => _isStarted ? TargetSeconds - CurrentSeconds : 0;
		public override void LevelStart()
		{
			base.LevelStart();
			_startSeconds = Time.realtimeSinceStartup;
		}

		protected override void AfterInject()
		{
		}

		protected override void AfterDispose()
		{
		}

		public override void Tick()
		{
			if (!_isStarted) return;
			base.Tick();
			if(Time.realtimeSinceStartup - _startSeconds >= TargetSeconds){
				LevelComplete();
			}
		}

	}
}
