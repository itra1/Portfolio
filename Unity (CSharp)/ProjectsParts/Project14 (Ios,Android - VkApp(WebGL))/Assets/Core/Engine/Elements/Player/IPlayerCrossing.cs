
namespace Core.Engine.Elements.Player
{
	/// <summary>
	/// Пересечение игроком
	/// </summary>
	public interface IPlayerCrossing
	{
		void PlayerCrossing();
		int Index { get; set; }
	}
}
