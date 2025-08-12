using Engine.Scripts.Timelines.Notes.Base;
using Engine.Scripts.Timelines.Notes.Settings;
using Engine.Scripts.Timelines.Playables;
using StringDrop;
using UnityEngine;
using UnityEngine.Timeline;

namespace Engine.Scripts.Timelines.Notes.Common
{
	[System.Serializable]
	[CreateAssetMenu(fileName = "My Note Definition", menuName = "Dypsloom/Rhythm Timeline/Note Definition", order = 1)]
	public class NoteDefinition : ScriptableObject
	{
		public enum ClipDurationType
		{
			Free,
			Crochet,
			HalfCrochet,
			QuarterCrochet,
			ScaledCrochet,
		}

		[SerializeField] protected ClipDurationType m_ClipDuration;
		[SerializeField] protected float m_ScaledCrochetValue = 1.2f;
		[StringDropList(typeof(NoteType))][SerializeField] private string _noteType;

		public ClipDurationType ClipDuration => m_ClipDuration;
		public string NoteType => _noteType;

#if UNITY_EDITOR
		[Tooltip("The settings used to customize the clip in the editor.")]
		[SerializeField] protected RhythmClipEditorSettings m_RhythmClipEditorSettings;
		public RhythmClipEditorSettings RhythmClipEditorSettings => m_RhythmClipEditorSettings;
#endif

		public virtual void SetClipDuration(RhythmClip rhythmClip, TimelineClip clip)
		{
			if (m_ClipDuration == ClipDurationType.Free)
				return;

			if (m_ClipDuration == ClipDurationType.HalfCrochet)
			{
				clip.duration = rhythmClip.RhythmClipData.RhythmDirector.HalfCrochet;
				return;
			}

			if (m_ClipDuration == ClipDurationType.Crochet)
			{
				clip.duration = rhythmClip.RhythmClipData.RhythmDirector.Crochet;
				return;
			}

			if (m_ClipDuration == ClipDurationType.QuarterCrochet)
			{
				clip.duration = rhythmClip.RhythmClipData.RhythmDirector.QuarterCrochet;
				return;
			}

			if (m_ClipDuration == ClipDurationType.ScaledCrochet)
			{
				clip.duration = rhythmClip.RhythmClipData.RhythmDirector.Crochet * m_ScaledCrochetValue;
				return;
			}
		}
	}
}