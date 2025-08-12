using UnityEngine;
using System.Collections;
using UnityEditor;
using Utilites.Geometry;

namespace it.Game.Items
{
  public class InfectedCrystal : HoldenItem
  {
	 public override int HoldStey => 2;

	 private it.Game.Environment.Level4.Obelisc _obelisc;

	 private void Update()
	 {
		if (!_isUse)
		  return;

		RaycastHit[] _hits;
		int count = RaycastExt.SafeSphereCastAll(transform.position + Vector3.up, transform.forward, 3, out _hits, 0.1f, -1, transform);
		if(count > 0)
		{
		  bool existsObelisk = false;
		  for(int i = 0; i < count; i++)
		  {
			 var obel = _hits[i].collider.GetComponentInParent<it.Game.Environment.Level4.Obelisc>();
			 if (obel != null)
				existsObelisk = true;
			 if (obel != null && _obelisc == null)
			 {
				_obelisc = obel;
				_obelisc.PlayerConnect(transform);
			 }
		  }
		  if(!existsObelisk && _obelisc != null)
		  {
			 _obelisc.PlayerDisconnect();
			 _obelisc = null;
		  }
		}

	 }

	 public override void UnUse()
	 {
		base.UnUse();
		if(_obelisc != null)
		{
		  _obelisc.PlayerDisconnect();
		  _obelisc = null;
		}
	 }

  }
}