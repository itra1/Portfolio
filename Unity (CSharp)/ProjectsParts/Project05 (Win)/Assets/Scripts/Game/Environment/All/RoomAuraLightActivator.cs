using System.Collections;

using UnityEngine;

namespace it.Game.Environment
{
  public class RoomAuraLightActivator : MonoBehaviour
  {
	 private Aura2API.AuraLight[] _auraLight;

	 private void Awake()
	 {
		FindLights();
	 }

	 private void FindLights()
	 {
		if (_auraLight == null || _auraLight.Length == 0)
		  _auraLight = GetComponentsInChildren<Aura2API.AuraLight>();
	 }

	 [ContextMenu("Activate")]
	 public void Activate()
	 {
		FindLights();

		for (int i = 0; i < _auraLight.Length; i++)
		{
		  _auraLight[i].enabled = true;
		}
	 }

	 [ContextMenu("Deactivate")]
	 public void Deactivate()
	 {
		FindLights();

		for (int i = 0; i < _auraLight.Length; i++)
		{
		  _auraLight[i].enabled = false;
		}
	 }

  }
}