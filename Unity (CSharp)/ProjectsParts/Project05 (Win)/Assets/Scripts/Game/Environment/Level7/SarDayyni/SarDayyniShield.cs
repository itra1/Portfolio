using System.Collections;

using DG.Tweening;

using UnityEngine;

namespace it.Game.Environment.Level7.SarDayyni
{
  public class SarDayyniShield : MonoBehaviour
  {
	 private Renderer _renderer;
	 private void Start()
	 {
		_renderer = gameObject.GetComponentInChildren<Renderer>();
		_renderer.material = Instantiate(_renderer.material);
		Clear();
	 }

	 public void Clear()
	 {
		_renderer.material.SetFloat("_Dissolve", 1);
		_renderer.GetComponent<Collider>().enabled = true;
	 }

	 public void Open()
	 {
		_renderer.material.DOFloat(0, "_Dissolve", 1).OnComplete(() =>
		{
		  _renderer.GetComponent<Collider>().enabled = false;
		});

	 }

  }
}