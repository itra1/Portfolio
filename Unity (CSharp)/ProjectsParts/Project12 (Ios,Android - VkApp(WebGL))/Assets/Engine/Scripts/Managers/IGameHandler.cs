using Cysharp.Threading.Tasks;
using Engine.Scripts.Timelines;
using UnityEngine.Events;

namespace Engine.Scripts.Managers
{
	public interface IGameHandler
	{
		/// <summary>
		/// событие запуска/остановки игрового процесса
		/// </summary>
		UnityEvent<RhythmTimelineAsset, bool> OnGameChangeEvent { get; set; }
		UniTask RestartSong();
		void EndSong();
		UniTask PlaySong(RhythmTimelineAsset song);
	}
}
