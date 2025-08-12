
namespace Core.Engine.Components.GameQuests
{
	/// <summary>
	/// Тип квеста
	/// </summary>
	public static class GameQuestType
	{
		/// <summary>
		/// Все предметы
		/// </summary>
		public const string AllItems = "AllItems";
		/// <summary>
		/// определенное число предметов
		/// </summary>
		public const string AnyItems = "AnyItems";
		/// <summary>
		/// Целевые предметы по alias
		/// </summary>
		public const string TargetItems = "TargetItems";
		/// <summary>
		/// Играть время
		/// </summary>
		public const string TimeLive = "TimeLive";
		/// <summary>
		/// Получить очки
		/// </summary>
		public const string GetPoints = "GetPoints";
	}
}
