namespace Core.Configs
{
    public interface IVlcConfig
    {
        bool IsLoaded { get; }
        string[] Parameters { get; }
    }
}