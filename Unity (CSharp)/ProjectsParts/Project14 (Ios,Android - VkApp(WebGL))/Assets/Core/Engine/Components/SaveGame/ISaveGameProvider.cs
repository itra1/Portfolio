namespace Core.Engine.Components.SaveGame
{
	public interface ISaveGameProvider
	{
		bool IsInitiated { get; }

		public ISaveItem GetProperty<T>();

	}
}
