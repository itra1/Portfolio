namespace Core.Engine.Components.SaveGame
{
/// <summary>
/// Проперти сохранения
/// </summary>
/// <typeparam name="T"></typeparam>
	public interface ISaveProperty<T> : ISaveItem
	{
		string Name { get; }
		T Value { get; set; }

		T DefaultValue { get; }

		void Load();

		void Save();

	}
}
