using UnityEngine;
using DG.Tweening;

namespace it.Game.NPC.Enemyes
{
  /// <summary>
  /// Голем гуляющтй по лабиринту
  /// </summary>
  public class FireGolemMaze : Enemy
  {
	 [SerializeField] private PlayMakerFSM _behaviour;
	 [SerializeField] private Material _material;
	 [SerializeField] private Material _currentMaterial;
	 [SerializeField] [ColorUsage(false, true)] private Color _activeColor;
	 public PlayMakerFSM BehaviourFsm { get => _behaviour; set => _behaviour = value; }

	 private Vector3 _startPosition;
	 private bool _isActive;

	 protected override void Awake()
	 {
		base.Awake();
		_startPosition = transform.position;
		_isActive = false;
		CreateMaterials();
	 }

	 private void CreateMaterials()
	 {
		_currentMaterial = Instantiate(_material);
		_currentMaterial.SetColor("_EmissionColor", Color.black);

		Transform skin = transform.Find("Rig_golems/Geometry");

		Renderer[] renders = skin.GetComponentsInChildren<Renderer>();
		for(int i = 0; i < renders.Length; i++)
		{
		  renders[i].material = _currentMaterial;
		}
	 }

	 public void Activate()
	 {
		if (_isActive) return;
		_isActive = true;
		_currentMaterial.DOColor(_activeColor, "_EmissionColor", 1).OnComplete(()=> {
		  BehaviourFsm.SendEvent("OnActivate");
		});


	 }

	 public void Deactivate()
	 {
		if (!_isActive) return;
		_isActive = false;

		BehaviourFsm.SendEvent("OnDeactivate");
		_currentMaterial.DOColor(Color.black, "_EmissionColor", 1);
	 }

  }
}