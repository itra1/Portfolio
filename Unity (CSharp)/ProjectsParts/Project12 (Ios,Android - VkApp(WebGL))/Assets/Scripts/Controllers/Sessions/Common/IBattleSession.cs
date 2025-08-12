using Engine.Scripts.Timelines;

namespace Game.Scripts.Controllers.Sessions.Common
{
	public interface IBattleSession
	{
		RhythmTimelineAsset SelectedSong { get; set; }
	}
}