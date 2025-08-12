namespace Core.App
{
	public interface IApplicationState
	{
		bool IsScreenLocked { get; }
		public bool IsAppLoadingCompleted { get; }
	}
}