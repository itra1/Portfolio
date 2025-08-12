using UnityEngine;
using System.Collections;

/// <summary>
/// Контроллер ворот на карте
/// </summary>
public class MapGateController : MonoBehaviour {

  bool waitOpenGate;
  public bool isOpen;
  [SerializeField]
  int gateNum;

  bool _isTapDown;

  void Start() {
    MapGamePlay.OnTapDrag += MovedCheck;
  }

  void OnDestroy() {
		MapGamePlay.OnTapDrag -= MovedCheck;
  }

  void MovedCheck(Vector2 moveDelta) {
    _isTapDown = false;
  }

  /// <summary>
  /// Тап мышой или пальцем
  /// </summary>
  void OnMouseDown() {
    if(isOpen) return;
    if(waitOpenGate) return;
    _isTapDown = true;
  }

  void OnMouseUp() {
    if(!_isTapDown) return;
    waitOpenGate = true;
    UiController.ClickButtonAudio();

    ShowGateScene();
  }

  void ShowGateScene() {

    AudioManager.SetSoundMixer(AudioSnapshotTypes.mapEffect0, 0.5f);
    //AudioEffects.SetSoundMixer(AudioSnapshotTypes.mapMusic0, 0.5f);
    GameManager.ShowGateScene(CloseGateScene, false, gateNum);
  }

  void CloseGateScene() {
    AudioManager.SetSoundMixer(AudioSnapshotTypes.mapEffectDef, 0.5f);
    //AudioEffects.SetSoundMixer(AudioSnapshotTypes.mapMusicDef, 0.5f);
    waitOpenGate = false;
  }

}
