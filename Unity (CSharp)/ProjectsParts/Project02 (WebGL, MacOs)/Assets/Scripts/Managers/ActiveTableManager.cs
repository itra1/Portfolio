using System.Collections;
using UnityEngine;
using it.Api;
using System.Collections.Generic;
using it.Network.Rest;
//using System.Threading.Tasks;

namespace Garilla.Managers
{
	public class ActiveTableManager
	{
		public static event UnityEngine.Events.UnityAction OnListChange;
		private List<TablePlayerSession> _activeSessions = new List<TablePlayerSession>();

		public List<TablePlayerSession> ActiveSessions { get => _activeSessions; set => _activeSessions = value; }

		public ActiveTableManager(MonoBehaviour parent){
			parent.StartCoroutine(Init());
		}


		private IEnumerator Init()
		{
			yield return new WaitForSeconds(0.2f);
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserLogin, OnUserLogin);
		}

		private void OnUserLogin(com.ootii.Messages.IMessage handler)
		{
			if (!UserController.IsLogin)
			{
				_activeSessions.Clear();
				OnListChange?.Invoke();
			}
			else
				LoadActiveTable();
		}

		public void LoadActiveTable()
		{
			it.Api.UserApi.GetUserActivity((result) =>
			{
				if (result.IsSuccess)
				{
					_activeSessions = result.Result;
					OnListChange?.Invoke();
				}
			});
		}

		public void TableListChange(){
			LoadActiveTable();
		}

	}
}