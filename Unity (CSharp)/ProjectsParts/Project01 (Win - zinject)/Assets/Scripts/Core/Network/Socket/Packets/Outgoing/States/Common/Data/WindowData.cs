namespace Core.Network.Socket.Packets.Outgoing.States.Common.Data
{
    public class WindowData
    {
        public string type;
        public string alias;
        public ulong material_id;
        public bool visibility;
        public bool fullscreen;
        public bool closed;
        public string tag_open;
        public string source_open;
        public bool in_focus;
        public VideoData video;
    }
}