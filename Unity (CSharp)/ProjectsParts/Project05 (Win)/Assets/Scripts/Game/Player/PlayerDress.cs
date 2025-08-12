using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace it.Game.Player
{
  public class PlayerDress : MonoBehaviour
  {
	 private Animator _animator;
	 [SerializeField]
	 private GameObject[] _sources;
	 [SerializeField]
	 private GameObject _tails;
	 [SerializeField]
	 private Material[] _tailsMaterials;

	 private int skinIndex = -1;

	 public bool IsActive
	 {
		get => _isActive;
		set =>  _isActive = value;
	 }

	 private bool _isActive;

	 public Animator Animator
	 {
		get
		{
		  if (_animator == null)
			 _animator = GetComponent<Animator>();
		  return _animator;
		}
		set
		{
		  _animator = value;
		}
	 }

	 public void SetDress(int num)
	 {
		skinIndex = num;
		ConfirmDress();
	 }

	 public void SetLayer(LayerMask targetLayer)
	 {
		GameObject skin = _sources[skinIndex];

		Renderer[] skinRenderers = skin.transform.GetComponentsInChildren<Renderer>(true);

		for(int i = 0; i < skinRenderers.Length; i++)
		{
		  skinRenderers[i].gameObject.layer = targetLayer.value;
		}

		Renderer[] tailsRenderer = _tails.transform.GetComponentsInChildren<Renderer>(true);

		for (int i = 0; i < tailsRenderer.Length; i++)
		{
		  tailsRenderer[i].gameObject.layer = targetLayer.value;
		}

	 }

	 private void ConfirmDress()
	 {

		for (int i = 0; i < _sources.Length; i++)
		{
		  _sources[i].SetActive(i == skinIndex);
		}

		Renderer[] renderersTails = _tails.GetComponentsInChildren<Renderer>();

		for (int i = 0; i < renderersTails.Length; i++)
		{
		  renderersTails[i].material = _tailsMaterials[skinIndex];
		}
	 }

	 public void Change()
	 {
		skinIndex++;

		if (skinIndex >= _sources.Length)
		  skinIndex = 0;

		ConfirmDress();

	 }


  }
}