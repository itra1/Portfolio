using System.Collections;
using Slate;
using UnityEngine;
using UnityEngine.VFX;

namespace it.Game.Environment.Level4
{
  public class TheKarmamancerGate : MonoBehaviour
  {
	 [SerializeField]
	 private Transform _portal;
	 [SerializeField]
	 private Cutscene _closegateCutScene;
	 [SerializeField]
	 private VisualEffect _vfx;

	 private bool _isGate;

	 public void Clear()
	 {
		_vfx.SetFloat("Rate", 0);
		_portal.localPosition = new Vector3(0, -6.5f, 0);
		_portal.gameObject.SetActive(false);
		_isGate = false;
	 }

	 public void Show()
	 {
		_portal.gameObject.SetActive(true);
		_closegateCutScene.Play(() =>
		{
		  _vfx.SetFloat("Rate", 100);
		  _portal.localPosition = new Vector3(0, -1.2f, 0);
		});
	 }

	 public void PortalEnter()
	 {
		if (_isGate) return;
		_isGate = true;

		it.Game.Managers.GameManager.Instance.NextLevel();
	 }

  }
}