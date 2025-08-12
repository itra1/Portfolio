using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.States;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
    [SocketAction("soundVolume_level_up")]
    [SocketAction("soundVolume_level_down")]
    [SocketAction("soundVolume_mute_toggle")]
    public class AudioControlSet : IncomingAction
    {
        public override bool Parse() => true;
        
        public override bool Process()
        {
            if (!base.Process())
                return false;
            
            var messageType = Alias switch
            {
                "soundVolume_level_up" => MessageType.AudioVolumeUp,
                "soundVolume_level_down" => MessageType.AudioVolumeDown,
                "soundVolume_mute_toggle" => MessageType.AudioMuteToggle,
                _ => null
            };

            if (messageType != null) 
                MessageDispatcher.SendMessage(messageType);
            
            return true;
        }
    }
}