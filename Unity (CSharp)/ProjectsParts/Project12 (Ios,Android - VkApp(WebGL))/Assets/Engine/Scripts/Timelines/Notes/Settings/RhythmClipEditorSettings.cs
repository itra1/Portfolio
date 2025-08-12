using System;
using UnityEngine;

namespace Engine.Scripts.Timelines.Notes.Settings
{
	[Serializable]
	public class RhythmClipEditorSettings
	{
		[Tooltip("The left texture.")]
		[SerializeField] private Texture m_Left;
		[Tooltip("The center texture.")]
		[SerializeField] private Texture m_Center;
		[Tooltip("The right texture.")]
		[SerializeField] private Texture m_Right;
		[SerializeField] private Texture _specialPoint;
		[SerializeField] private Texture _endPoint;
		[Tooltip("The clip color.")]
		[SerializeField] private Color m_Color = Color.clear;

		public Texture Left
		{
			get => m_Left;
			set => m_Left = value;
		}

		public Texture Center
		{
			get => m_Center;
			set => m_Center = value;
		}

		public Texture Right
		{
			get => m_Right;
			set => m_Right = value;
		}

		public Color Color
		{
			get => m_Color;
			set => m_Color = value;
		}
		public Texture SpecialPoint => _specialPoint;
		public Texture EndPoint => _endPoint;
	}
}