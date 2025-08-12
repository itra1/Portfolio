using UnityEngine;
using System.Collections;

namespace it.Game.Managers {
  public class KeyListener : MonoBehaviourBase {

    private com.ootii.Input.EasyInputSource _baseInputSource;
    private com.ootii.Input.EasyInputSource _gameInputSource;

    private void Update() {

      if (_baseInputSource == null)
        _baseInputSource = Game.Managers.GameManager.Instance.BaseInputSource;

      if (_baseInputSource != null)
        CheckBaseInput();

      if (_gameInputSource == null)
        _gameInputSource = Game.Managers.GameManager.Instance.GameInputSource;

    }

    private void CheckBaseInput() {

      if (_baseInputSource.IsJustPressed(KeyCode.Escape)) {
        GameManager.Instance.OnEscape();
      }

      if (_baseInputSource.IsJustPressed("Inventary")) {
        com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.InventaryOpenPanel);
        //Game.Managers.GameManager.Instance.Inventary.ShowInventaryPanel(true);
      }
      if (_baseInputSource.IsJustPressed("Symbols"))
      {
        com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.SymbolOpenPanel);
        //Game.Managers.GameManager.Instance.SymbolsManager.OpenSymbolDialog(true);
      }


    }

  }
}