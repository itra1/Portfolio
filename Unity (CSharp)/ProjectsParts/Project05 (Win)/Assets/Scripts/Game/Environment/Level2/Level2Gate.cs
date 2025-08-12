using UnityEngine;
using System.Collections;
using it.Game.Environment;
using DG.Tweening;

namespace it.Game.Environment.Level2
{
  public class Level2Gate : Environment
  {
    [SerializeField]
    private Collider _collider;
    [SerializeField]
    private Light _light;
    [SerializeField]
    private GameObject _model;

    protected override void Start()
    {
      base.Start();
    }
    [ContextMenu("Set Open")]
    public void SetOpen()
    {
      State = 2;
      _collider.gameObject.SetActive(true);
      _model.gameObject.SetActive(true);
      _light.intensity = 0;
      _light.DOIntensity(8, 1f);
      Save();
    }

    private void SetClose()
    {
      _collider.gameObject.SetActive(false);
      _model.gameObject.SetActive(false);
      _light.intensity = 0;
    }

    public void PlayerEnter()
    {
      Debug.Log("Level Complete");
    }

    public void Portals()
	 {
      if (State != 2)
        return;

      State = 3;

      it.Game.Managers.GameManager.Instance.LoadMenu();
      //it.Game.Managers.GameManager.Instance.NextLevel();
    }

    protected override void ConfirmState(bool isForce = false)
    {
      base.ConfirmState(isForce);

      if(State == 2)
      {
        SetOpen();
      }
      else
      {
        SetClose();
      }

    }

  }


}