using Core.Configs;
using Core.Configs.Consts;

namespace Core.Options.Offsets
{
    public class ScreenOffsets : IScreenOffsets, IScreenOffsetsSetter
    {
        private readonly IConfig _config;
        private readonly IApplicationOptions _options;

        public float Left { get; private set; }
        public float Right { get; private set; }
        public float Top { get; private set; }
        public float Bottom { get; private set; }
        
        public ScreenOffsets(IConfig config, IApplicationOptions options)
        {
            _config = config;
            _options = options;
        }
        
        public void Apply()
        {
            Left = _options.ScreenBorderLeft ?? GetOffsetValue(ConfigKey.ScreenBorderLeft);
            Right = _options.ScreenBorderRight ?? GetOffsetValue(ConfigKey.ScreenBorderRight);
            Top = _options.ScreenBorderUp ?? GetOffsetValue(ConfigKey.ScreenBorderUp);
            Bottom = _options.ScreenBorderDown ?? GetOffsetValue(ConfigKey.ScreenBorderDown);
        }
        
        private float GetOffsetValue(string key)
        {
            if (!_config.TryGetValue(key, out var rawValue))
                return 0;
			
            if (!float.TryParse(rawValue, out var value))
                return 0;
			
            return value;
        }
    }
}