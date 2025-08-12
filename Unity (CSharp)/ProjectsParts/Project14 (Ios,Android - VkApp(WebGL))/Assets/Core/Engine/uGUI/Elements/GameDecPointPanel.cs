using Core.Engine.Components.Timers;
using Core.Engine.Signals;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMPro;

using UnityEngine;

using Zenject;

namespace Core.Engine.uGUI.Elements
{
	public class GameDecPointPanel : MonoBehaviour, IZInjection
	{
		[SerializeField] private TMP_Text _pointLabel;

		private SignalBus _signalbus;
		private float _startPoint;
		private float _currentValue;

		[Inject]
		public void Initiate(SignalBus signalBus)
		{
			_signalbus = signalBus;

			_signalbus.Subscribe<GamePointChangeSignal>(OnGamePointChangeSignal);
		}

		private void OnGamePointChangeSignal(GamePointChangeSignal signal)
		{
			_currentValue = signal.Value;
			PrintValue((int)(_startPoint - _currentValue));
		}

		public void StartValue(int value)
		{
			_startPoint = value;
			_currentValue = 0;
			PrintValue((int)(_startPoint - _currentValue));
		}

		private void PrintValue(int value)
		{
			_pointLabel.text = Math.Max(value,0).ToString();
		}

	}
}
