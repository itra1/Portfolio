using UnityEngine;
using System.Collections;
using DG.Tweening;
using Leguar.TotalJSON;
namespace it.Game.Environment.GreenCthulhu
{
  public class ScullBarrier : Environment
  {
    /*
     * Состояния
     * 0 - ждем шаманов
     * 2 - открываем
     * 
     */

    private bool _greenShaman = false;
    private bool _redShaman = false;
    private bool _purpurShaman = false;

    [SerializeField]
    private it.Game.NPC.Enemyes.Cthulhu.Cthulhu _shamanGreen;
    [SerializeField]
    private it.Game.NPC.Enemyes.Cthulhu.Cthulhu _shamanRed;
    [SerializeField]
    private it.Game.NPC.Enemyes.Cthulhu.Cthulhu _shamanPurpur;

    [SerializeField]
    public GameObject _barrier;

    private int countShamanMove = 3;
    private string fsmName = "Behaviour";

    [ContextMenu("AddPurpurShaman")]
    public void AddPurpurShaman()
    {
      _purpurShaman = true;
      Save();
      ActivateShamans();
    }
    [ContextMenu("AddGreenShaman")]
    public void AddGreenShaman()
    {
      _greenShaman = true;
      Save();
      ActivateShamans();
    }
    [ContextMenu("AddRedShaman")]
    public void AddRedShaman()
    {
      _redShaman = true;
      Save();
      ActivateShamans();
    }

    private void ActivateShamans()
    {
      if (_shamanPurpur.gameObject.activeInHierarchy != _purpurShaman)
        _shamanPurpur.gameObject.SetActive(_purpurShaman);
      if (_shamanGreen.gameObject.activeInHierarchy != _greenShaman)
        _shamanGreen.gameObject.SetActive(_greenShaman);
      if (_shamanRed.gameObject.activeInHierarchy != _redShaman)
        _shamanRed.gameObject.SetActive(_redShaman);

      _shamanRed.GetFsm("Behaviour").SendEvent("OnOpenShield");
      _shamanPurpur.GetFsm("Behaviour").SendEvent("OnOpenShield");
      _shamanGreen.GetFsm("Behaviour").SendEvent("OnOpenShield");

      //_shamanPurpur.ActiveAllPlayMakerFsm(false);
      //_shamanPurpur.ActivePlayMakerFsm("OpenShield");
      //_shamanGreen.ActiveAllPlayMakerFsm(false);
      //_shamanGreen.ActivePlayMakerFsm("OpenShield");
      //_shamanRed.ActiveAllPlayMakerFsm(false);
      //_shamanRed.ActivePlayMakerFsm("OpenShield");

    }

    protected override void ConfirmState(bool isForce = false)
    {
      base.ConfirmState(isForce);

      ActivateShamans();

      if (isForce)
      {
        _barrier.SetActive(State == 0);
      }

    }

    /// <summary>
    /// Триггер при входе на платформу
    /// </summary>
    public void PlayerEnterPlatform()
    {
      if (!_greenShaman || !_redShaman || !_purpurShaman)
        return;

      if (State >= 2)
        return;

      SetActivateShamans();
      State = 2;
      Save();
    }

    private void SetActivateShamans()
    {
      countShamanMove = 3;

      var fsm = _shamanPurpur.GetFsm(fsmName);
      fsm.Fsm.Event("StartMove"); 
      fsm = _shamanGreen.GetFsm(fsmName);
      fsm.Fsm.Event("StartMove");
      fsm = _shamanRed.GetFsm(fsmName);
      fsm.Fsm.Event("StartMove");
    }

    public void ShamanMoveComplete()
    {
      countShamanMove--;

      if (countShamanMove <= 0)
        ActivateCast();
    }

    private void ActivateCast()
    {

      var fsm = _shamanPurpur.GetFsm(fsmName);
      fsm.Fsm.Event("OnCast");
      fsm = _shamanGreen.GetFsm(fsmName);
      fsm.Fsm.Event("OnCast");
      fsm = _shamanRed.GetFsm(fsmName);
      fsm.Fsm.Event("OnCast");

      DOVirtual.DelayedCall(1, ActivateLaser);
      DOVirtual.DelayedCall(2, OpenShield);
    }

    private void ActivateLaser()
    {

    }
    private void OpenShield()
    {
      _barrier.SetActive(false);
    }
    #region Save

    protected override void LoadData(JValue data)
    {
      base.LoadData(data);

      JSON dt = data as JSON;
      _greenShaman = dt.GetBool("greenShaman");
      _redShaman = dt.GetBool("redShaman");
      _purpurShaman = dt.GetBool("purpurShaman");

    }

    protected override JValue SaveData()
    {
      JSON dt = new JSON();
      dt.Add("greenShaman",_greenShaman);
      dt.Add("redShaman", _redShaman);
      dt.Add("purpurShaman", _purpurShaman);

      return dt;
    }
    #endregion


  }
}