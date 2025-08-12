using UnityEngine;
using it.Game.Managers;
using DG.Tweening;

namespace it.Game.Environment.All.Portals {
  /// <summary>
  /// Портал
  /// </summary>
  public class Portal : Environment {
    /*
     * Состочния:
     * 0 - отключен
     * 1 - включен
     * 
     */

    [SerializeField]
    private bool _startActive = true;
    [SerializeField]
    private GameObject _block;
    [SerializeField]
    private Transform _targetTeleportPosition;
    [SerializeField]
    private Light _light;

    [SerializeField]
    private UnityEngine.Events.UnityEvent _onPortalEvent;

    protected override void Start() {
      base.Start();
      _light.intensity = 0;
    }

    protected override void NoLoadData() {
      base.NoLoadData();
      if (_startActive)
        SetActivateBlock(_startActive);
    }
	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);
      ConformActive();

    }

    private void ConformActive()
	 {
      if (State == 1 || _startActive)
      {
        _block.SetActive(true);
        _light.DOIntensity(3, 1);
      }
      else
      {
        _block.SetActive(false);
        _light.DOIntensity(0, 1);
      }
    }

	 public void SetActivateBlock(bool isActive = true) {
      if (isActive) {
        State = 1;
      } else {
        State = 0;
      }
      Save();
      ConformActive();
    }
    /// <summary>
    /// Выполнение телепортации
    /// </summary>
    public void OnPortal() {
      if (State != 1)
        return;

      GameManager.Instance.GameInputSource.IsEnabled = false;

      UiManager.Instance.FillAndRepeatColor(new Color32(0, 0, 0, 0),
          new Color32(0, 0, 0, 255), 1, null, () => {
            Game.Player.PlayerBehaviour.Instance.PortalJump(_targetTeleportPosition);
          }, () => {
            _onPortalEvent?.Invoke();
            GameManager.Instance.GameInputSource.IsEnabled = true;
          });
    }
    private void OnDrawGizmosSelected() {
      if (_targetTeleportPosition != null)
        Gizmos.DrawLine(transform.position, _targetTeleportPosition.position);
    }

  }
}