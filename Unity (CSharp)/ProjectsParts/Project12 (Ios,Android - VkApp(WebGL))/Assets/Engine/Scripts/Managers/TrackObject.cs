using System.Collections.Generic;
using Engine.Scripts.Timelines.Notes.Base;
using UnityEngine;
using UnityEngine.Events;

namespace Engine.Scripts.Managers
{
	public class TrackObject : MonoBehaviour
	{
		[Tooltip("The event when at least one note was activated.")]
		[SerializeField] protected UnityEvent m_OnNoteActivate;
		[Tooltip("The event when no notes are active on the track anymore.")]
		[SerializeField] protected UnityEvent m_OnNoteDeactivate;

		[Tooltip("The start point where the notes should spawn.")]
		[SerializeField] protected Transform m_StartPoint;
		[Tooltip("The end point where the notes should be when they are perfect for the beat.")]
		[SerializeField] protected Transform m_EndPoint;
		[Tooltip("The 3D touch collider.")]
		[SerializeField] protected Collider m_TouchCollider3D;
		[Tooltip("The 2D touch collider.")]
		[SerializeField] protected Collider2D m_TouchCollider2D;
		[SerializeField] private SpriteRenderer _road;

		public Transform StartPoint => m_StartPoint;
		public Transform EndPoint => m_EndPoint;

		protected List<Note> m_Notes;
		protected List<Note> m_NotesVisible;

		public Note CurrentNote => m_Notes == null || m_Notes.Count == 0 ? null : m_Notes[0];
		public Note CurrentVisibleNote => m_NotesVisible == null || m_NotesVisible.Count == 0 ? null : m_NotesVisible[0];
		public Collider2D TouchCollider2D => m_TouchCollider2D;
		public Collider TouchCollider3D => m_TouchCollider3D;

		public IReadOnlyList<Note> Notes => m_Notes;
		public IReadOnlyList<Note> NotesVisible => m_NotesVisible;

		protected virtual void Awake()
		{
			ClearNotes();
		}

		public virtual void ClearNotes()
		{
			if (m_Notes == null)
			{
				m_Notes = new List<Note>();
			}
			else
			{
				m_Notes.Clear();
			}
		}

		public void SetVisibleNote(Note note)
		{
			if (m_NotesVisible == null)
			{ m_NotesVisible = new List<Note>(); }
			m_NotesVisible.Add(note);
		}

		public void RemoveVisibleNote(Note note)
		{
			if (m_NotesVisible == null || !m_NotesVisible.Contains(note))
				return;

			_ = m_NotesVisible.Remove(note);
		}

		public void SetActiveNote(Note note)
		{
			if (m_Notes == null)
				m_Notes = new List<Note>();
			m_Notes.Add(note);

			if (m_Notes.Count == 1)
			{
				m_OnNoteActivate.Invoke();
			}
		}

		public void RemoveActiveNote(Note note)
		{
			if (m_Notes == null || !m_Notes.Contains(note))
				return;

			_ = m_Notes.Remove(note);

			if (m_Notes.Count == 0)
			{
				m_OnNoteDeactivate.Invoke();
			}
		}

		/// <summary>
		/// Get the note direction at a specific normalized time 0->end, 1->start.
		/// </summary>
		/// <param name="t">The normalized time.</param>
		/// <returns></returns>
		public virtual Vector3 GetNoteDirection(float t)
		{
			return (EndPoint.position - StartPoint.position).normalized;
		}
	}
}