using StringDrop;
using UnityEngine;

namespace Engine.Engine.Scripts.Settings.Common
{
	[System.Serializable]
	[CreateAssetMenu(fileName = "NoteGraphic", menuName = "Settings/Notes/NoteGraphic")]
	public class NoteGraphic : ScriptableObject
	{
		[SerializeField][StringDropList(typeof(NoteColorType), false)] private string _noteColorType;
		[SerializeField] private Color _noteColor;
		[SerializeField] private Gradient _noteHoldGradient;
		[SerializeField] private Sprite _clearNoteSprite;
		[SerializeField] private Sprite _tapNoteSprite;
		[SerializeField] private Sprite _swipeNoteSprite;
		[SerializeField] private Sprite _specialNoteSprite;
		[SerializeField] private Sprite _finalNoteSprite;
		[SerializeField] private Sprite _perfectNoteSprite;

		public string NoteColorType => _noteColorType;
		public Sprite ClearNoteSprite => _clearNoteSprite;
		public Sprite TapNoteSprite => _tapNoteSprite;
		public Sprite SwipeNoteSprite => _swipeNoteSprite;
		public Sprite SpecialNoteSprite => _specialNoteSprite;
		public Sprite FinalNoteSprite => _finalNoteSprite;
		public Sprite PerfectNoteSprite => _perfectNoteSprite;
		public Color NoteColor => _noteColor;
		public Gradient NoteHoldGradient => _noteHoldGradient;
	}
}
