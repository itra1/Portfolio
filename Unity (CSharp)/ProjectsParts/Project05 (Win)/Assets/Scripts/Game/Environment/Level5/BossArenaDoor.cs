using System.Collections;

using UnityEngine;
using DG.Tweening;

namespace it.Game.Environment.Level5
{
  public class BossArenaDoor : MonoBehaviour
  {
	 public bool IsOpen { get; set; }

	 private Renderer _renderer;
	 
	 private void Awake()
	 {
		_renderer = GetComponentInChildren<Renderer>();
		_renderer.material = Instantiate(_renderer.material);
	 }

	 public void Open(bool animate)
	 {
		if (IsOpen) return;

		if (!animate)
		  _renderer.gameObject.SetActive(false);
		else
		{
		  _renderer.material.SetFloat("_Dissolve", 1);
		  _renderer.material.DOFloat(0, "_Dissolve", 1).OnComplete(()=> {
			 _renderer.gameObject.SetActive(false);
		  });
		}
		
	 }

	 public void Close(bool animate)
	 {
		if (!_renderer) return;
		if (!animate)
		  _renderer.gameObject.SetActive(true);
		else
		{
		  _renderer.material.SetFloat("_Dissolve", 0);
		  _renderer.gameObject.SetActive(true);
		  _renderer.material.DOFloat(1, "_Dissolve", 1).OnComplete(()=> {
		  });
		}
	 }

  }
}