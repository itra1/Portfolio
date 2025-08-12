namespace Core.User.Installation
{
    public interface IUserInstallation
    {
        ulong? DefaultDesktopId { get; }
        int? StatusColumnCount { get; }
    }
}