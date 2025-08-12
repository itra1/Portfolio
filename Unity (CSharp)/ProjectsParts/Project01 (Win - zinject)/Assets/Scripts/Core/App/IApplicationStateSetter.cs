namespace Core.App
{
    public interface IApplicationStateSetter
    {
        bool IsScreenLocked { set; }
    }
}