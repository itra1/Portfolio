using UnityEngine;

namespace Settings.Themes
{
	[CreateAssetMenu(fileName = "Theme", menuName = "Settings/Theme")]
	public class Theme : ScriptableObject, ITheme
	{
		[SerializeField] private Color _standartColor;
		[SerializeField] private Color _focusColor;
		[SerializeField] private Color _errorColor;
		[SerializeField] private Color _placeholderColor;

		public Color StandartColor => _standartColor;
		public Color FocusColor => _focusColor;
		public Color ErrorColor => _errorColor;
		public Color PlaceholderColor => _placeholderColor;
	}
}
