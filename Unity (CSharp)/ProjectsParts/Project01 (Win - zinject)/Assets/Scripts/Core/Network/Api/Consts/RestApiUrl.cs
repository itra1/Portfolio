namespace Core.Network.Api.Consts
{
    public static class RestApiUrl
    {
        public const string Login = "/auth/login";
        public const string LockScreen = "/state/lock-screen";
        public const string CurrentScreenFormat = "/state/current-screen/{0}";
        public const string UserInstallation = "/installations/me";
        public const string LoadedStatuses = "/state/status/loaded";
        public const string ActiveStatusFormat = "/state/status/active/{0}";
        public const string StatusMaterialByColumnFormat = "/state/status/material/{0}/{1}";
        public const string ActiveStatusContentMaterialFormat = "/status-content/mobile/active/{0}";
        public const string MaterialStateFormat = "/materials/state/{0}";
        public const string Weather = "/weather";
        public const string PreviewFormat = "/preview/{0}";
    }
}