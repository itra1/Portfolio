
using Providers.SystemMessage.Common;
using Providers.SystemMessage.Components;

using UnityEngine;

namespace Providers.SystemMessage.Presenters
{
	public class SystemMessagePresenter : MonoBehaviour, ISystemMessageVisible
	{
		[SerializeField] private SystemMessagePanel _prefab;

		private void Awake()
		{
			_prefab.gameObject.SetActive(false);
		}

		public void SetMessage(string message)
		{
			_prefab.SetMessage(message);
		}

	}
}
