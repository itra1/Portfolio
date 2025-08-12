namespace UI.MusicPlayer.Presenter
{
    public interface IMusicPlayerState
    {
        bool IsPlaying { get; }
        bool IsLooping { get; }
    }
}