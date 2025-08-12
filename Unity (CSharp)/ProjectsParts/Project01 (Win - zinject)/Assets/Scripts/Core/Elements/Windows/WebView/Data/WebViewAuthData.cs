using System;
using Core.Materials.Attributes;

namespace Core.Elements.Windows.WebView.Data
{
    [Serializable]
    public class WebViewAuthData
    {
        [MaterialDataPropertyParse("id"), MaterialDataPropertyUpdate]
        public string Id { get; set; }
		
        [MaterialDataPropertyParse("authUrl"), MaterialDataPropertyUpdate]
        public string AuthUrl { get; set; }
        
        [MaterialDataPropertyParse("maxAuthAttempts"), MaterialDataPropertyUpdate]
        public int MaxAuthAttempts { get; set; }
		
        [MaterialDataPropertyParse("authType"), MaterialDataPropertyUpdate]
        public string AuthType { get; set; }
		
        [MaterialDataPropertyParse("username"), MaterialDataPropertyUpdate]
        public string Username { get; set; }
		
        [MaterialDataPropertyParse("password"), MaterialDataPropertyUpdate]
        public string Password { get; set; }
		
        [MaterialDataPropertyParse("status"), MaterialDataPropertyUpdate]
        public string Status { get; set; }
		
        [MaterialDataPropertyParse("authScript"), MaterialDataPropertyUpdate]
        public string AuthScript { get; set; }
    }
}