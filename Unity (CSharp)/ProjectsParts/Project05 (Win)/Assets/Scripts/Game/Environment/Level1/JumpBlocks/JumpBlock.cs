using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using it.Game.Player.Save;
using com.ootii.Messages;
using it.Game.Environment.Handlers;
using DG.Tweening;

namespace it.Game.Environment.Level1.JumpBlocks
{

  public class JumpBlock : Environment
  {
	 //_DissolveCutoff
	 /*
	  * Статусы:
	  * 0 - стандартно
	  * 1 - поднято
	  */
	 public Transform _block;
	 public List<string> _startMaterials = new List<string>();
	 public List<MaterialData> matData;
	 public List<Material> _materials;

	 public UnityEngine.Events.UnityEvent OnComplete;

	 [SerializeField]
	 private PegasusController _pegasusController;

	 protected override void Start()
	 {
		base.Start();

		if(State == 0)
		  ActivatePlatform(false);

	 }

	 private void ActivatePlatform(bool isActive)
	 {
		for(int i = 0; i < _materials.Count; i++)
		{
		  _materials[i].SetFloat("_DissolveCutoff", isActive ? 0 : 1);
		}
		Collider[] colls = transform.GetComponentsInChildren<Collider>(true);
		for(int i = 0; i < colls.Length; i++)
		{
		  colls[i].enabled = isActive;
		}
	 }
	 private void ActivateAnim()
	 {
		Collider[] colls = transform.GetComponentsInChildren<Collider>(true);
		for (int i = 0; i < colls.Length; i++)
		{
		  colls[i].enabled = true;
		}

		for (int i = 0; i < _materials.Count; i++)
		{
		  Material mat = _materials[i];
		  DOTween.To(() => mat.GetFloat("_DissolveCutoff"), (x) => mat.SetFloat("_DissolveCutoff", x), 0, 2f);
		}
		
	 }

	 [System.Serializable]
	 public struct MaterialData
	 {
		public string sourceName;
		public Material mat;
	 }

	 [ContextMenu("UpRoad")]
	 public void UpRoad()
	 {
		State = 1;
		ConfirmState(false);
		Save();
	 }

	 public void ActivateVisible()
	 {
		_pegasusController.Activate(() =>
		{
		  OnComplete?.Invoke();
		  DOVirtual.DelayedCall(0.5f, _pegasusController.Deactivate);
		});
		_pegasusController.OnIndexChenge = (indx) =>
		{
		  if(indx == 2)
		  {
			 UpRoad();
		  }

		};
	 }

	 [ContextMenu("FindMatrials")]
	 public void FindMatrials()
	 {
		_startMaterials.Clear();
		Renderer[] renderers = _block.GetComponentsInChildren<Renderer>(true);
		for(int i = 0; i < renderers.Length; i++)
		{
		  if (!_startMaterials.Contains(renderers[i].material.name))
			 _startMaterials.Add(renderers[i].material.name);
		}
	 }

	 [ContextMenu("ReplaceMaterial")]
	 public void ReplaceMaterial()
	 {
		Renderer[] renderers = _block.GetComponentsInChildren<Renderer>(true);
		for (int i = 0; i < renderers.Length; i++)
		{
		  var structura = matData.Find(x => x.sourceName == renderers[i].material.name);
		  renderers[i].material = structura.mat;
		}
	 }

	 protected override void ConfirmState(bool force = false)
	 {
		base.ConfirmState(force);
		if (force)
		{
		  ActivatePlatform(State != 0);
		}
		else
		{
		  if (State == 1)
			 ActivateAnim();
		}
	 }


  }
}