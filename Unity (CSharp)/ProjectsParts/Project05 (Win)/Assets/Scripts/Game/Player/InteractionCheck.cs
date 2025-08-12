using System.Collections.Generic;
using UnityEngine;
using it.Game.Items;

namespace it.Game.Player
{
  public class InteractionCheck : PlayerComponent, Game.Player.IPlayerControl
  {

	 [System.NonSerialized]
	 private float _radiusCheck = 1f;
	 private float _minHeight = 0f;
	 private float _maxHeight = 1.7f;
	 private float _forvardDistance = 1f;

	 private float _usageAngle = 20;

	 private Game.Items.IInteraction _active = null;

	 public IInteraction ActiveUsage
	 {
		get => _active;
	 }

	 public bool IsReadyUse
	 {
		get
		{
		  return _active != null && _active.IsInteractReady;
		}
	 }

	 private List<IInteraction> _usageList = new List<IInteraction>();

	 private void FixedUpdate()
	 {
		//RaycastHit[] hits = Physics.SphereCastAll(transform.position + (Vector3.up * _minHeight) + transform.forward * -1f, _radiusCheck, transform.forward, _forvardDistance); ;
		//RaycastHit[] hits = Physics.CapsuleCastAll(transform.position + (Vector3.up * _minHeight),
		//  transform.position + (Vector3.up * 1.7f),
		//  _radiusCheck, transform.forward, _forvardDistance);
		RaycastHit[] hits = Physics.CapsuleCastAll(transform.position + (Vector3.up * _minHeight) + transform.forward * -1f,
		  transform.position + (Vector3.up * 1.7f) + transform.forward * -1f,
		  _radiusCheck, transform.forward, _forvardDistance);

		if (hits.Length <= 0)
		{
		  _active = null;
		  return;
		}

		IInteraction tmp = null;
		float angle = 360;
		for (int i = 0; i < hits.Length; i++)
		{
		  IInteraction uo = hits[i].collider.GetComponent<IInteraction>();
		  if (uo == null || !uo.IsInteractReady)
			 continue;

		  Vector3 direct = hits[i].point - transform.position;
		  Vector3 forvardDirection = direct - Vector3.Project(direct, transform.up);
		  float lYawAngle = Vector3.Angle(transform.forward, forvardDirection);

		  if (lYawAngle > angle && lYawAngle > _usageAngle)
			 continue;

		  Vector3 posStart = transform.position;

		  //RaycastHit[] subHits = Physics.RaycastAll(posStart, hits[i].transform.position, _forvardDistance + _radiusCheck);

		  //for (int s = 0; s < subHits.Length; s++)
		  //{


		  //IUseItem uoSub = subHits[i].collider.GetComponent<IUseItem>();
		  //if (uoSub == null || !uoSub.IsUseReady)
		  //continue;

		  //Vector3 dist = hits[i].point

		  //}
		  //{

		  if (forvardDirection.magnitude > _forvardDistance + _radiusCheck)
		  {
			 //Debug.Log(hits[i].collider.name + " : " + forvardDirection.magnitude);
			 //Debug.Log(_forvardDistance + _radiusCheck);
			 continue;
		  }

		  angle = lYawAngle;
		  tmp = uo;

		}

		if (tmp != null && !tmp.Equals(_active))
		{
		  _active = tmp;
		}
		else if (tmp == null)
		{
		  _active = null;
		}
	 }

	 private void OnDrawGizmosSelected()
	 {
		if (_active != null)
		{
		  Gizmos.DrawLine(transform.position, (_active as MonoBehaviour).transform.position);
		}
	 }

	 private void OnEnterTrigger(Collider colContact)
	 {

		IInteraction col = colContact.gameObject.GetComponent<IInteraction>();

		if (col != null)
		{
		  _usageList.Add(col);
		  _active = col;
		}

	 }
	 private void OnExitTrigger(Collider colContact)
	 {

		IInteraction col = colContact.gameObject.GetComponent<IInteraction>();

		if (col != null)
		{
		  _usageList.Remove(col);
		}
	 }

	 private void Update()
	 {
		//if (Game.Managers.GameManager.Instance.GameInputSource.IsJustPressed("Use"))
		//{
		//  OnUse();
		//}
	 }

	 public void OnButtonDown(string name)
	 {
		if (name.Equals("Use"))
		  OnUse();
	 }

	 private void OnUse()
	 {
		if (_active == null)
		  return;

		if (_active.IsInteractReady)
		  _active.StartInteract();
	 }

  }
}