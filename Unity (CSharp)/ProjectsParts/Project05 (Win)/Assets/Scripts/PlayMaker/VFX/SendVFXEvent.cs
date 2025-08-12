using UnityEngine;
using UnityEngine.VFX;

namespace HutongGames.PlayMaker
{
  [ActionCategory("VFX")]
  public class SendVFXEvent : FsmStateAction
  {
    public FsmGameObject _gameObject;
    public FsmString _vfxNameObject;
    [RequiredField]
    public FsmString _eventName;

    public override void OnEnter()
    {

      VisualEffect effect = null;

      if (_gameObject.Value == null)
        return;

      if(string.IsNullOrEmpty(_vfxNameObject.Value))
        effect = _gameObject.Value.GetComponent<VisualEffect>();
		else
		{
        Transform inst = _gameObject.Value.transform.Find(_vfxNameObject.Value);
        if(inst != null)
          effect = inst.GetComponent<VisualEffect>();
      }
      if(effect != null)
        effect.SendEvent(_eventName.Value);

      Finish();
    }
  }
}