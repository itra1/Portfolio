namespace Core.User.Installation
{
    public class UserInstallation : IUserInstallation, IUserInstallationSetter
    {
        public ulong? DefaultDesktopId { get; set; }
        public int? StatusColumnCount { get; set; }
    }
}