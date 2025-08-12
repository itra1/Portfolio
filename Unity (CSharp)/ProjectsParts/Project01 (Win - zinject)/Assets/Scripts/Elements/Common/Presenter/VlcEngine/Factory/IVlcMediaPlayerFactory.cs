using System;
using LibVLCSharp;

namespace Elements.Common.Presenter.VlcEngine.Factory
{
    public interface IVlcMediaPlayerFactory
    {
        MediaPlayer Create(LibVLC library, EventHandler<EventArgs> onPaying = null, EventHandler<EventArgs> onStopped = null);
        
        void Destroy(ref MediaPlayer mediaPlayer, EventHandler<EventArgs> onPaying = null, EventHandler<EventArgs> onStopped = null);
    }
}