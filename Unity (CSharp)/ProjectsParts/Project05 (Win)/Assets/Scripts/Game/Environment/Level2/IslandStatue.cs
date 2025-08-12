using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace it.Game.Environment.Level2
{
  public class IslandStatue : Environment
  {
	 [SerializeField]
	 private Renderer _statue;
	 
	 private Material _material;
	 [SerializeField]
	 private Light _light;
	 [ColorUsage(false,true)]
	 [SerializeField]
	 private Color _activeEmissionMaterial;
	 [SerializeField]
	 private float _activeLightIntencity = 3;

	 private Material Material
	 {
		get
		{
		  if (_material == null)
			 _material = _statue.material;
		  return _material;
		}
	 }

	 public void OnGetItem()
	 {
		if (State != 0)
		  return;

		State = 1;

		OnLight();
	 }

	 protected override void OnStateChange()
	 {
		base.OnStateChange();

		if(State != 0)
		{
		  OnLight();
		}
		else
		{
		  OffLight();
		}

	 }

	 public void OnLight()
	 {
		float lTransaction = 1;
		DOTween.To(() => Material.GetColor("_Emission"), (x) => Material.SetColor("_Emission",x), _activeEmissionMaterial, lTransaction);
		_light.DOIntensity(_activeLightIntencity, lTransaction);
	 }

	 private void OffLight()
	 {
		Material.SetColor("_Emission", Color.black);
		_light.intensity = 0;
	 }

  }
}