using System.Collections;
using UnityEngine;
using it.Game.NPC.Enemyes;
using it.Game.Handles;

namespace it.Game.Environment.Level5.Leech
{
  public class LeechLaser : MonoBehaviour
  {
	 [SerializeField] private GameObject _sphere;
	 [SerializeField] private TriggerHandler _triggerHandler;
	 private bool _isActive = false;

	 private void Awake()
	 {
		_triggerHandler.onTriggerEnter.AddListener(OnEnterLeech);
	 }

	 private void OnDestroy()
	 {
		_triggerHandler.onTriggerEnter.RemoveAllListeners();
	 }

	 public void SetActivate()
	 {
		_isActive = true;
	 }

	 public void SetDeactivate()
	 {
		_isActive = false;
	 }

	 public void OnEnterLeech(Collider col)
	 {
		var lch = col.GetComponent<it.Game.NPC.Enemyes.Leech>();

		if(lch != null)
		{
		  lch.LaserHit();
		}

	 }

  }
}