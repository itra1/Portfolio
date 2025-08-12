using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace it.UI.ConfirmDialog
{
  public class ConfirmDialog : UIDialog
  {
    private UnityEngine.Events.UnityAction onConfirmAction;
    private UnityEngine.Events.UnityAction onCancelAction;

    [SerializeField]
    private TextMeshProUGUI _mainText;
    [SerializeField]
    private TextMeshProUGUI _confirmTextButton;
    [SerializeField]
    private TextMeshProUGUI _cancelTextButton;

    public void SetData(string mainText, string confirmText, string cancelText, UnityEngine.Events.UnityAction confirmAction, UnityEngine.Events.UnityAction cancelAction)
    {
      this._mainText.text = mainText;
      this._confirmTextButton.text = confirmText;
      this._cancelTextButton.text = cancelText;
      this.onConfirmAction = confirmAction;
      this.onCancelAction = cancelAction;
    }


    public void ConfirmButton()
    {
      onConfirmAction?.Invoke();
    }

    public void CancelButton()
    {
      onCancelAction?.Invoke();
    }
  }
}