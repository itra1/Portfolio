using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace it.Game.Environment.Handlers
{
  public class AuraPresetSet : MonoBehaviourBase
  {
	 [SerializeField]
	 private string _presetName;
	 
	 public void SetPreset()
	 {
		if (string.IsNullOrWhiteSpace(_presetName))
		{
		  Logger.LogWarning("No set name aura preset");
		  return;
		}
		
		CameraBehaviour.Instance.AuraCamera.frustumSettings.LoadBaseSettings(Resources.Load<Aura2API.AuraBaseSettings>("AuraPresets/" + _presetName));

	 }

  }
}