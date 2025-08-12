using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace it.UI {
  /// <summary>
  /// Базовый класс для диалогов
  /// </summary>
  public class UIDialog : UUIDBase, it.Game.IEscape {

    public UnityEngine.Events.UnityAction onEnamble;
    public UnityEngine.Events.UnityAction onDisable;

    [SerializeField]
    protected bool _isEscape;
    [SerializeField]
    private bool _showCursor = true;

    [SerializeField]
    private bool _deactiveGameInput = false;

    protected bool _isEscapeProcess;

    public Animator Animator {
      get {
        if (_animator == null)
          _animator = GetComponentInChildren<Animator>();
        return _animator;
      }
      set => _animator = value;
    }
    public bool LastHierarchy { get => _LastHierarchy; }

    [SerializeField]
    private bool _LastHierarchy = false;
    private Animator _animator;

    protected Tween _deactivCall = null;

    protected virtual void OnEnable() {

      com.ootii.Messages.MessageDispatcher.SendMessage(this, EventsConstants.UIEnable, this, 0);
      _isEscapeProcess = false;

      onEnamble?.Invoke();

      if (_isEscape)
        it.Game.Managers.GameManager.Instance.AddEscape(this);
      if(_showCursor)
        it.Game.Managers.GameManager.Instance.SetCursorVisible(this,true);
      if (_deactiveGameInput)
        it.Game.Managers.GameManager.Instance.GameInputSource.IsEnabled = false;
      Show();
    }

    protected virtual void OnDisable() {
      if (_isEscape && !it.Game.Managers.GameManager.IsQuiting)
        it.Game.Managers.GameManager.Instance.RemoveEscape(this);
      if (_showCursor && !it.Game.Managers.GameManager.IsQuiting)
        it.Game.Managers.GameManager.Instance.SetCursorVisible(this, false);
      if (_deactiveGameInput && !it.Game.Managers.GameManager.IsQuiting)
        it.Game.Managers.GameManager.Instance.GameInputSource.IsEnabled = true;

      com.ootii.Messages.MessageDispatcher.SendMessage(this, EventsConstants.UIDisable, this, 0);
      onDisable?.Invoke();
    }
    public virtual void Show() {
      gameObject.SetActive(true);

      Animator?.SetBool("Visible", true);
      if (_deactivCall != null && _deactivCall.active)
        _deactivCall.Kill();
    }

    public virtual void Hide(float timeHide = 1f) {
      Animator?.SetBool("Visible", false);
      _deactivCall = DOVirtual.DelayedCall(timeHide, () => { gameObject.SetActive(false); });
    }

    public virtual void Escape() {
      _isEscapeProcess = true;
    }
  }
}