

namespace Core.Engine.Signals
{
	/// <summary>
	/// Там по игровому экрану
	/// </summary>
	public class ScreenTapSignal : ISignal
	{
		public UnityEngine.Vector3 Position { get; set; }
	}
}
