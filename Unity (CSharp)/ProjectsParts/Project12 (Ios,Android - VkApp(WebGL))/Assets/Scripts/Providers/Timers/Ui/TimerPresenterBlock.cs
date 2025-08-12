using UnityEngine;

namespace Game.Scripts.Providers.Timers.Ui
{
	[System.Serializable]
	public class TimerPresenterBlock
	{
		[SerializeField] private TimerPresenterItem[] _items;

		public TimerPresenterItem[] Items => _items;
	}
}
