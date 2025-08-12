using TMPro;
using UnityEngine;

namespace Game.Scripts.Helpers
{
	public class ConsoleLog : MonoBehaviour
	{
		[SerializeField] private TMP_Text _logLabel;

		private void Awake()
		{
			AppLog.OnLog += (message) =>
			{
				_logLabel.text = message + '\n' + _logLabel.text;
			};
		}
	}
}
