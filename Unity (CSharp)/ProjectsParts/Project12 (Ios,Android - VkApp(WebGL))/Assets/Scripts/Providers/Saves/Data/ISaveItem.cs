namespace Game.Scripts.Providers.Saves.Data
{
	public interface ISaveItem
	{
		void Load(string data);

		string Save();
	}
}
