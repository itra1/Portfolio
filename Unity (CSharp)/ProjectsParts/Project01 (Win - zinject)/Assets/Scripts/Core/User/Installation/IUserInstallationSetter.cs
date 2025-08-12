namespace Core.User.Installation
{
    public interface IUserInstallationSetter
    {
        ulong? DefaultDesktopId { set; }
        int? StatusColumnCount { set; }
    }
}