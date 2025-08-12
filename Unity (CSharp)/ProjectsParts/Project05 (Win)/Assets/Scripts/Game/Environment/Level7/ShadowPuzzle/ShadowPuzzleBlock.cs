using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Game.Environment.Level8.ShadowPuzzle
{
  public class ShadowPuzzleBlock : MonoBehaviourBase
  {
	 public bool IsOpen => _isOpen;
	 [SerializeField]
	 private bool _isOpen = false;

	 public UnityEngine.Events.UnityAction<ShadowPuzzleBlock> OnTriggerEnterEvent;


	 private void OnEnable()
	 {
		var comp = GetComponent<Game.Handles.TriggerPlayerHandler>();
		if (comp == null)
		  comp = gameObject.AddComponent<Game.Handles.TriggerPlayerHandler>();

		if (comp.onTriggerEnter == null)
		{
		  comp.onTriggerEnter = new UnityEngine.Events.UnityEvent();
		}
		comp.onTriggerEnter.AddListener(() => { OnTriggerEnterEvent?.Invoke(this); });

	 }

#if UNITY_EDITOR
	 private void OnDrawGizmos()
	 {
		if (IsOpen)
		{
		  Gizmos.color = Color.green;
		  Gizmos.DrawWireSphere(transform.position, 0.2f);
		}
		else
		{
		  Gizmos.color = Color.red;
		  Gizmos.DrawWireSphere(transform.position, 0.2f);
		}
	 }
#endif

  }
}