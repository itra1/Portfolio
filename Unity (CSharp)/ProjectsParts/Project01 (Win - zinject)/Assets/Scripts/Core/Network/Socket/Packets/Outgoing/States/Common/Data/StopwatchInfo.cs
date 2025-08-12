using System.Collections.Generic;

namespace Core.Network.Socket.Packets.Outgoing.States.Common.Data
{
    public class StopwatchInfo
    {
        public bool display;
        public bool active;
        public bool paused;
        public long time_start;
        public long? time_paused;
        public long total_time;
        public List<long> laps;
        public float x;
        public float y;
        public string color;
    }
}