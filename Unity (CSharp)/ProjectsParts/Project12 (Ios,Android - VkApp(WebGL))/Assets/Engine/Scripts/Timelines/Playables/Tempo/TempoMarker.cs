using UnityEngine;
using UnityEngine.Timeline;

namespace Engine.Scripts.Timelines.Playables.Tempo
{
	[ExcludeFromPreset]
	[HideInMenu]
	public class TempoMarker : Marker
	{
		[Tooltip("The Tempro Marker ID, used to define on/off beat (and potentially others).")]
		[SerializeField] protected int m_ID;

		public int ID
		{
			get { return m_ID; }
			set { m_ID = value; }
		}

		public virtual void Copy(TempoMarker clonedFrom)
		{
			if (clonedFrom == null)
			{ return; }
			m_ID = clonedFrom.m_ID;
			return;
		}
	}
}