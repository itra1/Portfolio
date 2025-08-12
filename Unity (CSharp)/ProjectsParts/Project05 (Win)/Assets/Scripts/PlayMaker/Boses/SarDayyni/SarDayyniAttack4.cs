using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using DG.Tweening;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;
using it.Game.Player;
using com.ootii.Geometry;

namespace it.Game.PlayMaker.Boses.SarDayyni
{
  [ActionCategory("Enemyes")]
  [Tooltip("Первая создает в метре перед лицом персонажа портал который телепортирует его в случайное место в комнате(в воздух то же можно) если игрок вовремя не остановится")]
  public class SarDayyniAttack4 : SarDayyniAttack
  {
	 public FsmGameObject bullet;

	 public FsmFloat maxRadius;
	 public FsmFloat midRadius;
	 public FsmFloat minRadius;
	 public FsmFloat maxHeight;
	 public FsmFloat minHeight;

	 public override void OnEnter()
	 {
		base.OnEnter();
		var teleport = bullet.Value.GetComponent<it.Game.NPC.Enemyes.Boses.SarDayyni.SarDayyniTeleport>();

		com.ootii.Actors.BodyCapsule body = new com.ootii.Actors.BodyCapsule();

		for (int i = 0; i < PlayerBehaviour.Instance.ActorController.BodyShapes.Count; i++)
		{
		  if (PlayerBehaviour.Instance.ActorController.BodyShapes[i].Name == "Body Capsule")
			 body = PlayerBehaviour.Instance.ActorController.BodyShapes[i] as com.ootii.Actors.BodyCapsule;
		}

		Vector3 target = _go.transform.position;

		bool isCurrectPoint = false;
		do
		{
		  target = GetPosition();
		  Vector3 point1 = target + body.Offset;
		  Vector3 point2 = target + body.EndOffset;

		  isCurrectPoint = !Physics.CapsuleCast(point1, point2, body.Radius, PlayerBehaviour.Instance.transform.forward, body.Radius, -1);

		} while (!isCurrectPoint);

		teleport.target = target;

		bullet.Value.transform.position = PlayerBehaviour.Instance.HipBone.position + PlayerBehaviour.Instance.transform.forward * 1.3f;
		bullet.Value.SetActive(true);

		Fsm.Event(OnComplete);

	 }
	 private Vector3 GetPosition()
	 {
		Vector2 direct = Random.insideUnitCircle;
		float dist = Random.Range(minRadius.Value, maxRadius.Value);

		Vector3 target = _go.transform.position;
		target.y = minHeight.Value + Random.Range(0, 20f);
		target += new Vector3(direct.x, 0, direct.y) * dist;
		return target;
	 }
	 public bool InHeight(Vector3 target)
	 {
		return ((target.y < maxHeight.Value + 0.1f || target.y > maxHeight.Value - 0.1f) || (target.y < maxHeight.Value + 0.1f || target.y > maxHeight.Value - 0.1f));
	 }

  }
}