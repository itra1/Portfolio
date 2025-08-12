using Game.Providers.Timers.Common;
using Game.Providers.Timers.Helpers;
using TMPro;
using UnityEngine;

namespace Game.Providers.Ui.Elements {
	public class TimerView :MonoBehaviour {
		[SerializeField] private string _printFormst;
		[SerializeField] private TMP_Text _label;

		private ITimer _timer;

		public void SetTimer(ITimer timer) {
			_timer = timer;

			_timer?.OnTick((time) => {
				_label.text = time.AsRealTimeString();
			});
		}
	}
}
