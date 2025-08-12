using Game.Scripts.UI.Components.Interfaces;
using TMPro;
using UnityEngine;

namespace Game.Scripts.UI.Components
{
	public class SongMenuSeparator : MonoBehaviour, IMainListItem
	{
		[SerializeField] private TMP_Text _valueLabel;

		public void SetValue(int current, int max)
		{
			//_valueLabel.text = $"{current}/{max} <sprite=0>";
			_valueLabel.text = $"<color=#ffffff25><sprite=4 color=#ffffff25> Access from</color> <sprite=5> {current}/{max}";
		}
		public void SetValue(int max)
		{
			_valueLabel.text = $"{max} <sprite=0>";
		}
	}
}