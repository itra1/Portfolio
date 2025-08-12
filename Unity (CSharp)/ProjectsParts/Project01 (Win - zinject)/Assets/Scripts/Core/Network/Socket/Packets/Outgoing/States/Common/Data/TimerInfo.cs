namespace Core.Network.Socket.Packets.Outgoing.States.Common.Data
{
    public class TimerInfo
    {
        public bool paused;
        public bool alarm;
        public bool active;
        public bool display;
        public long timeEnd;
        public long residue;
        public float x;
        public float y;
        public string color;
    }
}