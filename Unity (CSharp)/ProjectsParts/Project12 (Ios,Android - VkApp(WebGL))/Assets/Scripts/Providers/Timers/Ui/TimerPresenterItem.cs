using TMPro;
using UnityEngine;

namespace Game.Scripts.Providers.Timers.Ui
{
	[System.Serializable]
	public class TimerPresenterItem
	{
		[SerializeField] TMP_Text _valueLabel;
		[SerializeField] TMP_Text _titleLabel;

		public TMP_Text ValueLabel => _valueLabel;
		public TMP_Text TitleLabel => _titleLabel;
	}
}
