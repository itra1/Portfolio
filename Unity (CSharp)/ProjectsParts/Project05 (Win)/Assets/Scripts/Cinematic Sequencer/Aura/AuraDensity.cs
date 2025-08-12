using UnityEngine;
using System.Collections;

namespace Slate.ActionClips.Aura
{
  [Category("Aura")]
  [Description("Изменение параметра")]
  public class AuraDensity : AuraBase
  {
	 protected override void ConfirmValue()
	 {
		_aura.densityInjection.strength = newValue;
	 }

	 protected override void SetStartValue()
	 {
		_startvalue = _aura.densityInjection.strength;
	 }
  }
}