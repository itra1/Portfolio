namespace Core.Network.Socket.Consts
{
    public class PacketLogMessage
    {
        public const string FullPacketParsingErrorFormat = "Packet parsing error with type \"{0}\": {1}{2}{3}";
        public const string ShortPacketParsingErrorFormat = "Packet parsing error with type: {0}{1}{2}";
        public const string PacketReceivedFormat = "Packet received: {0}";
        public const string PacketIgnoredFormat = "Packet ignored: {0}";
        public const string PacketIgnored = "Packet ignored";
        public const string PacketSentFormat = "Packet sent: {0}";
    }
}