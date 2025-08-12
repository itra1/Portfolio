namespace Environment.Microsoft.Windows.Apps.Office.Server.IO
{
    public interface IStringStream
    {
        public bool CanRead { get; }
        string Read();
        void Write(string value);
    }
}