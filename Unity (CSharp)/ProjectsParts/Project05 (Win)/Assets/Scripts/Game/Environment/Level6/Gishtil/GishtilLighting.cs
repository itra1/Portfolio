using UnityEngine;
using DG.Tweening;
using DigitalRuby.ThunderAndLightning;

namespace it.Game.Environment.Level6.Gishtil
{
  public class GishtilLighting : MonoBehaviourBase
  {
	 [ContextMenuItem("SetActive", "SetActiveLighting")]
	 [ContextMenuItem("SetDeactive", "SetDeactiveLighting")]
	 [SerializeField]
	 private bool _isEnabled;
	 public bool IsEnabled
	 {
		get => _isEnabled; set
		{
		  if (_isEnabled == value)	 return;
		  _isEnabled = value;

		  if (_isEnabled)
			 SetActiveLighting();
		  else
			 SetDeactiveLighting();
		}
	 }

	 private bool _isFireContact;
	 public bool IsFireContact { get => _isFireContact; set => _isFireContact = value; }

	 private DigitalRuby.ThunderAndLightning.LightningBoltPrefabScriptBase _lighting;
	 public LightningBoltPrefabScriptBase Lighting
	 {
		get
		{
		  if (_lighting == null)
			 _lighting = GetComponentInChildren<LightningBoltPrefabScriptBase>();
		  return _lighting;
		}
	 }


	 [SerializeField]
	 private Renderer _renderer;
	 [ColorUsage(false,true)]
	 [SerializeField]
	 private Color _color;
	 [SerializeField]
	 private Color _startFire;
	 [SerializeField]
	 private Color _colorFire;

	 private void Start()
	 {
		_renderer.material = Instantiate(_renderer.material);
		SetDeactiveLighting();
	 }

	 public void FireCintact(bool isContact)
	 {
		if (_isFireContact == isContact)
		  return;

		_isFireContact = isContact;
		if (!isContact)
		{
		  _lighting.GlowTintColor = _startFire;
		}
		else
		{
		  DOTween.To(() => _lighting.GlowTintColor, (x) => _lighting.GlowTintColor = x, _colorFire, 1).OnComplete(()=>{

			 transform.GetComponentInParent<GishtilBossArena>().EnemyActivateLight();
		  });
		}
	 }

	 public void SetActiveLighting()
	 {
		Lighting.Camera = CameraBehaviour.Instance.Camera;
		Lighting.enabled = true;
		_isEnabled = true;
		_renderer.material.SetColor("_EmissionColor", Color.black);
		DOTween.To(() => _renderer.material.GetColor("_EmissionColor"),
		  (x) => _renderer.material.SetColor("_EmissionColor", x),
		  _color,
		  1
		  );
	 }

	 public void SetDeactiveLighting()
	 {
		Lighting.enabled = false;
		_isEnabled = false;
		_renderer.material.SetColor("_EmissionColor", Color.black);
	 }

  }
}