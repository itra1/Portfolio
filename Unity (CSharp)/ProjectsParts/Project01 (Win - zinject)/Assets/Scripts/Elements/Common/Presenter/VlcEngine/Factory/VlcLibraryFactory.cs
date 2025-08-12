using System.Linq;
using System.Threading.Tasks;
using Core.Configs;
using Core.Configs.Consts;
using Core.Options;
using LibVLCSharp;
using UnityEngine;

namespace Elements.Common.Presenter.VlcEngine.Factory
{
    public class VlcLibraryFactory : IVlcLibraryFactory
    {
        private readonly IConfig _config;
        private readonly IVlcConfig _vlcConfig;
        private readonly IApplicationOptions _options;
        
        public VlcLibraryFactory(IConfig config, IVlcConfig vlcConfig, IApplicationOptions options)
        {
            _config = config;
            _vlcConfig = vlcConfig;
            _options = options;
        }
        
        public LibVLC Create()
        {
            var parameters = _vlcConfig.Parameters.ToList();
            
            if (_options.IsDevServerEnabled && _config.TryGetValue(ConfigKey.Proxy, out var value))
                parameters.Add($"--http-proxy=\"{value}\"");
            
            LibVLCSharp.Core.Initialize(Application.dataPath);
            
            var library = new LibVLC(parameters.ToArray());
            
            library.SetDialogHandlers(
                (_, _, _, _, _, _) => Task.CompletedTask,
                (dialog, _, _, _, _, _, _, _) =>
                {
                    dialog.PostAction(1);
                    return Task.CompletedTask;
                },
                (_, _, _, _, _, _, _) => Task.CompletedTask,
                (_, _, _) => Task.CompletedTask);

            return library;
        }
        
        public void Destroy(ref LibVLC library)
        {
            if (library == null)
                return;
            
            library.UnsetDialogHandlers();
            library.Dispose();
            library = null;
        }
    }
}