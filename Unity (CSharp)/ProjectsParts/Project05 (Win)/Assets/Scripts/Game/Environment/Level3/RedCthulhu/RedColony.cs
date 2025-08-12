using UnityEngine;
using System.Collections;
using DG.Tweening;
namespace it.Game.Environment.RedCthulhu
{
  public class RedColony : Environment
  {
    /*
     * Состония
     * 0-стартовое состояние
     * 1 - пастель появилась в воротах
     * 2 - пастель пообщалась с шаманом
     * 3 - пастель активировала догму
     * 4 - шаман отблагодарил и улетел
     * 
     */

    [SerializeField]
    private UnityEngine.Events.UnityEvent _onComplete;

	 [SerializeField]
	 private it.Game.NPC.Enemyes.Cthulhu.RedGuard[] _guards;
    [SerializeField]
    private it.Game.NPC.Enemyes.Cthulhu.Cthulhu _shaman;
    [SerializeField]
    private it.Game.Environment.Handlers.PegasusController _pegasusColony;
    [SerializeField]
    private it.Game.Environment.Level3.Dogma _dogma;

    public void PastelInGate()
	 {
      if (State >= 1)
        return;

		State = 1;

      Save();

      for(int i = 0;  i < _guards.Length; i++)
      {
        PlayMakerFSM[] components = _guards[i].GetComponents<PlayMakerFSM>();
        for(int x = 0; x < components.Length; x++)
        {
          if (components[x].Fsm.Name.Equals("Guard"))
            components[x].Fsm.Event("PlayerInGate");
        }
      }

	 }

    protected override void ConfirmState(bool isForce = false)
    {
      base.ConfirmState(isForce);

      Debug.Log("State = " + State);
      if (State >=1)
      {
        for (int i = 0; i < _guards.Length; i++)
        {
          PlayMakerFSM[] components = _guards[i].GetComponents<PlayMakerFSM>();
          for (int x = 0; x < components.Length; x++)
          {
            if (components[x].Fsm.Name.Equals("Guard"))
              components[x].Fsm.Event("FirstLookComplete");
          }
        }
      }
      if (State <= 1)
      {
        _shaman.gameObject.SetActive(true);
        _shaman.GetFsm("Behaviour").SendEvent("StartFSM");
        _dogma.Deactivate();
      }
      else if(State == 2)
      {
        _shaman.gameObject.SetActive(true);
        _shaman.GetFsm("Behaviour").SendEvent("FirstComplete");
        _dogma.Deactivate();
      }else if(State == 3)
      {
        _dogma.Activate();
        _shaman.gameObject.SetActive(true);
        _shaman.GetFsm("Behaviour").SendEvent("OnMoveToPortal");
      }
      else if (State > 3)
      {
        _dogma.Activate();
        _shaman.gameObject.SetActive(false);

		}

    }

    /// <summary>
    /// Вызывается из PlayMaker shamanom
    /// </summary>
    [ContextMenu("Chaman Call")]
    public void ShamanLookQuest()
    {
      State = 2;
      Save();
      _pegasusColony.Activate(() =>
      {
        DOVirtual.DelayedCall(1f, () => {
          _pegasusColony.Deactivate();
        });
      });
    }

    public void QuestComplete()
    {
      State = 3;
      _dogma.Activate();
      Save();
      _shaman.GetFsm("Behaviour").SendEvent("OnMoveToPortal");
      //_shaman.ActiveAllPlayMakerFsm(false);
      //_shaman.ActivePlayMakerFsm("Move to portal");
    }

    public void PortalComplete()
    {
      _onComplete?.Invoke();
      State = 4;
      Save();
    }

  }
}