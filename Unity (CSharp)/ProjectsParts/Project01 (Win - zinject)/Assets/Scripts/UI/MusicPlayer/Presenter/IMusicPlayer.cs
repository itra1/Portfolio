namespace UI.MusicPlayer.Presenter
{
    public interface IMusicPlayer : IMusicPlayerState
    {
        ulong CurrentTrackId { get; }
        int Time { get; }
        int TimeChanged { get; }
        
        void ToggleLoop();
        void ResetTrack();
        void TogglePlay();
        void SetTime(int time);
        void SetNextTrack();
        void SetPreviousTrack();
        void SetTrack(ulong trackId);
    }
}