
namespace Core.Engine.Components.GameQuests
{
	/// <summary>
	/// Уничтожение определенное количество элементов
	/// </summary>
	[GameQuestType(GameQuestType.AnyItems)]
	public class AnyItemsQuest : GameQuest
	{
		/// <summary>
		/// Уничтожение целевого количества любых элементов
		/// </summary>
		public int TargetItems { get; set; }
	}
}
