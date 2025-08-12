using Core.Engine.Signals;

namespace Core.Engine.Components.GameQuests
{
/// <summary>
/// Установлен активный квест
/// </summary>
	public class ActiveQuestSetSignal : ISignal
	{
	public GameQuest Quest { get; set; }
	}
}
