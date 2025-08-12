using System;
using System.Collections.Generic;
using System.Linq;
using LibVLCSharp;

namespace Elements.Common.Presenter.VlcEngine.Factory
{
    public class VlcMediaFactory : IVlcMediaFactory
    {
        private const string DShowPrefix = "dshow://";
        
        public Media Create(LibVLC library, string url)
        {
            Media media = null;
            
            if (url.StartsWith(DShowPrefix))
            {
                var parameters = new List<string>();
                
                FindParameters(parameters, url, ':');
                FindParameters(parameters, url, '-');
                
                var link = url.Split(' ').FirstOrDefault();
                
                if (!string.IsNullOrEmpty(link))
                    media = new Media(new Uri(link), parameters.ToArray());
            }
            
            media ??= new Media(new Uri(url));
            
            return media;
        }

        public void Destroy(ref Media media)
        {
            if (media == null)
                return;
            
            media.Dispose();
            media = null;
        }

        private void FindParameters(ICollection<string> parameters, string url, char separator)
        {
            if (parameters == null)
                return;
            
            var options = url.Split($" {separator}", StringSplitOptions.RemoveEmptyEntries);
            
            if (options.Length <= 1) 
                return;
            
            for (var i = 1; i < options.Length; i++)
            {
                var option = options[i];
                        
                if (!string.IsNullOrEmpty(option))
                    parameters.Add($"{separator}{option.Trim()}");
            }
        }
    }
}