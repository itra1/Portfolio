
namespace Core.Engine.Components.Vibrations
{
	public interface IVibrationProvider
	{
		bool IsEnabled { get; }

		void Vibrate();
		void VibratePop();
		void VibratePeek();
		void VibrateNope();
	}
}
