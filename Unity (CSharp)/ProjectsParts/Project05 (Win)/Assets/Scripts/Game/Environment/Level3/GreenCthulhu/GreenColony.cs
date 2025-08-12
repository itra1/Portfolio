using UnityEngine;
using System.Collections;
using DG.Tweening;
namespace it.Game.Environment.GreenCthulhu
{
  public class GreenColony : Environment
  {
    /*
     * Состония
     * 0-стартовое состояние
     * 2 - пастель пообщалась с шаманом
     * 3 - пастель активировала статую с едой
     * 4 - шаман отблагодарил и улетел
     * 
     */

    [SerializeField]
    private UnityEngine.Events.UnityEvent _onComplete;

    [SerializeField]
    private it.Game.NPC.Enemyes.Cthulhu.Cthulhu _shaman;
    [SerializeField]
    private it.Game.Environment.Handlers.PegasusController _pegasusColony;

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);

		if (isForce)
		{
        if (State < 2)
        {
          _shaman.gameObject.SetActive(true);
          _shaman.GetFsm("Behaviour").SendEvent("StartFSM");

        }
        else if (State == 2)
		  {
          _shaman.gameObject.SetActive(true);
          _shaman.GetFsm("Behaviour").SendEvent("FirstComplete");

        }else if (State > 2)
        {
          _shaman.gameObject.SetActive(false);
		  }
		  else
        {
          _shaman.gameObject.SetActive(true);
        }
		}
	 }

	 [ContextMenu("Chaman Call")]
    public void ShamanLookQuest()
    {
      State = 2;
      Save();
      _pegasusColony.Activate(() =>
      {

        DOVirtual.DelayedCall(1f, () =>
        {
          _pegasusColony.Deactivate();
        });

      });
    }

    public void QuestComplete()
    {
      State = 3;
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