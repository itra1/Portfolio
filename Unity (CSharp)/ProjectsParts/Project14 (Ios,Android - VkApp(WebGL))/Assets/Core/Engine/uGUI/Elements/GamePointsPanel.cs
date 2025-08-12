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
	public class GamePointsPanel : MonoBehaviour, IZInjection
	{
		private TMP_Text _pointLabel;
		private SignalBus _signalBus;

		[Inject]
		public void Initiation(SignalBus signalBus)
		{
			_signalBus = signalBus;
		}

	}
}
