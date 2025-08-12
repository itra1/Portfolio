using Core.Configs;
using Core.Configs.Consts;

namespace ScreenStreaming.Parameters
{
    public class ScreenStreamingConfig : IScreenStreamingParameters
    {
        private readonly IConfig _config;
        
        public bool IsEnabled => _config.TryGetValue(ConfigKey.StreamAutoStart, out var value) && value != "0";
        public string ServerUrl => _config.TryGetValue(ConfigKey.StreamServer, out var serverUrl) ? serverUrl : null;
        
        public ScreenStreamingConfig(IConfig config) => _config = config;
    }
}