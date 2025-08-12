namespace ScreenStreaming.Parameters
{
    public interface IScreenStreamingParameters
    {
        bool IsEnabled { get; }
        string ServerUrl { get; }
    }
}