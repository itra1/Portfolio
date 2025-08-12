using Game.Base;
using Game.Game.Signals;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Providers.Ui.Elements
{
	public class SettingsButtonPanel : MonoBehaviour, IInjection
	{
		[SerializeField] private Button _addButton;

		[Inject] protected SignalBus _signalBus;

		public void Awake()
		{
			_addButton.onClick.AddListener(AddButtonTouch);
		}

		private void AddButtonTouch()
		{
			_signalBus.Fire<OpenSettingsSignal>();
		}
	}
}
