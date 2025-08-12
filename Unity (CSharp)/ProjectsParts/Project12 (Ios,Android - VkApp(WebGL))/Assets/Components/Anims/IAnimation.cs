namespace itra.Animations
{
	/// <summary>
	/// Basic animation interface
	/// </summary>
	public interface IAnimation
	{
		bool IsPlaying { get; }
		bool Play();
		bool Stop();
	}
}
