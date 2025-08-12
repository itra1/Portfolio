using System.Collections;
using UnityEngine;
using TMPro;

namespace Garilla.Games
{
	public class PlayerGameAfk : MonoBehaviour
	{
		[SerializeField] TextMeshProUGUI _valueLabel;
		private TimerManager.RealTimer _timer;

		public void SetAfk(TimerManager.RealTimer timer){
			_timer = timer;
			_timer.OnUpdate.AddListener(() =>
			{
				ConfirmTimer();

			});
			_timer.OnComplete.AddListener((res) =>
			{
				gameObject.SetActive(false);
			});

			if (_timer.IsActive)
				gameObject.SetActive(true);

			ConfirmTimer();
		}

		public void StopTimer(){
			_timer = null;
			gameObject.SetActive(false);
		}

		private void ConfirmTimer(){
			_valueLabel.text = ((int)_timer.TimeLeftDouble).ToString();
		}

	}
}