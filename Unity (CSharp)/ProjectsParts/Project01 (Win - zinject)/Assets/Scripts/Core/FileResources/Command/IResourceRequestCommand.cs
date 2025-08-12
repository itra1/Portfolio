namespace Core.FileResources.Command
{
	public interface IResourceRequestCommand
	{
		string Url { get; }
		bool InProgress { get; }
		bool IsCanceled { get; }

		object Resources { get; }
		string FilePath { get; }

		void Cancel();
	}
}