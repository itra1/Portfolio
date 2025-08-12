using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using I2.Loc;

namespace it.UI.Settings {
  public class InputItem : Item {
    [SerializeField]
    private TMP_InputField _textfield;
    [SerializeField]
    private TextMeshProUGUI _textPlaceholder;
    [SerializeField]
    private TextMeshProUGUI _inputText;

    protected override void OnEnable() {
      base.OnEnable();
      _textfield.onSelect.AddListener(OnSelect);
      _textfield.onValueChanged.AddListener(OnChangeValue);
      _textfield.onDeselect.AddListener(OnDeselect);
    }

    protected override void OnDisable() {
      _textfield.onSelect.RemoveAllListeners();
      _textfield.onValueChanged.RemoveAllListeners();
      _textfield.onDeselect.RemoveAllListeners();
    }

    public void OnSelect(string data) {
      _textfield.text = "Input key";
      Debug.Log("OnSelect " + data);
    }
    public void OnDeselect(string data) {
      _textfield.text = "Value";
      Debug.Log("OnDeselect " + data);
    }
    private void OnChangeValue(string data) {
      Debug.Log("OnChangeValue " + data);
    }

  }
}