namespace Game.Providers.Saves.Data
{
	public interface ISaveItem
	{
		void Load(string data);

		string Save();
	}
}
