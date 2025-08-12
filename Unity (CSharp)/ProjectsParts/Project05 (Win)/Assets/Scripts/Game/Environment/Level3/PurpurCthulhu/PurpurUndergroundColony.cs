using UnityEngine;
using System.Collections;
using DG.Tweening;
namespace it.Game.Environment.GreenCthulhu
{
  public class PurpurUndergroundColony : Environment
  {

    [SerializeField]
    private it.Game.Environment.Handlers.PegasusController _pegasus;
    [SerializeField]
    private it.Game.Environment.Handlers.PegasusController _pegasusToPortal;
    [SerializeField]
    private Light _portalLight;
    [SerializeField]
    private ParticleSystem _portalParticles;
    [SerializeField]
    private Transform _stoneGate;

    private int mobCount;

    private bool _waitOut = false;

    public void PlayerVisible()
    {

    }

    public void ActivateHuastar()
    {
      State = 1;
      Save();

      _pegasus.Activate(() =>
      {
        OpenGateAnimate();
        DOVirtual.DelayedCall(1, () =>
        {
          SendAllChtulhuMoveToGate();
        });

      });

    }

    protected override void ConfirmState(bool isForce = false)
    {
      base.ConfirmState(isForce);


      if(isForce)
      {
        OpenGate(State == 1);
      }

    }

    private void OpenGateAnimate()
    {
      _portalLight.DOIntensity(6f, 1);
      _portalParticles.Play();
      _stoneGate.DOLocalMove(new Vector3(0, 5, 0), 1);
    }

    private void OpenGate(bool isOpen)
    {
      _portalLight.intensity = isOpen ? 6 : 0;
      _portalParticles.Stop();

      _stoneGate.localPosition = isOpen ? new Vector3(0, 5, 0) : Vector3.zero;

    }

    private void SendAllChtulhuMoveToGate()
    {
      var cthulhArr = GetComponentsInChildren<it.Game.NPC.Enemyes.Cthulhu.Cthulhu>();
      mobCount = cthulhArr.Length;
      _waitOut = true;

      DOVirtual.DelayedCall(2, GateToLevel_3);

      for (int i = 0; i < cthulhArr.Length; i++)
      {
        cthulhArr[i].ActiveAllPlayMakerFsm(false);
        cthulhArr[i].ActivePlayMakerFsm("MoveOut");
      }

    }

    private void GateToLevel_3()
    {
      if (!_waitOut)
        return;

      _waitOut = false;

      _pegasus.Deactivate();
      _pegasusToPortal.Activate(() =>
      {
        // Добавляем скрытый предмет для обозначения прохождения уровня
        Managers.GameManager.Instance.Inventary.AddItem(PurpurColony.DUNGEON_COMPLETE_ITEM);
        it.Game.Managers.GameManager.Instance.NextLevel(2,false);
        //_pegasusToPortal.Deactivate();
      });
    }

    

    public void DeactiveChtulhu()
    {
      mobCount--;

      if(mobCount <= 0)
      {
        GateToLevel_3();
      }
    }

  }
}