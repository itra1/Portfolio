using UnityEngine;
using DG.Tweening;
using it.Game.Items.Inventary;
using it.Game.Player.Interactions;

namespace it.Game.Environment.GreenCthulhu
{
  public class HasturMini : Environment, IInteractionCondition
  {
    [SerializeField]
    private UnityEngine.Events.UnityEvent _onComplete;

    [SerializeField]
    private GameObject _seedInteruct;

    [SerializeField]
    private Transform _seedPosition;

    private GameObject _seedInst;

    [SerializeField]
    private string _seedUuid;


    public void AddSeed()
    {

      SetInstanceSeed();
      InventaryItem ii = _seedInst.GetComponent<InventaryItem>();
      ii.ColorShow(null);


      State = 1;
      Save();

      DOVirtual.DelayedCall(2f, () =>
      {
        _onComplete?.Invoke();
        //_seedInteruct.SetActive(false);
      });

    }

    protected override void ConfirmState(bool isForce = false)
    {
      base.ConfirmState(isForce);

      if(isForce && State == 1)
      {
        SetInstanceSeed();
        _seedInteruct.SetActive(false);
      }

    }

    private void SetInstanceSeed()
    {

      GameObject prefab = Managers.GameManager.Instance.Inventary.GetPrefab(_seedUuid);

      _seedInst = Instantiate(prefab, _seedPosition);
      _seedInst.transform.localScale = Vector3.one;
      _seedInst.transform.localPosition = Vector3.zero;
      _seedInst.transform.rotation = _seedPosition.rotation;
      InventaryItem ii = _seedInst.GetComponent<InventaryItem>();
      ii.CheckExistsAndDeactive = false;

    }

	 public bool InteractionReady()
	 {
		return Managers.GameManager.Instance.Inventary.ExistsItem(_seedUuid);
    }
  }
}