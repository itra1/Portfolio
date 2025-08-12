
namespace Core.Engine.Components.SaveGame
{
	/// <summary>
	/// Игровой уровень
	/// </summary>
	public class GameLevelSG : SaveProperty<int>
	{
		public override int DefaultValue => 0;
	}
}
