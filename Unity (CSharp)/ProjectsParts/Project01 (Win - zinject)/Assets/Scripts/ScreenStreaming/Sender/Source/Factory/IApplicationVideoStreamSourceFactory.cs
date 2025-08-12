namespace ScreenStreaming.Sender.Source.Factory
{
    public interface IApplicationVideoStreamSourceFactory
    {
        IApplicationVideoStreamSource Create(ApplicationVideoStreamSender parent);
    }
}