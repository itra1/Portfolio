namespace ScreenStreaming.Sender.Source.Factory
{
    public class ApplicationVideoStreamSourceFactory : IApplicationVideoStreamSourceFactory
    {
        public IApplicationVideoStreamSource Create(ApplicationVideoStreamSender parent) =>
            new ApplicationVideoStreamSource(parent);
    }
}