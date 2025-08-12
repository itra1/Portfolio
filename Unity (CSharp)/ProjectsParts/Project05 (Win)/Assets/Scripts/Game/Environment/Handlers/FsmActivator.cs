using UnityEngine;
using System.Collections;
namespace it.Game.Environment.Handlers
{
  public class FsmActivator : MonoBehaviourBase
  {
    [SerializeField]
    private PlayMakerFSM _fsm;
    [SerializeField]
    private string _eventName;


    public void Activate()
    {
      _fsm.Fsm.Event(_eventName);
    }

  }
}