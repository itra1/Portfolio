using System;
using Unity.RenderStreaming;

namespace ScreenStreaming.Sender.Source
{
    public interface IApplicationVideoStreamSource : IStreamRect, IDisposable
    {
        StreamSenderBase.WaitForCreateTrack CreateTrack();
    }
}