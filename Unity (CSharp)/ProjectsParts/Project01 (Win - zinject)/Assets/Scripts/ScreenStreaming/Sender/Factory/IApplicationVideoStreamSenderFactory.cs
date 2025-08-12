using UnityEngine;

namespace ScreenStreaming.Sender.Factory
{
    public interface IApplicationVideoStreamSenderFactory
    {
        IApplicationVideoStreamSender Create(RectTransform parent);
    }
}