using System;
using Core.Logging;
using Core.User.Installation.Parsing.Consts;
using Leguar.TotalJSON;

namespace Core.User.Installation.Parsing
{
    public class UserInstallationParser : IUserInstallationParser
    {
        private readonly IUserInstallationSetter _installation;
        
        public UserInstallationParser(IUserInstallationSetter installation) => _installation = installation;
        
        public void Parse(string rawData)
        {
            var json = JSON.ParseString(rawData);
            
            if (TryGetValueFrom(json, UserInstallationPropertyName.DefaultDesktopId, out ulong defaultDesktopId))
                _installation.DefaultDesktopId = defaultDesktopId;
            else
                Debug.LogError("Default desktop id is missing");
            
            if (TryGetValueFrom(json, UserInstallationPropertyName.StatusColumnCount, out int statusColumnCount))
                _installation.StatusColumnCount = statusColumnCount;
            else
                Debug.LogError("Status column count is missing");
        }
        
        private bool TryGetValueFrom(JSON json, string propertyName, out ulong value) => 
            TryGetValueFrom(json, propertyName, json.GetULong, out value);
        
        private bool TryGetValueFrom(JSON json, string propertyName, out int value) => 
            TryGetValueFrom(json, propertyName, json.GetInt, out value);
        
        private bool TryGetValueFrom<TValue>(JSON json,
            string propertyName,
            Func<string, TValue> jsonValueGetter,
            out TValue value)
        {
            if (!json.ContainsKey(propertyName) || json.Get(propertyName) is null or JNull)
            {
                value = default;
                return false;
            }
            
            value = jsonValueGetter(propertyName);
            return true;
        }
    }
}