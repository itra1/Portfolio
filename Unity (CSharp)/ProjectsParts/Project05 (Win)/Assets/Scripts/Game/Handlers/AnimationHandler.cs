using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace it.Game.Handles
{
  [System.Serializable]
  public class EventString : UnityEngine.Events.UnityEvent<string>
  { }

  public class AnimationHandler : MonoBehaviour
  {
    [HideInInspector]
    public EventString OnEventString;

    public void EmitEventString(string data)
    {
      OnEventString?.Invoke(data);
    }
  }
}