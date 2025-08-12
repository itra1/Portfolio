using HutongGames.PlayMaker;
using Utilites.Geometry;
using UnityEngine;

namespace it.Game.PlayMaker.Enemy
{
  [ActionCategory("Enemy")]
  [HutongGames.PlayMaker.Tooltip("Токсичность дракона")]
  public class DragonToxic : FsmStateAction
  {
	 public FsmOwnerDefault _gameObject;
	 public FsmGameObject particles;
	 public FsmFloat maxRadius;
	 public FsmEvent OnComplete;
	 private GameObject _go;

	 private float _startTime;
	 float radius = 0.5f;

	 public override void OnEnter()
	 {
		base.OnEnter();
		_go = Fsm.GetOwnerDefaultTarget(_gameObject);
		_startTime = Time.time;
		radius = 0.5f;
		particles.Value.GetComponent<ParticleSystem>().Play();
	 }


	 public override void OnUpdate()
	 {
		base.OnUpdate();

		radius += Time.deltaTime;
//#if UNITY_EDITOR
//		com.ootii.Graphics.GraphicsManager.DrawCircle(_go.transform.position + Vector3.up * 0.5f, radius, Color.red, null, 1);
//#endif
		RaycastHit _hit;
		if (RaycastExt.SafeCircularCast(_go.transform.position + Vector3.up * 0.5f, _go.transform.forward, _go.transform.up, out _hit, radius, 20, it.Game.ProjectSettings.PlayerLayerMask))
		{
		  Game.Managers.GameManager.Instance.UserManager.Health.Damage(_go.transform, 1000);
		}

		if (radius >= maxRadius.Value)
		  Fsm.Event(OnComplete);
	 }

  }

}