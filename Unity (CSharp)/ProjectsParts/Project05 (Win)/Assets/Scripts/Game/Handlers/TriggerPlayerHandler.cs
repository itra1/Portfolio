using System.Collections;

using it.Game.Player;

using UnityEngine;
using UnityEngine.Events;

namespace it.Game.Handles
{
  public class TriggerPlayerHandler : MonoBehaviourBase, IPlayerTriggerEnter
  {
	 public UnityEvent onTriggerEnter = new UnityEvent();
	 public UnityEvent onTriggerExit = new UnityEvent();
	 public UnityEvent onTriggerStay = new UnityEvent();

	 /// <summary>
	 /// Плеер в григере
	 /// </summary>
	 public bool IsPlayer { get => _isPlayer; set => _isPlayer = value; }

	 private bool _isPlayer = false;

	 private void Start() { }

	 //private void OnTriggerEnter(Collider other)
	 //{
	 //if (!IsPlayer && other.GetComponent<Player.PlayerBehaviour>() != null)
	 //{
	 //  onTriggerEnter?.Invoke();
	 //  IsPlayer = true;
	 //}
	 //}

	 //private void OnTriggerExit(Collider other)
	 //{
	 //if (IsPlayer && other.GetComponent<Player.PlayerBehaviour>() != null)
	 //{
	 //  onTriggerExit?.Invoke();
	 //  IsPlayer = false;
	 //}
	 //}

	 IEnumerator UpdateCor()
	 {
		while (IsPlayer)
		{
		  onTriggerStay?.Invoke();
		  yield return new WaitForFixedUpdate();
		}
	 }

	 //private void OnTriggerStay(Collider other)
	 //{
		//if (IsPlayer && other.GetComponent<Player.PlayerBehaviour>() != null)
		//{
		//  onTriggerStay?.Invoke();
		//}
	 //}

	 public void OnPlayerTriggerEnter()
	 {
		onTriggerEnter?.Invoke();
		IsPlayer = true;
	 }

	 public void OnPlayerTriggerExit()
	 {
		onTriggerExit?.Invoke();
		IsPlayer = false;
	 }
  }
}