using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace it.Game.Animals
{
  [ActionCategory("Animals")]
  [HutongGames.PlayMaker.Tooltip("Появление призрака оленя")]
  public class DeerGhostShow : FsmStateAction
  {
	 [Tooltip("Показ")]
	 public FsmBool show = true;
	 public FsmGameObject gameObject;
	 public FsmFloat speed = 1;

	 public FsmEvent onComplete;

	 private Material _material;
	 private ParticleSystem _particles;
	 private ParticleSystem.EmissionModule _emission;
	 private Light[] _light;

	 private float _maxIntensive;
	 private Color _defColor;
	 private float percent;

	 public override void OnEnter()
	 {
		var renderer = gameObject.Value.GetComponentInChildren<SkinnedMeshRenderer>();
		_material = renderer.material;
		_defColor = _material.GetColor("_Color");
		_particles = gameObject.Value.GetComponentInChildren<ParticleSystem>();
		_emission = _particles.emission;
		_light = gameObject.Value.GetComponentsInChildren<Light>();
		_maxIntensive = _light[0].intensity;

		if (show.Value)
		{
		  _material.SetColor("_Color", new Color(_defColor.r, _defColor.g, _defColor.b, 0));
		  _emission.enabled = false;
		  percent = 0;
		}
		else
		{
		  _emission.enabled = false;
		  percent = 1;
		}
	 }

	 public override void OnUpdate()
	 {
		percent += (show.Value ? 1 : -1) * Time.deltaTime * speed.Value;

		percent = Mathf.Clamp(percent, 0f, 1f);

		Color targetColor = Color.Lerp(new Color(_defColor.r, _defColor.g, _defColor.b, 0), _defColor, percent);

		_material.SetColor("_Color", targetColor);

		if ((show.Value && percent >= 1) || (!show.Value && percent <= 0))
		{
		  if(show.Value)
			 _emission.enabled = true;

		  Fsm.Event(onComplete);
		}

	 }

  }
}