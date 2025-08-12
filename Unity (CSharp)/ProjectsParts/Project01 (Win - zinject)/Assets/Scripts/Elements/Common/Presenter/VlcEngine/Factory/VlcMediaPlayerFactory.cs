using System;
using LibVLCSharp;

namespace Elements.Common.Presenter.VlcEngine.Factory
{
    public class VlcMediaPlayerFactory : IVlcMediaPlayerFactory
    {
        public MediaPlayer Create(LibVLC library, EventHandler<EventArgs> onPaying = null, EventHandler<EventArgs> onStopped = null)
        {
            var mediaPlayer = new MediaPlayer(library);
            
            mediaPlayer.EnableHardwareDecoding = true;
            
            if (onPaying != null)
                mediaPlayer.Playing += onPaying;

            if (onStopped != null)
                mediaPlayer.Stopped += onStopped;
            
            return mediaPlayer;
        }

        public void Destroy(ref MediaPlayer mediaPlayer, EventHandler<EventArgs> onPaying = null, EventHandler<EventArgs> onStopped = null)
        {
            if (mediaPlayer == null)
                return;
            
            if (onPaying != null)
                mediaPlayer.Playing -= onPaying;

            if (onStopped != null)
                mediaPlayer.Stopped -= onStopped;

            if (mediaPlayer.IsPlaying)
                mediaPlayer.Stop();
            
            mediaPlayer.Dispose();
            mediaPlayer = null;
        }
    }
}