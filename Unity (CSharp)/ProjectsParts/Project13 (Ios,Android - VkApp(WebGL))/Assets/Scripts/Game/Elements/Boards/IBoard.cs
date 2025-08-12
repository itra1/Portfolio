using Game.Game.Settings;
using Game.Providers.Battles.Interfaces;

namespace Game.Game.Elements.Boards
{
	public interface IBoard
	{
		void Destroy(bool isPlayer = true);
		void SetData(BoardsSettings.BoardItem boardItem);
		void SetData(IBoardStageSettings stageData);
	}
}
