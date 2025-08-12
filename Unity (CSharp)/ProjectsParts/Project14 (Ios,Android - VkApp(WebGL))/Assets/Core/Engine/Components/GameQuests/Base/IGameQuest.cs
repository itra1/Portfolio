using UnityEngine.Events;

namespace Core.Engine.Components.GameQuests
{
	public interface IGameQuest
	{
	/// <summary>
	/// Событие старта
	/// </summary>
		public void LevelStart();

		/// <summary>
		/// ВЫполнение квеста
		/// </summary>
		public void LevelComplete();

		public IGameQuest SubscribeStart(UnityAction onComplete);
		public IGameQuest UnsubscribeStart(UnityAction onComplete);
		public IGameQuest SubscribeComplete(UnityAction onComplete);
		public IGameQuest UnsubscribeComplete(UnityAction onComplete);

	}
}
