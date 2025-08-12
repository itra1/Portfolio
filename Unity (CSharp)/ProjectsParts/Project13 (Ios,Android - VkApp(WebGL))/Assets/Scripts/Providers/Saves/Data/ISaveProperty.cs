namespace Game.Providers.Saves.Data
{
	public interface ISaveProperty<T> : ISaveItem
	{
		string Name { get; }
		T Value { get; set; }

		T DefaultValue { get; }
	}
}
