using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Sett = it.Settings;

namespace it.UI
{
	public class SignalButton : MonoBehaviour
	{
		public UnityAction OnPingTick;
		[SerializeField] private bool enablePing;
		[SerializeField] private int pingTime;
		[SerializeField] SignalStatusView statusView;
//		public void Start()
//		{
//#if UNITY_WEBGL
//			ping(Sett.Settings.Servers.Server);
//#else
//			SetStatusViewPing();
//			StartCoroutine(CheckPing());
//#endif
//		}
		/// <summary>
		/// Вызывается из интерфейса
		/// </summary>
		public void Open()
		{
			it.Main.PopupController.Instance.ShowPopup(PopupType.Network);
		}

		//IEnumerator CheckPing()
		//{
		//	yield return new WaitForSeconds(pingTime);
		//	SetStatusViewPing();
		//	OnPingTick?.Invoke();
		//	if (enablePing)
		//		StartCoroutine(CheckPing());
		//}
		private void OnDestroy()
		{
			OnPingTick = null;
			StopAllCoroutines();
		}
	}
}