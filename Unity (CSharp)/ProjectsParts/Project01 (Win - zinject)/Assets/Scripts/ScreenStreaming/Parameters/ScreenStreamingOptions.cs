using Core.Options;

namespace ScreenStreaming.Parameters
{
    public class ScreenStreamingOptions : IScreenStreamingParameters
    {
        private readonly IApplicationOptions _options;
        
        public bool IsEnabled => _options.IsRenderStreamingEnabled;
        public string ServerUrl => _options.RenderStreamingUrl;
        
        public ScreenStreamingOptions(IApplicationOptions options) => _options = options;
    }
}