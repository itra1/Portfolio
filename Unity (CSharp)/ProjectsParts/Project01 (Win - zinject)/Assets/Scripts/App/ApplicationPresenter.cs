using com.ootii.Messages;
using UnityEngine;

namespace App
{
	/// <summary>
	/// Устаревшее название - "GameManager"
	/// </summary>
	public class ApplicationPresenter : MonoBehaviour, IApplicationPresenter
	{
		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
			gameObject.AddComponent<MessageDispatcherStub>();
		}
		
		private void OnDestroy()
		{
			if (gameObject.TryGetComponent<MessageDispatcherStub>(out var component))
				Destroy(component);
		}
	}
}