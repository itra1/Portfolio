using UnityEngine;

namespace it.Game.Handles
{

  public class CheckToPlayerDistance : MonoBehaviourBase
  {

	 [SerializeField]
	 private float _distance = 50;

	 public bool InDistance { get; private set; }
	 private bool InDistanceTmp { get; set; }

	 public UnityEngine.Events.UnityEvent onPlayerIn;
	 public UnityEngine.Events.UnityEvent onPlayerOut;

	 [SerializeField]
	 public System.Action onPlayerInDistance;
	 [SerializeField]
	 public System.Action onPlayerOutDistance;
	 
	 private void Update()
	 {

		
		if (Game.Managers.GameManager.Instance.UserManager.PlayerBehaviour == null)
		{
		  InDistanceTmp = false;
		  return;
		}
		
		InDistanceTmp = (Game.Managers.GameManager.Instance.UserManager.PlayerBehaviour.transform.position - transform.position).sqrMagnitude <
							 (_distance * _distance);

		if (InDistanceTmp != InDistance)
		{
		  InDistance = InDistanceTmp;

		  if (InDistance)
		  {
			 onPlayerInDistance?.Invoke();
			 onPlayerIn?.Invoke();
		  }
		  else
		  {
			 onPlayerOutDistance?.Invoke();
			 onPlayerOut?.Invoke();
		  }

		}

	 }

	 private void OnDrawGizmosSelected()
	 {

		Gizmos.DrawWireSphere(transform.position, _distance);

	 }

  }
}
