using Core.Engine.Signals;

namespace Core.Engine.Components.GameQuests
{
/// <summary>
/// Собраны предметы
/// </summary>
	public class GetItemSignal : ISignal
	{
		public string Alias;
		public int count = 1;
	}
}
