using Core.Engine.Components.GameQuests;
using Core.Engine.Components.Timers;

using TMPro;

using UnityEngine;

using Zenject;

namespace Core.Engine.uGUI.Elements
{
	public class GameDecTimerPanel : MonoBehaviour, IZInjection
	{
		[SerializeField] private TMP_Text _timerLabel;

		private ITimersProvider _timerProvider;
		private ITimer _timer;
		private double _startTimer;
		private TimeLiveQuest _quest;

		[Inject]
		public void Initiate(ITimersProvider timerProvider){
			_timerProvider = timerProvider;
		}

		public void StartTimer(TimeLiveQuest quest)
		{
			_quest = quest;
			//	_startTimer = time;
			//_timer = _timerProvider.Create(TimerType.GameDesc)
			//.OnTick(OnTick)
			//.OnStop(OnStop)
			//.Start(_startTimer);
			//PrintTimer(_startTimer);
		}

		private void Update()
		{
			if(_quest != null){
				PrintTimer(_quest.SecondLeft);
			}
		}

		//private void OnTick(double val){
		//	PrintTimer(val);
		//}

		//private void OnStop()
		//{
		//	PrintTimer(0);
		//}

		private void PrintTimer(double value){
			_timerLabel.text = $"{((int)(value / 60)).ToString("00")}:{((int)(value % 60)).ToString("00")}";
		}

	}
}
