
namespace Core.Engine.Components.GameQuests
{
	[GameQuestType(GameQuestType.TargetItems)]
	public class TargetItemsQuest : GameQuest
	{
		/// <summary>
		/// Целевое количество очков
		/// </summary>
		public int TargetPoints { get; set; }
	}
}
